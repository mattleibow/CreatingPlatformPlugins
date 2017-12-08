using System;

namespace Library
{
	internal static class ScreenImplementation
	{
		public static ScreenProperties GetProperties()
		{
			throw new PlatformNotSupportedException();
		}
	}
}
