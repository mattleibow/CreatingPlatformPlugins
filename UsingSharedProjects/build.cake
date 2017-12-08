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

    // copy the library to output
    EnsureDirectoryExists("./output/android");
    CopyFileToDirectory("./library/Library.Android/bin/Release/Library.dll", "./output/android");
    EnsureDirectoryExists("./output/ios");
    CopyFileToDirectory("./library/Library.iOS/bin/Release/Library.dll", "./output/ios");
    EnsureDirectoryExists("./output/uwp");
    CopyFileToDirectory("./library/Library.UWP/bin/Release/Library.dll", "./output/uwp");
    EnsureDirectoryExists("./output/netstandard");
    CopyFileToDirectory("./library/Library.NetStandard/bin/Release/netstandard1.0/Library.dll", "./output/netstandard");

    // build the nuget
    NuGetPack("./nuget/Package.nuspec", new NuGetPackSettings
    {
        BasePath = "./",
        OutputDirectory = "./output"
    });
});

RunTarget(target);