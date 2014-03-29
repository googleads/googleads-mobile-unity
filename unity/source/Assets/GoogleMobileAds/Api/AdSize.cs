namespace GoogleMobileAds.Api {
    public class AdSize {
        private bool isSmartBanner;
        private int width;
        private int height;

        public static readonly AdSize Banner = new AdSize(320, 50);
        public static readonly AdSize MediumRectangle = new AdSize(300, 250);
        public static readonly AdSize IABBanner = new AdSize(468, 60);
        public static readonly AdSize Leaderboard = new AdSize(728, 90);
        public static readonly AdSize SmartBanner = new AdSize(true);

        public AdSize(int width, int height) {
            isSmartBanner = false;
            this.width = width;
            this.height = height;
        }

        private AdSize(bool isSmartBanner) {
            this.isSmartBanner = isSmartBanner;
            this.width = 0;
            this.height = 0;
        }

        public int Width
        {
            get
            {
                return width;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
        }

        public bool IsSmartBanner
        {
            get
            {
                return isSmartBanner;
            }
        }
    }
}
