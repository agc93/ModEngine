using System.Collections.Generic;
using HexPatch;
using ModEngine.Core;


namespace ModEngine.Console
{
    public class WingmanMod : ModEngine.Core.Mod
    {
        public Dictionary<string, List<FilePatchSet>> FilePatches { get; set; } = new();
    }
}