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
 - [**Generated Projects**](UsingGeneratedProjects)  
   All the code lives in the platform projects, and then the cross-platform code is genertaed using GenAPI.

## Bait-And-Switch

The concept of a bait-and-switch NuGet is very simple and consists of 2 or more
assemblies with the same name:

 - a dummy assembly, named `AssemblyName.dll`
 - the real assembly, also named `AssemblyName.dll`

The process of using a bait-and-switch is to compile against the dummy assembly
and then to run against the real assembly.

### The Problem

The reason this whole process is required is simply because a .NET Standard 
library or PCL cannot reference a platform-specific project. 

An example scenario would be to fetch the current device's screen density. 
This is a very device-specific process, and there is no way to do this from 
a .NET Standard library.

A .NET Standard library may be required because the app is a Xamarin.Forms app,
which is designed to be written in a .NET Standard library. Or the library may
be targeting multiple platforms, and needs to be consumable from any .NET
Standard-compliant platform - such as .NET Core or Unity.

### The Solution

Bait-and-switch is a process in which to "trick" the app into thinking that it
is using a .NET Standard library, but in fact is using a platform library. This
is typically easy to do as _assemblies are loaded by name_. So, as long as the 
final assembly names match, the app will never know that the assembly it is 
running with is not the same as the one it was compiled with.

Obviously, the _public_ types must also match otherwise there will be a runtime
error when the app attempts to access a member, but it does not exist.

The dummy assembly doesn't have to do anything (it usually just throws an 
exception), it just needs to have the same name as the platform assembly and 
have the same _public_ types and members.

### An Example Library

Assume that there is some requirement for a .NET Standard library that fetches
the screen density. Assume this is the required/desired API:

```csharp
public static class Screen
{
    public static double GetDensity();
}
```

As can be seen, the API is pure BCL and does not appear to require any platform
types. Since nothing can be done in the dummy .NET Standard library to fetch 
the screen density, the only thing to do is to throw an exception:

```csharp
public static class Screen
{
    public static double GetDensity()
    {
        throw new PlatformNotSupportedException();
    }
}
```

The exception _should_ never actually be thrown at runtime, because this 
assembly is never going to make it into the final app. This assembly
is purely for the compiler during the compilation of the consumer .NET 
Standard library.

But, the actual implementation is very platform-specific - such as with
the iOS implementation:

```csharp
public static class Screen
{
    public static double GetDensity()
    {
        return UIScreen.MainScreen.Scale;
    }
}
```

The type `UIScreen` is only available on iOS, will only ever be available on 
iOS and this type will never reach .NET Standard definition. The only way to 
create this library is to create an iOS library, but this means that it will 
not be consumable from any .NET Standard library.

Both the dummy .NET Standard library and the platform library _must_ be 
compiled with the same name. Then, the consumer .NET Standard library 
references the dummy .NET Standard library and the consumer app references
the platform library.

If a NuGet is created, then the work will be done automatically and the only 
step that needs to be taken is to ensure that both the app and the consumer 
.NET Standard library reference that NuGet.

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
