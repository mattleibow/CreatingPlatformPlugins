using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Library
{
	partial class Screen
	{
		private static LifecycleCallbacks callbacks;

		public static void Init(Application app)
		{
			if (callbacks == null)
			{
				callbacks = new LifecycleCallbacks();
				app.RegisterActivityLifecycleCallbacks(callbacks);
			}
		}

		static partial void GetPropertiesInternal(ref ScreenProperties properties)
		{
			var wm = Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
			var dm = new DisplayMetrics();
			wm.DefaultDisplay.GetMetrics(dm);
			properties.Density = dm.Density;
			properties.PixelWidth = dm.WidthPixels;
			properties.PixelHeight = dm.HeightPixels;
		}

		static partial void ShowPropertiesInternal(string message)
		{
			var activity = callbacks?.CurrentActivity;
			if (activity == null)
			{
				Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
			}
			else
			{
				var dialog = new AlertDialog.Builder(activity)
					.SetTitle("Screen Properties")
					.SetMessage(message)
					.SetNeutralButton("OK", delegate { })
					.Create();
				dialog.Show();
			}
		}

		private class LifecycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
		{
			public Activity CurrentActivity { get; private set; }

			public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
			{
				CurrentActivity = activity;
			}

			public void OnActivityStarted(Activity activity)
			{
				CurrentActivity = activity;
			}

			public void OnActivityResumed(Activity activity)
			{
				CurrentActivity = activity;
			}

			public void OnActivityPaused(Activity activity)
			{
				CurrentActivity = null;
			}

			public void OnActivityStopped(Activity activity)
			{
			}

			public void OnActivityDestroyed(Activity activity)
			{
			}

			public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
			{
			}
		}
	}
}
