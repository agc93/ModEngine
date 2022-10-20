using System.Collections.Generic;
using ModEngine.Core;

namespace ModEngine.Merge;

public interface IPresetLoader<TMod> where TMod : Mod
{
    IEnumerable<TMod> LoadPresets();
}