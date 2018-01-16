using System;

using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI.Popups;

namespace Library
{
	partial class Screen
	{
		static partial void GetPropertiesInternal(ref ScreenProperties properties)
		{
			var di = DisplayInformation.GetForCurrentView();
			properties.Density = di.RawPixelsPerViewPixel;
			if (ApiInformation.IsPropertyPresent(typeof(DisplayInformation).FullName, nameof(DisplayInformation.ScreenWidthInRawPixels)))
			{
				properties.PixelWidth = (int)di.ScreenWidthInRawPixels;
				properties.PixelHeight = (int)di.ScreenHeightInRawPixels;
			}
		}

		static async partial void ShowPropertiesInternal(string message)
		{
			var dialog = new MessageDialog(message, "Screen Properties");
			dialog.Commands.Add(new UICommand("OK"));
			await dialog.ShowAsync();
		}
	}
}
