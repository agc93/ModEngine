using System.Collections.Generic;
using ModEngine.Core;

namespace ModEngine.Merge;

public class ModPreset<TMod> where TMod : Mod
{
    public int Version { get; set; } = 1;
    public string? EngineVersion { get; set; }
    public Dictionary<string, string> ModParameters { get; set; }
    public List<TMod> Mods { get; set; }
}