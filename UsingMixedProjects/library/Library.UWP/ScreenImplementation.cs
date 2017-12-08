using Windows.Foundation.Metadata;
using Windows.Graphics.Display;

namespace Library
{
	internal static class ScreenImplementation
	{
		public static ScreenProperties GetProperties()
		{
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
		}
	}
}
