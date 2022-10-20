using System.Collections.Generic;
using ModEngine.Core;

namespace ModEngine.Merge;

public record MergeComponent<TMod> where TMod : Mod
{
    public string? Name { get; set; }
    public Dictionary<string, string> Parameters { get; init; } = new();
    public IEnumerable<TMod> Mods { get; init; } = new List<TMod>();
    public string? Message { get; init; }
    public Dictionary<string, string>? MergedResources { get; init; }
    public int Priority { get; init; } = 10;
}