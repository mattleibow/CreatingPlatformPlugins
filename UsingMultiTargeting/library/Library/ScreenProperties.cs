namespace Library
{
	public struct ScreenProperties
	{
		public int PixelWidth { get; internal set; }

		public int PixelHeight { get; internal set; }

		public double Density { get; internal set; }

		public int Width => (int)(PixelWidth / Density);

		public int Height => (int)(PixelHeight / Density);
	}
}
