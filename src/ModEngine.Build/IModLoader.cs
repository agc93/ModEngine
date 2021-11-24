using System;
using System.Collections.Generic;
using ModEngine.Core;

namespace ModEngine.Build
{
    /// <summary>
    /// This is purely a convenience interface for abstracting the process of "loading" mods from any source.
    /// You do not need to implement this if it's not needed, but it can be convenient.
    /// </summary>
    public interface IModLoader<TMod> where TMod : Mod
    {
        Dictionary<string, TMod> LoadFromFiles(IEnumerable<string> filePaths, List<Func<TMod, bool>>? loadRequirements = null);
    }
}