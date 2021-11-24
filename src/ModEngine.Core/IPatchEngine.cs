using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ModEngine.Core
{
    public interface IPatchEngine<TPatch> where TPatch : Patch
    {
        public Task<IEnumerable<FileInfo>> RunPatch(string sourceKey, IEnumerable<PatchSet<TPatch>> sets, string? targetName = null);

        public async Task<IEnumerable<string>?> LoadFiles(Dictionary<string, IEnumerable<PatchSet<TPatch>>> patches,
            Func<string, IEnumerable<string>?>? extraFileSelector = null) {
            return null;
        }

        public IEnumerable<Func<string, IEnumerable<string>>>? SourceFileSelectors => null;
    }
}