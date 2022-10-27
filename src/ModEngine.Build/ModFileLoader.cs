using System.Text.Json;
using Microsoft.Extensions.Logging;
using ModEngine.Core;

namespace ModEngine.Build
{
    /// <summary>
    /// A simple <see cref="IModLoader{TMod}"/> implementation for loading mods from loose files.
    /// </summary>
    /// <typeparam name="TMod">The mod type to deserialize from the files.</typeparam>
    public class ModFileLoader<TMod> : IModLoader<TMod> where TMod : Mod
    {
        private readonly ILogger<ModFileLoader<TMod>>? _logger;

        public ModFileLoader()
        {
            
        }

        public ModFileLoader(ILogger<ModFileLoader<TMod>> logger) : this()
        {
            _logger = logger;
        }
        public Dictionary<string, TMod> LoadFromFiles(IEnumerable<string> filePaths, List<Func<TMod, bool>>? loadRequirements = null)
        {
            var fileMods = new Dictionary<string, TMod>();
            foreach (var file in filePaths.Where(f => f.Length > 0 && File.Exists(f) && File.ReadAllText(f).Any()))
            {
                try
                {
                    _logger?.LogTrace($"Attempting to load mod data from {file}");
                    var allText = File.ReadAllText(file);
                    var jsonOpts = new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                        Converters =
                        {
                            new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                        }
                    };
                    if (JsonSerializer.Deserialize<TMod>(allText, jsonOpts) is {} jsonMod) {
                        var requirements = loadRequirements?.ToList() ?? new List<Func<TMod, bool>>();
                        if (!requirements.Any() || requirements.All(req => req(jsonMod))) {
                            _logger?.LogTrace($"Successfully loaded mod data from {file}: {jsonMod.GetLabel(Path.GetFileName(file))}");
                            fileMods.Add(file, jsonMod);
                        }
                    }
                }
                catch (Exception)
                {
                    _logger?.LogWarning($"Failed to load mod data from {file}!");
                }
            }
            return fileMods;
        }
    }
}