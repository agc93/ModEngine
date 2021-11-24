using System.IO;

namespace ModEngine.Build
{
    /// <summary>
    /// Retrieves, unpacks or otherwise loads requested files ready for patching.
    /// </summary>
    public interface ISourceFileService
    {
        FileInfo? LocateFile(string fileName);
    }
}