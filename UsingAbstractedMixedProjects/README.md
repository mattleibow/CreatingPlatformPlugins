# Using Mixed Projects With Abstraction

To reduce the chances of the .NET Standard library recieving features that do
not get implemented in the platform libraries, the public API can be
abstracted into interfaces. The shared project defines the interfaces, and
then platform libraries must implement all the members to avoid any compiler
errors.

## The Solution

Using a mixture of shared code and platform code, there will be 3 types of
projects:

 - one **shared project** _(for the code)_
 - one **.NET Standard project** _(for the bait)_
 - several **platform projects** - one per desired platform _(for the switch)_

### Example Solution

An example scenario would be 5 projects:

 - one **shared project**
 - one **.NET Standard project** targeting `netstandard1.0`
 - an **Android project**
 - an **iOS project**
 - a Windows 10 **UWP project**

### Example Code

#### Shared Project

The shared project contains the code that defines the APIs as well as provides
access to those APIs. It, however, does not contain any implementation details:

```csharp
// the public interface that defines the required API
public interface IScreen
{
    double GetDensity();
}

// the public class that provides access to the API
public static class Screen
{
#if NETSTANDARD1_0
    // the public property that throws on any unsupported platforms
    public static IScreen Instance =>
        throw new PlatformNotSupportedException();
#else
    // a lazy construction of the platform implementation
    private static Lazy<IScreen> instance = 
        new Lazy<IScreen>(() => new ScreenImplementation());

    // the public property that provides the API
    public static IScreen Instance => instance.Value;
#endif
}
```

In addition to the API definition, the API can be extended using extension
methods. This allows for any features to be "added" to API, without having to
change the API definition:

```csharp
// an extension method that "adds" members to the API interface
public static class ScreenExtensions
{
    public static bool IsDense(this IScreen screen)
    {
        return screen.GetDensity() > 1.0;
    }
}
```

#### Platform Projects

The platform projects can now just contain implementation details and not be
concerned with what needs to be implemented as the compiler does all the work.
The implementation class just has to implement all the members of the
interface, and then it will just work:

**Android Project**

```csharp
// an internal type that is not visible to the public
internal class ScreenImplementation : IScreen
{
    public double GetDensity()
    {
        var svc = Application.Context.GetSystemService(Context.WindowService);
        var wm = svc.JavaCast<IWindowManager>();
        var dm = new DisplayMetrics();
        wm.DefaultDisplay.GetMetrics(dm);
        return dm.Density;
    }
}
```

**iOS Project**

```csharp
// an internal type that is not visible to the public
internal class ScreenImplementation : IScreen
{
    public double GetDensity()
    {
        return UIScreen.MainScreen.Scale;
    }
}
```

**UWP Project**

```csharp
// an internal type that is not visible to the public
internal class ScreenImplementation : IScreen
{
    public double GetDensity()
    {
        var di = DisplayInformation.GetForCurrentView();
        return di.RawPixelsPerViewPixel;
    }
}
```

#### Consumer Project

The consumer app or library can then easily make use of the APIs:

```csharp
void DoScreenThings()
{
    IScreen screen = Screen.Instance;
    double density = screen.GetDensity();
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
