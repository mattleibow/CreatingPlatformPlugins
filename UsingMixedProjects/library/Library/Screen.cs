namespace Library
{
	public static class Screen
	{
		public static double GetDensity()
		{
			return GetProperties().Density;
		}

		public static ScreenProperties GetProperties()
		{
			return ScreenImplementation.GetProperties();
		}
	}
}
