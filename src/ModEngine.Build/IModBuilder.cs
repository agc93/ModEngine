using System;
using System.IO;
using System.Threading.Tasks;
using BuildEngine;

namespace ModEngine.Build
{
    /// <summary>
    /// This is a convenience interface for the "final build" of a mod.
    /// This is usually something like packing or compiling, but is not required.
    /// </summary>
    public interface IModBuilder
    {
        public Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(IBuildContext buildContext, string targetFileName);
    }
}