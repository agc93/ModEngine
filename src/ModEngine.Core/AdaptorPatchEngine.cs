using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModEngine.Core
{
    public class AdaptorPatchEngine<TPatch> : IPatchEngine<TPatch> where TPatch : Patch
    {
        private readonly IPatchEngine<Patch> _engine;
        private readonly Func<TPatch, Patch> _selectorFunc;

        public AdaptorPatchEngine(IPatchEngine<Patch> engine, Func<TPatch, Patch> selectorFunc) {
            _engine = engine;
            _selectorFunc = selectorFunc;
        }

        public async Task<IEnumerable<FileInfo>> RunPatch(SourceFile sourceKey, IEnumerable<PatchSet<TPatch>> sets, string? targetName = null) {
            var transformedSets = sets.Select(ps =>
            {
                var newPatches = ps.Patches.Select(p => _selectorFunc(p));
                return new PatchSet<Patch> {
                    Name = ps.Name,
                    Patches = newPatches.ToList()
                };
            });
            var result = await _engine.RunPatch(sourceKey, transformedSets, targetName);
            return result;
        }
        
        public virtual async Task<IEnumerable<string>?> LoadFiles(Dictionary<string, IEnumerable<PatchSet<TPatch>>> patches,
            Func<string, IEnumerable<string>?>? extraFileSelector = null) {
            return null;
        }
    }
}