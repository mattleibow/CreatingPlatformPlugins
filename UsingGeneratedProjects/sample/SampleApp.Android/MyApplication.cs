using Android.App;
using Android.Runtime;
using Library;
using System;

namespace SampleApp.Droid
{
	[Application]
	public class MyApplication : Application
	{
		public MyApplication(IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();

			Screen.Init(this);
		}
	}
}
