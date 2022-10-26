#load nuget:?package=DevelopEngine.Cake&version=0.1.4

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Setup<BuildData>(ctx => {
    Information("Running build setup...");
    var publish = HasEnvironmentVariable("GITHUB_REF") 
        && (EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v") || EnvironmentVariable("GITHUB_REF") == "refs/heads/main");
    return new BuildData {
        ReleaseBuild = publish,
        Projects = GetProjects(File("./src/ModEngine.sln"), configuration),
        BuildVersion = packageVersion,
        Configuration = configuration
    };
});

Task("Default")
.IsDependentOn("Post-Build")
.IsDependentOn("NuGet")
.IsDependentOn("Publish");

Task("Publish")
.IsDependentOn("Publish-NuGet-Package");

RunTarget(target);
