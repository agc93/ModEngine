using System.Collections.Generic;
using BuildEngine;
using HexPatch;
using ModEngine.Build;
using ModEngine.Core;

using static System.Console;
using Mod = ModEngine.Core.Mod;
using Patch = ModEngine.Core.Patch;
using FilePatchSet = HexPatch.FilePatchSet;

namespace ModEngine.Console
{
    class Program
    {
        static void Main(string[] args) {
            var dirCtxBuilder = new DirectoryBuildContextFactory(null);
            var hexEngine = new HexPatchEngine(new FilePatcher(null, null), null);
            var engines = new List<IPatchEngine<Patch>> { hexEngine };

            var serv = new ModPatchService<WingmanMod, DirectoryBuildContext>(dirCtxBuilder.CreateContext("test"), 
                new SourceFileService(null), null, null);
            serv.AddEngine(hexEngine, m => HexPatchEngine.HexPatchHelpers.ConvertPatches(m.FilePatches));
        }

        
    }
}
