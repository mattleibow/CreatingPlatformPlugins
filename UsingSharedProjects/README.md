# Using Shared Projects

Some libraries have mostly shared code, but do have a few members that are
platform-specific. This keeps all the code together, but still allows for
multiple platform libraries.

These libraries are easy to develop using the infamous `#if` preprocessor
directive. This is just like a normal `if` statement, but runs during
compilation to conditionally include/exclude code. Although it is often best
to avoid using too many preprocessor directives as they do tend towards chaos,
and spaghetti code, they are a very powerful and useful tool.

Although custom definitions can be created, there are a few default
preprocessor definitions that are available:

| Platform  | Definitions                     | Special Definitions                        |
| --------- | ------------------------------- | ------------------------------------------ |
|           |                                 | `DEBUG` (only for the Debug configuration) |
| Android   | `__ANDROID__`, `__MOBILE__`     | `__ANDROID_<api>__` (eg: `__ANDROID_14__`) |
| iOS       | `__IOS__`, `__MOBILE__`         | `__UNIFIED__`                              |
| macOS     | `__MACOS__`                     | `__UNIFIED__`                              |
| tvOS      | `__TVOS__`, `__MOBILE__`        | `__UNIFIED__`                              |
| UWP       | `__WINDOWS_UWP__`, `NETFX_CORE` |                                            |
| watchOS   | `__WATCHOS__`, `__MOBILE__`     | `__UNIFIED__`                              |

More information on `#if` can be found in the C# documentation:  
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives/preprocessor-if

More information on the platforms and .NET Standard versions can be found in
the .NET documentation:  
https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support

## The Solution

Using an entirely shared approach, there will be 3 types of projects:

 - one **shared project** _(for the code)_
 - one **.NET Standard project** _(for the bait)_
 - several **platform projects** - one per desired platform _(for the switch)_

Since all the code is in the shared project, the .NET Standard project and each
of the platform projects just need to reference the shared project. Any other
code that is not platform-specific can also exist in the shared project instead
of being duplicated for each platform project.

### Example Solution

An example scenario would be 5 projects:

 - one **shared project**
 - one **.NET Standard project** targeting `netstandard1.0`
 - an **Android project**
 - an **iOS project**
 - a Windows 10 **UWP project**

### Example Code

An example of what shared code may look like with `#if`, `#elif` and `#endif` 
preprocessor directive:

```csharp
// a public class that is visible to any platform
public class Screen
{
    // a public method that is visible to any platform
    public double GetDensity()
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

## The NuGet Package

Since most of the work is actually done by NuGet, the definition of the .nuspec
is very important. NuGet will always choose the more specific platform.

If there is an assembly for `netstandard`, `MonoAndroid` and `MonoAndroid70`,
depending on the destination project, NuGet will select the assembly that best
fits:

 - for a Android v7.0+ project, the `MonoAndroid70` assembly will be selected
 - for a Android v1.0+ project, the `MonoAndroid` assembly will be selected
 - for any other project type, the `netstandard` assembly will be selected

More information on creating NuGet packages can be found in the NuGet
documentation:  
https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package

## Example NuGet Package

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
