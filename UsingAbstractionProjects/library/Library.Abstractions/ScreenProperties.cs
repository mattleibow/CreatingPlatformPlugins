namespace Library
{
	public struct ScreenProperties
	{
		public int PixelWidth { get; set; }

		public int PixelHeight { get; set; }

		public double Density { get; set; }

		public int Width => (int)(PixelWidth / Density);

		public int Height => (int)(PixelHeight / Density);
	}
}
