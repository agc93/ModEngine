using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModEngine.Build
{
    public static class FileSelectors
    {
        public static Func<string, IEnumerable<string>> SidecarFiles(Dictionary<string, IEnumerable<string>> sidecars) {
            return (oFile) => {
                var srcExt = Path.GetExtension(oFile);
                if (sidecars.ContainsKey(srcExt)) {
                    return sidecars[srcExt].Select(dExt => {
                        return Path.Combine(Path.GetDirectoryName(oFile), $"{Path.GetFileNameWithoutExtension(oFile)}{dExt}");
                    });
                }
                return null;
            };
        }
    }
}