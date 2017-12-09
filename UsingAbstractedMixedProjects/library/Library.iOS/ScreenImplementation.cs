using UIKit;

namespace Library
{
	internal class ScreenImplementation : IScreen
	{
		public ScreenProperties GetProperties()
		{
			var scale = UIScreen.MainScreen.Scale;
			var scaledSize = UIScreen.MainScreen.Bounds;
			return new ScreenProperties
			{
				Density = scale,
				PixelWidth = (int)(scaledSize.Width * scale),
				PixelHeight = (int)(scaledSize.Height * scale),
			};
		}
	}
}
