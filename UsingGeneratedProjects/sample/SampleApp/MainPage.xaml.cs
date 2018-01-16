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

		private void OnShowScreenProperties(object sender, EventArgs e)
		{
			Screen.ShowProperties();
		}
	}
}
