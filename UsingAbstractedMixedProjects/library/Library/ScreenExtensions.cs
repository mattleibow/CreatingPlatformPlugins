namespace Library
{
	public static class ScreenExtensions
	{
		public static double GetDensity(this IScreen screen)
		{
			return screen.GetProperties().Density;
		}
	}
}
