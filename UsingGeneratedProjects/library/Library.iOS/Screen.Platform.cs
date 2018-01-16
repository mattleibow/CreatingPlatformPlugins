using UIKit;

namespace Library
{
	partial class Screen
	{
		static partial void GetPropertiesInternal(ref ScreenProperties properties)
		{
			var scale = UIScreen.MainScreen.Scale;
			var scaledSize = UIScreen.MainScreen.Bounds;
			properties.Density = scale;
			properties.PixelWidth = (int)(scaledSize.Width * scale);
			properties.PixelHeight = (int)(scaledSize.Height * scale);
		}

		static partial void ShowPropertiesInternal(string message)
		{
			var alert = new UIAlertView("Screen Properties", message, (IUIAlertViewDelegate)null, "OK");
			alert.Show();
		}
	}
}
