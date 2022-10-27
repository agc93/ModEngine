using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuildEngine;
using Microsoft.Extensions.Logging;
using ModEngine.Build.Diagnostics;
using ModEngine.Core;

namespace ModEngine.Build
{
    /// <summary>
    /// A reference implementation of a <see cref="BuildService{TContext}"/> for running patches from a number of patch engines.
    /// </summary>
    /// <remarks>
    /// While useful, and largely feature-complete, it is quite likely you will
    /// need to reimplement this in your own project to match each
    /// project/game's requirements
    /// </remarks>
    /// <typeparam name="TMod">The mod type to load and run.</typeparam>
    /// <typeparam name="TContext">The build context type.</typeparam>
    public class ModPatchService<TMod, TContext> : BuildService<TContext>, IDisposable where TContext : IBuildContext where TMod : Mod
    {
        private readonly List<PatchEngineDefinition<TMod, Patch>> _patchEngines = new();
        protected IEnumerable<PatchEngineDefinition<TMod, Patch>> PatchEngines => _patchEngines.OrderBy(p => p.Priority);
        protected readonly ISourceFileService FileService;
        protected readonly ILogger<ModPatchService<TMod, TContext>>? Logger;
        protected readonly IModBuilder? ModBuilder;
        public List<TMod> Mods { get; }
        
        public Func<TContext, FileInfo>? PreBuildAction { get; set; }
        
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ModPatchService(List<TMod> mods, TContext context, ISourceFileService fileService, IModBuilder? modBuilder, ILogger<ModPatchService<TMod, TContext>>? logger) : base(context) {
            FileService = fileService;
            Logger = logger;
            Mods = mods;
            ModBuilder = modBuilder;
        }
        
        public ModPatchService(TContext context, ISourceFileService fileService, IModBuilder? modBuilder, ILogger<ModPatchService<TMod, TContext>>? logger) : this(new List<TMod>(), context, fileService, modBuilder, logger) {
        }

        public ModPatchService(List<TMod> mods, TContext context, ISourceFileService fileService, IModBuilder? modBuilder, IEnumerable<PatchEngineDefinition<TMod, Patch>> patchEngineDefinitions, ILogger<ModPatchService<TMod, TContext>>? logger) : this(mods, context, fileService, modBuilder, logger) {
            _patchEngines.AddRange(patchEngineDefinitions);
        }
        
        public ModPatchService(TContext context, ISourceFileService fileService, IModBuilder? modBuilder, IEnumerable<PatchEngineDefinition<TMod, Patch>> patchEngineDefinitions, ILogger<ModPatchService<TMod, TContext>>? logger) : this(new List<TMod>(), context, fileService, modBuilder, patchEngineDefinitions, logger) {
        }

        public virtual ModPatchService<TMod, TContext> AddEngine(IPatchEngine<Patch> patchEngine,
            Func<TMod, Dictionary<string, IEnumerable<PatchSet<Patch>>>> patchSelector, int? priority = 10) {
            _patchEngines.Add(new PatchEngineDefinition<TMod, Patch>(patchEngine, patchSelector) {Priority = priority ?? 10});
            return this;
        }

        public virtual ModPatchService<TMod, TContext> AddEngines(params PatchEngineDefinition<TMod, Patch>[] patchEngineDefinitions) {
            foreach (var definition in patchEngineDefinitions) {
                _patchEngines.Add(definition);
            }
            return this;
        }

        public virtual async Task<ModPatchService<TMod, TContext>> RunPatches() {
            foreach (var patchEngine in _patchEngines) {
                foreach (var mod in Mods) {
                    var modifiedFiles = new List<FileInfo>();
                    Logger?.LogInformation($"Running patches for {mod.GetLabel()}");
                    var patches = patchEngine.PatchSelector(mod);
                    foreach (var (targetFile, patchSets) in patches) {
                        var srcFile = new SourceFile(targetFile);
                        try {
                            var realFile = BuildContext.GetFile(targetFile);
                            if (realFile != null) {
                                srcFile.File = realFile;
                            }
                        }
                        catch {
                            // ignored
                        }
                        var patchSetList = patchSets.ToList();
                        Logger?.LogDebug($"Patching {Path.GetFileName(targetFile)}...");
                        var fi = await patchEngine.Engine.RunPatch(srcFile, patchSetList);
                        modifiedFiles.AddRange(fi);
                    }
                    Logger?.LogDebug($"Modified {modifiedFiles.Count} files: {string.Join(", ", modifiedFiles.Select(f => f.Name))}");
                }
            }

            return this;
        }

