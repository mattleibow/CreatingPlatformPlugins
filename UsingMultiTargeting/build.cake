var target = Argument("target", "Default");

Task("Default")
    .Does(() =>
{
    // build the library
    NuGetRestore("./library/Library.sln", new NuGetRestoreSettings
    {
        Verbosity = NuGetVerbosity.Quiet
    });
    MSBuild("./library/Library.sln", new MSBuildSettings
    {
        Verbosity = Verbosity.Minimal,
        Configuration = "Release",
        PlatformTarget = PlatformTarget.MSIL,
        MSBuildPlatform = MSBuildPlatform.x86,
    });

    // copy the library and nuget to output
    EnsureDirectoryExists("./output");
    CopyDirectory("./library/Library/bin/Release/", "./output/");
});

RunTarget(target);