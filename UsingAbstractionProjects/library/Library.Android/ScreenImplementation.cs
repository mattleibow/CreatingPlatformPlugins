using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Library
{
	internal class ScreenImplementation : IScreen
	{
		public ScreenProperties GetProperties()
		{
			var wm = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			var dm = new DisplayMetrics();
			wm.DefaultDisplay.GetMetrics(dm);
			return new ScreenProperties
			{
				Density = dm.Density,
				PixelWidth = dm.WidthPixels,
				PixelHeight = dm.HeightPixels,
			};
		}
	}
}
