using System.Collections.Generic;

namespace ModEngine.Core
{
    public class PatchSet<TPatch> where TPatch : Patch
    {
        public string? Name {get;set;}
        public List<TPatch> Patches { get; set; } = new();
    }

    public class PatchSet : PatchSet<Patch>
    {
    }
}