# Using Abstraction Projects

To reduce the chances of the .NET Standard library recieving features that do
not get implemented in the platform libraries, the public API can be
abstracted into interfaces. 

These interfaces can then be moved to a separate project so that other
platforms can easily be added later. This is especially useful if the library
is expected to recieve third party extensions (eg: for new platforms).

## The Solution

Using a mixture of shared code and platform code, there will be 3 types of
projects:

 - one **.NET Standard project** _(for the abstractions)_
 - another **.NET Standard project** _(for the bait)_
 - several **platform projects** - one per desired platform _(for the switch)_

### Example Solution

An example scenario would be 5 projects:

 - two **.NET Standard projects** targeting `netstandard1.0`
 - an **Android project**
 - an **iOS project**
 - a Windows 10 **UWP project**

### Example Code

#### Abstraction Project

The abstraction project contains the code that defines the APIs. However, it
does not contain any implementation details:

```csharp
// the public interface that defines the required API
public interface IScreen
{
    double GetDensity();
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

#### .NET Standard Project

The bait .NET Standard library simply has a dummy API that throws an exception
when trying to access the API:

```csharp
// the public class that provides access to the API
public static class Screen
{
    // the public property that throws on any unsupported platforms
    public static IScreen Instance =>
        throw new PlatformNotSupportedException();
}
```

Instead of duplicating this class per platform, a shared project could be 
created to remove the chances of mismatching APIs access points:

```csharp
// the public class that provides access to the API
public static class Screen
{
#if NETSTANDARD1_0
    // the public property that throws on any unsupported platforms
    public static IScreen Instance =>
        throw new PlatformNotSupportedException();
#else
    // a lazy construction of the actual platform implementation
    private static Lazy<IScreen> instance = 
        new Lazy<IScreen>(() => new ScreenImplementation());

    // the public property that provides the API
    public static IScreen Instance => instance.Value;
#endif
}
```

#### Platform Projects

The platform projects can now just contain implementation details and not be
concerned with what needs to be implemented as the compiler does all the work.
The implementation class just has to implement all the members of the
interface, and has to provide an API for accessing the various interfaces.

Each platform must provide access to the APIs (this could be in a shared 
project to remove the chances of mismatching APIs access points):

```csharp
// the public class that provides access to the API
public static class Screen
{
    // a lazy construction of the platform implementation
    private static Lazy<IScreen> instance = 
        new Lazy<IScreen>(() => new ScreenImplementation());

    // the public property that provides the API
    public static IScreen Instance => instance.Value;
}
```

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

Because the abstractions library must be included with each platform, we have 
to indicate this in the .nuspec:

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
    <!-- the platform-specific assemblies for Android -->
    <file src="output/netstandard/Library.Abstractions.dll" target="lib/MonoAndroid" />
    <file src="output/android/Library.dll" target="lib/MonoAndroid" />
    <!-- the platform-specific assemblies for iOS -->
    <file src="output/netstandard/Library.Abstractions.dll" target="lib/Xamarin.iOS" />
    <file src="output/ios/Library.dll" target="lib/Xamarin.iOS" />
    <!-- the platform-specific assemblies for UWP -->
    <file src="output/netstandard/Library.Abstractions.dll" target="lib/uap10.0" />
    <file src="output/uwp/Library.dll" target="lib/uap10.0" />

    <!-- the bait assemblies -->
    <file src="output/netstandard/Library.Abstractions.dll" target="lib/netstandard" />
    <file src="output/netstandard/Library.dll" target="lib/netstandard" />
  </files>
</package>
```

If the library is meant to be extended by third-party platform providers, we 
can split the package so that the abstractions can be used by the 
third-parties:

**Abstraction NuGet**

```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <!-- the unique package ID -->
    <id>PackageId.Abstractions</id>
    <!-- the current version of the package -->
    <version>1.0.0</version>
    <!-- the authors text for the gallery -->
    <authors>FirstName LastName</authors>
    <!-- the description for the package manager UI -->
    <description>Package description here.</description>
  </metadata>
  <files>
    <!-- the abstraction assembly -->
    <file src="output/netstandard/Library.Abstractions.dll" target="lib/netstandard" />
  </files>
</package>
```

**Platforms NuGet**

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
    <!-- indication that this package depends on the abstractions -->
    <dependencies>
      <dependency id="PackageId.Abstractions" version="1.0.0" />
    </dependencies>
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

**Third-Party NuGet**

```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <!-- the unique package ID -->
    <id>PackageId.ThirdParty</id>
    <!-- the current version of the package -->
    <version>1.0.0</version>
    <!-- the authors text for the gallery -->
    <authors>FirstName LastName</authors>
    <!-- the description for the package manager UI -->
    <description>Package description here.</description>
    <!-- indication that this package depends on the abstractions -->
    <dependencies>
      <dependency id="PackageId.Abstractions" version="1.0.0" />
    </dependencies>
  </metadata>
  <files>
    <!-- the platform-specific assembly for the third-party platform -->
    <file src="output/mac/Library.dll" target="lib/Xamarin.Mac" />
  </files>
</package>
```
