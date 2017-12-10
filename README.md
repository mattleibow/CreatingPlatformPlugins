# Creating Platform Plugins

There are a few ways to create a plugin or bait-and-switch library which
permits the use of platform-specific code from a cross-platform project:

 - [**Shared Project**](UsingSharedProjects)  
   All the code exists in a single shared project, controlled with `#if`
   preprocessor directives. This is good for projects with a large API
   but only a small set of platform-specific code.
 - [**Mixed Projects**](UsingMixedProjects)  
   The code exists in both the shared and platform projects. The code still has
   a few `#if` preprocessor directives, but the main implementation logic has
   been moved to the platform projects. This is good for projects that have a 
   few platform differences, but those differences require more complex code.
 - [**Abstracted Project**](UsingAbstractionProjects)  
   The code is split between a base/core project which defines the public API 
   using a set of interfaces, and then a series of platform projects which 
   independently define the implementation of those interfaces. This is good 
   for projects with larger APIs as well as for libraries that needs to 
   support third-party extensions or platforms.
 - [**Mixed Projects with Abstraction**](UsingAbstractedMixedProjects)  
   The code exists in both the shared and platform projects. Instead of relying
   of many API matching or numerous `#if` preprocessor directives, the public 
   API is abstracted out into an interface which can be implemented on each 
   platform separately.
 - [**Multi-Targeting**](UsingMultiTargeting)  
   All the code lives in a single project, which is set up to produce multiple 
   platform assemblies. The actual implementation may be to use a set of `#if` 
   preprocessor directives or it may be to abstract interfaces.

## The NuGet Package

Since most of the work is actually done by NuGet, the definition of the .nuspec
(or the .csproj for multi-targeting) is very important. NuGet will always 
choose the more specific platform.

If there is an assembly for `netstandard`, `MonoAndroid` and `MonoAndroid70`,
depending on the destination project, NuGet will select the assembly that best
fits:

 - for a Android v7.0+ project, the `MonoAndroid70` assembly will be selected
 - for a Android v1.0+ project, the `MonoAndroid` assembly will be selected
 - for any other project type, the `netstandard` assembly will be selected

More information on creating NuGet packages can be found in the NuGet
documentation:  
https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package

### Example NuGet Package

There are many elements that can be used to customize the package, but only a
few are required to create a bait-and-switch package:

```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <!-- the unique package ID -->
    <id>PackageId</id>
    <!-- the current version of the package -->
    <version>1.0.0</version>
    <!-- the authors text for the gallery -->
    <authors>FirstName LastName</authors>
    <!-- the description for the package manager UI -->
    <description>Package description here.</description>
  </metadata>
  <files>
    <!-- the platform-specific assembly for Android -->
    <file src="output/android/Library.dll" target="lib/MonoAndroid" />
    <!-- the platform-specific assembly for iOS -->
    <file src="output/ios/Library.dll" target="lib/Xamarin.iOS" />
    <!-- the platform-specific assembly for UWP -->
    <file src="output/uwp/Library.dll" target="lib/uap10.0" />

    <!-- the bait assembly -->
    <file src="output/netstandard/Library.dll" target="lib/netstandard" />
  </files>
</package>
```
