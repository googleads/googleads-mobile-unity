namespace GoogleMobileAds.Api
{
	// The margin of the ad on the screen.
	public struct AdMargin
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public AdMargin(int left, int top, int right, int bottom)
		{
			this.Left = left;
			this.Top = top;
			this.Right = right;
			this.Bottom = bottom;
		}
	}
}