        public override async Task<(bool Success, FileSystemInfo? Output)> RunBuildAsync(string targetFileName) {
            var bResult = PreBuildAction?.Invoke(Context);
            if (bResult is {Exists: true})
            {
                targetFileName = bResult.Name;
            }

            if (ModBuilder != null) {
                var result = await ModBuilder.RunBuildAsync(BuildContext, targetFileName);
                return result;
            }
            return (true, null);
        }

        public virtual async Task<ModPatchService<TMod, TContext>> LoadFiles(Func<string, IEnumerable<string>?>? extraFileSelector = null) {
            foreach (var patchEngine in _patchEngines) {
                var allPatches = this.Mods.SelectMany(m => patchEngine.PatchSelector(m)).ToList();
                var patches = new Dictionary<string, IEnumerable<PatchSet<Patch>>>();
                foreach (var requestSet in allPatches) {
                    if (patches.ContainsKey(requestSet.Key)) {
                        patches[requestSet.Key] = patches[requestSet.Key].Concat(requestSet.Value);
                    }
                    else {
                        patches.Add(requestSet.Key, requestSet.Value);
                    }
                }
                var files = await patchEngine.Engine.LoadFiles(patches, extraFileSelector);
                files ??= patches
                    .GroupBy(fp => fp.Key)
                    .Where(g => g.Any())
                    .Select(g => g.Key)
                    .SelectMany(k =>
                    {
                        if (patchEngine.Engine.SourceFileSelectors != null &&
                            patchEngine.Engine.SourceFileSelectors.Any()) {
                            return patchEngine.Engine.SourceFileSelectors.SelectMany(s => s(k).Append(k));    
                        }
                        return new[] { k };
                    })
                    .Distinct()
                    .ToList();
                foreach (var file in files)
                {
                    var srcFile = FileService.LocateFile(Path.GetFileName(file));
                    if (srcFile == null) {
                        throw new SourceFileNotFoundException(file);
                    }
                    BuildContext.AddFile(Path.GetDirectoryName(file) ?? file, srcFile);
                    if (extraFileSelector != null) {
                        var extraFiles = extraFileSelector.Invoke(file);
                        foreach (var eFile in extraFiles ?? new List<string>()) {
                            var exFile = FileService.LocateFile(Path.GetFileName(eFile));
                            if (exFile == null) {
                                throw new SourceFileNotFoundException(eFile);
                            }
                            BuildContext.AddFile(Path.GetDirectoryName(eFile) ?? eFile, exFile);
                        }
                    }
                }
            }
            return this;
        }
        
        public virtual (bool Success, T? Output)? RunAction<T>(Func<TContext, T> buildFunc) where T : class
        {
            try
            {
                var result = buildFunc.Invoke(Context);
                return (true, result);
            }
            catch (Exception e)
            {
                Logger?.LogWarning(e, "Error encountered during patch action!");
                return (false, null);
            }
        }
    }

    /// <summary>
    /// Used to register patch engines with a given <see cref="ModPatchService{TMod,TContext}"/>. 
    /// Represents a given patch engine and what patches it uses.
    /// </summary>
    /// <typeparam name="TPatch">The patch type this engine uses.</typeparam>
    public record PatchEngineDefinition<TMod, TPatch>(IPatchEngine<TPatch> Engine, Func<TMod, Dictionary<string, IEnumerable<PatchSet<TPatch>>>> PatchSelector) where TMod : Mod where TPatch : Patch
    {
        public IPatchEngine<TPatch> Engine { get; set; } = Engine;
        public Func<TMod, Dictionary<string, IEnumerable<PatchSet<TPatch>>>> PatchSelector { get; set; } = PatchSelector;
        public int Priority { get; set; }
    }
}