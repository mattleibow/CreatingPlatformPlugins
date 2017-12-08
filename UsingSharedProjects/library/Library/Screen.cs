using System;

#if WINDOWS_UWP
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
#elif __ANDROID__
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
#elif __IOS__
using UIKit;
#endif

namespace Library
{
	public static class Screen
	{
		public static double GetDensity()
		{
			return GetProperties().Density;
		}

		public static ScreenProperties GetProperties()
		{
#if WINDOWS_UWP
			var di = DisplayInformation.GetForCurrentView();
			var properties = new ScreenProperties
			{
				Density = di.RawPixelsPerViewPixel
			};
			if (ApiInformation.IsPropertyPresent(typeof(DisplayInformation).FullName, nameof(DisplayInformation.ScreenWidthInRawPixels)))
			{
				properties.PixelWidth = (int)di.ScreenWidthInRawPixels;
				properties.PixelHeight = (int)di.ScreenHeightInRawPixels;
			}
			return properties;
#elif __ANDROID__
			var wm = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			var dm = new DisplayMetrics();
			wm.DefaultDisplay.GetMetrics(dm);
			return new ScreenProperties
			{
				Density = dm.Density,
				PixelWidth = dm.WidthPixels,
				PixelHeight = dm.HeightPixels,
			};
#elif __IOS__
			var scale = UIScreen.MainScreen.Scale;
			var scaledSize = UIScreen.MainScreen.Bounds;
			return new ScreenProperties
			{
				Density = scale,
				PixelWidth = (int)(scaledSize.Width * scale),
				PixelHeight = (int)(scaledSize.Height * scale),
			};
#else
			throw new PlatformNotSupportedException();
#endif
		}
	}
}
