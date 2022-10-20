using System.Collections.Generic;
using ModEngine.Core;

namespace ModEngine.Merge;

public interface IMergeProvider<TMod> where TMod : Mod
{
    public string Name { get; }
    public IEnumerable<MergeComponent<TMod>> GetMergeComponents(List<string>? searchPaths);
}