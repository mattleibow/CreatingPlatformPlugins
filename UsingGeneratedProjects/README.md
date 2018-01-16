# Using Generated Projects

There are some libraries have a lot of code, some of it shared, and some of it
platform-specific. Often, these libraries will require a great effort to get
into a cross-platform state, or, it may never be possible.

An example would be some UI library that has the ability to show a dialog to 
the user. This library still has a uniform API, but the core aspect of the 
library is the user interface. It will be almost impossible to share UI code,
but the library still has to be cross-platform.

If the cross-platform assembly is generated, the there is no restriction on 
what can be done inside the assembly. All internal types and members will be 
ignored by the generator and can be platform-specific.

## The Solution

Using the generator, there will be 3 types of projects:

 - one **shared project** _(for any shared code)_
 - one **.NET Standard project** _(for the bait)_
 - several **platform projects** - one per desired platform _(for the switch)_

Since all the code exists in the platform projects, there is no need to make 
sure that the implementations only use cross-platform types. The only part of 
the assembly that needs to be cross-platform is the public API.

The generator is a simple command-line utility that can be used in a CI 
environment or locally. It is 
[found on nuget.org with a package ID of `Microsoft.DotNet.BuildTools.GenAPI`](https://www.nuget.org/packages/Microsoft.DotNet.BuildTools.GenAPI).
It can be downloaded by a CI or added to the `PATH` environment variable. Once
available for execution, the CLI is simple:

```ps
genapi.exe "Library.dll" -out "Library.gen.cs"
```

If the tool is used with a [Cake](https://cakebuild.net/) build, then all that
is needed is the install of the tool, and then a simple run:

```csharp
#tool nuget:?package=Microsoft.DotNet.BuildTools.GenAPI&prerelease

var genapiPath = Context.Tools.Resolve("GenAPI.exe");
StartProcess(genapiPath, $"\"./out/Library.dll\" -out \"./out/Library.gen.cs\"");
```

The basic execution requires that an assembly be passed in, and a location on
where to place the generated source code. This code can then be added to any
class library project.


### Example Solution

An example scenario would be 5 projects:

 - one **shared project**
 - one **.NET Standard project** targeting `netstandard1.0`
 - an **Android project**
 - an **iOS project**
 - a Windows 10 **UWP project**

### Example Code

An example of what the platform code may look like:

#### Android Project

```csharp
// a public class that is visible to any platform
public static class Screen
{
    // a public method that is visible to any platform
    public static void ShowDensity()
    {
        // code that is specific to Android
        var svc = Application.Context.GetSystemService(Context.WindowService);
        var wm = svc.JavaCast<IWindowManager>();
        var dm = new DisplayMetrics();
        wm.DefaultDisplay.GetMetrics(dm);
        var density = dm.Density;
        var message = $"Screen Density: {density}";

        // UI code
        Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
    }
}
```

#### iOS Project

```csharp
// a public class that is visible to any platform
public static class Screen
{
    // a public method that is visible to any platform
    public static void ShowDensity()
    {
        // code that is specific to iOS
        var density = UIScreen.MainScreen.Scale;
        var message = $"Screen Density: {density}";

        // UI code
        new UIAlertView("Screen Properties", message, (IUIAlertViewDelegate)null, "OK").Show();
    }
}
```

#### Windows Project

```csharp
// a public class that is visible to any platform
public static class Screen
{
    // a public method that is visible to any platform
    public static void ShowDensity()
    {
        // code that is specific to Windows
        var di = DisplayInformation.GetForCurrentView();
        var density = di.RawPixelsPerViewPixel;
        var message = $"Screen Density: {density}";

        // UI code
        var dialog = new MessageDialog(message, "Screen Properties");
        dialog.Commands.Add(new UICommand("OK"));
        await dialog.ShowAsync();
    }
}
```

#### Generated Code

The generator does not actually do anything special, as it just generates a 
code file with a bunch of stubs:

```csharp
namespace Library
{
    public static partial class Screen
    {
        public static void ShowProperties() { }
    }
}
```

#### Consumer Project

The consumer app or library can then easily make use of the APIs:

```csharp
void DoScreenThings()
{
    Screen.ShowProperties();
}
```

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
