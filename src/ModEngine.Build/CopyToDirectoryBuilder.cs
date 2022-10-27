using BuildEngine;

namespace ModEngine.Build;

public class CopyToDirectoryBuilder : IModBuilder
{
    public Task<(bool Success, FileSystemInfo Output)> RunBuildAsync(IBuildContext buildContext, string targetFileName) {
        if (buildContext is not DirectoryBuildContext ctx) {
            throw new InvalidOperationException("Unsupported build context!");
        }

        if (File.Exists(targetFileName)) {
            throw new InvalidOperationException(
                "Target is a file! Provide an existing directory or a new path to copy to");
        }

        var target = new DirectoryInfo(targetFileName);
        if (!target.Exists) {
            target.Create();
        }

        var sourceDir = ctx.WorkingDirectory;
        sourceDir.CopyTo(target.FullName, fi => true);
        return Task.FromResult<(bool Success, FileSystemInfo Output)>((target.GetFileSystemInfos().Any(), target));
    }
}