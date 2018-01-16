using System;

namespace Library
{
	public static partial class Screen
	{
		public static void ShowProperties()
		{
			var properties = GetProperties();

			var message =
				$"Density: {properties.Density}" + Environment.NewLine +
				$"Scaled Size: {properties.Width} x {properties.Height}" + Environment.NewLine +
				$"Pixel Size: {properties.PixelWidth} x {properties.PixelHeight}";

			ShowPropertiesInternal(message);
		}

		public static double GetDensity()
		{
			return GetProperties().Density;
		}

		public static ScreenProperties GetProperties()
		{
			var props = new ScreenProperties();
			GetPropertiesInternal(ref props);
			return props;
		}

		static partial void ShowPropertiesInternal(string message);

		static partial void GetPropertiesInternal(ref ScreenProperties properties);
	}
}
