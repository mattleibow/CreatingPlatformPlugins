using Windows.Foundation.Metadata;
using Windows.Graphics.Display;

namespace Library
{
	internal class ScreenImplementation : IScreen
	{
		public ScreenProperties GetProperties()
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
