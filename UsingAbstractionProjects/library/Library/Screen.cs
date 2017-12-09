using System;

namespace Library
{
	public static class Screen
	{
#if NETSTANDARD1_0
		public static IScreen Instance => throw new PlatformNotSupportedException();
#else
		private static Lazy<IScreen> instance = new Lazy<IScreen>(() => new ScreenImplementation());

		public static IScreen Instance => instance.Value;
#endif
	}
}
