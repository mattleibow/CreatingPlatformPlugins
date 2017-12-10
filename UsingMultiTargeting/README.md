# Using Multi-Targeting Projects

Some libraries have mostly shared code, but do have a few members that are 
platform-specific. If the new .csproj format is used, multi-targeting can be 
used to not only have a single project with multiple outputs, but it can also 
generate a NuGet package with all the assemblies added.

Just like with [shared projects](../UsingSharedProjects), these libraries are
easy to develop using the `#if` preprocessor directive. But, instead of a 
shared project with multiple platform projects, there is just a single project 
that produces the platform assemblies.

More information on `#if` can be found in the C# documentation:  
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-if

More information on the platforms and .NET Standard versions can be found in
the .NET documentation:  
https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support

More information on new .csproj format can be found in
the .NET documentation:  
https://docs.microsoft.com/en-us/dotnet/core/tools/csproj

**(NOTE: Multi-targeting support is still undergoing development, but is still very usable. Visual Studio for Mac also requires an IDE extension - [more info](https://oren.codes/2017/08/29/use-all-tfms-with-sdk-style-projects-in-visual-studio-for-mac/))**

## The Solution

Using multi-targeting, there will be just 1 project:

 - one **project** _(for everything)_

This single project is typically a .NET Standard project that has been 
modified to indicate that it must also produce platform assemblies. There are 
a few things that need to change:

 - the `<TargetFramework>` element should be replaced with the plural version
   with a value of the semicolon-separated platforms:  
   `<TargetFrameworks>netstandard1.0;monoandroid403;xamarinios10;uap10.0</TargetFrameworks>`
 - the `MSBuild.Sdk.Extras` NuGet package should be added  _(set 
   `PrivateAssets="all"`)_:  
   `<PackageReference Include="MSBuild.Sdk.Extras" Version="1.1.0" PrivateAssets="all" />`
 - the `MSBuildSDKExtrasTargets` targets file must be imported:  
   `<Import Project="$(MSBuildSDKExtrasTargets)" Condition="Exists('$(MSBuildSDKExtrasTargets)')" />`

More information on multi-targeting can be found here:

 - https://github.com/dotMorten/NetStandardProjectTests
 - https://oren.codes/2017/01/04/multi-targeting-the-world-a-single-project-to-rule-them-all/
 - https://oren.codes/2017/08/29/use-all-tfms-with-sdk-style-projects-in-visual-studio-for-mac/

### Example Solution

An example scenario would be just 1 project, with 4 target platforms:

 - one **project** targeting `netstandard1.0;monoandroid403;xamarinios10;uap10.0` 
   _(.NET Standard 1.0, Xamarin.Android v4.0.3, Xamarin.iOS, UWP)_

Because most of the work is done in the single .csproj, there is no need for a 
.nuspec or multiple projects. This however means that extra care should be 
taken to correctly define the project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <!-- the list of platforms to support -->
    <TargetFrameworks>netstandard1.0;monoandroid403;xamarinios10;uap10.0</TargetFrameworks>
    <!-- a NuGet package must be generated during a build -->
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>

  <!-- any extra properties for the .NET Standard build -->
  <PropertyGroup Condition="'$(TargetFramework)' == 'netstandard1.0'">
    <!-- for a pure bait-and-switch library, the .NET Standard output is a reference-only output -->
    <ProduceReferenceAssembly>True</ProduceReferenceAssembly>
  </PropertyGroup>

  <!-- any extra properties for the Android build -->
  <PropertyGroup Condition="'$(TargetFramework)' == 'monoandroid403'">
  </PropertyGroup>

  <!-- any extra properties for the iOS build -->
  <PropertyGroup Condition="'$(TargetFramework)' == 'xamarinios10'">
  </PropertyGroup>

  <!-- any extra properties for the UWP build -->
  <PropertyGroup Condition="'$(TargetFramework)' == 'uap10.0'">
    <!-- UWP requires a few extra properties, such as versioning -->
    <TargetPlatformIdentifier>UAP</TargetPlatformIdentifier>
    <TargetPlatformVersion>10.0.16299.0</TargetPlatformVersion>
    <TargetPlatformMinVersion>10.0.10240.0</TargetPlatformMinVersion>
  </PropertyGroup>

  <!-- the UWP, and only the UWP, requires a platform NuGet -->
  <ItemGroup Condition="'$(TargetFramework)' == 'uap10.0'">
    <PackageReference Include="Microsoft.NETCore.UniversalWindowsPlatform" Version="6.0.4" />
  </ItemGroup>

  <!-- any additional package -->
  <ItemGroup>
    <!-- a special package is required to make multi-targeting work -->
    <PackageReference Include="MSBuild.Sdk.Extras" Version="1.1.0" PrivateAssets="all" />
  </ItemGroup>

  <!--  -->
  <Import Project="$(MSBuildSDKExtrasTargets)" Condition="Exists('$(MSBuildSDKExtrasTargets)')" />
</Project>
```

### Example Code

Because multi-targeting is more a project structure than a code pattern, the code itself is not different from the shared project structure:

```csharp
// a public class that is visible to any platform
public static class Screen
{
    // a public method that is visible to any platform
    public static double GetDensity()
    {
#if WINDOWS_UWP
        // code that is specific to UWP
        var di = DisplayInformation.GetForCurrentView();
        return di.RawPixelsPerViewPixel;
#elif __ANDROID__
        // code that is specific to Android
        var svc = Application.Context.GetSystemService(Context.WindowService);
        var wm = svc.JavaCast<IWindowManager>();
        var dm = new DisplayMetrics();
        wm.DefaultDisplay.GetMetrics(dm);
        return dm.Density;
#elif __IOS__
        // code that is specific to iOS
        return UIScreen.MainScreen.Scale;
#else
        // code that will run on any other platform
        throw new PlatformNotSupportedException();
#endif
    }
}
```

#### Consumer Project

The consumer app or library can then easily make use of the APIs:

```csharp
void DoScreenThings()
{
    double density = Screen.GetDensity();
}
```

### Example NuGet Package

Since the build of the library will automatically produce a NuGet, there is no
need for a .nuspec. Rather, the package properties are set in the .csproj:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <!-- the unique package ID -->
    <PackageId>PackageId</PackageId>
    <!-- the current version of the package -->
    <Version>1.0.0</Version>
    <!-- the authors text for the gallery -->
    <Authors>FirstName LastName</Authors>
    <!-- the description for the package manager UI -->
    <Description>Package description here.</Description>
  </PropertyGroup>

  <!-- ... the rest of the project file ... -->

</Project>
```
