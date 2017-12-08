using System;
using Xamarin.Forms;

using Library;

namespace SampleApp
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			var properties = Screen.GetProperties();

			await System.Threading.Tasks.Task.Delay(2000);

			label.Text =
				$"Density: {properties.Density}" + Environment.NewLine +
				$"Scaled Size: {properties.Width} x {properties.Height}" + Environment.NewLine +
				$"Pixel Size: {properties.PixelWidth} x {properties.PixelHeight}";
		}
	}
}
