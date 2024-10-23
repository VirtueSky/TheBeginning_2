namespace VirtueSky.RemoteConfigs
{
	public struct RemoteData
	{
// LEVEL_TURN_ON_INTER_ADS
		public const string KEY_LEVEL_TURN_ON_INTER_ADS = "LEVEL_TURN_ON_INTER_ADS";
		public const int DEFAULT_LEVEL_TURN_ON_INTER_ADS = 5;
		public static int LEVEL_TURN_ON_INTER_ADS => VirtueSky.DataStorage.GameData.Get(KEY_LEVEL_TURN_ON_INTER_ADS, DEFAULT_LEVEL_TURN_ON_INTER_ADS);
// INTER_CAPPING_LEVEL
		public const string KEY_INTER_CAPPING_LEVEL = "INTER_CAPPING_LEVEL";
		public const int DEFAULT_INTER_CAPPING_LEVEL = 2;
		public static int INTER_CAPPING_LEVEL => VirtueSky.DataStorage.GameData.Get(KEY_INTER_CAPPING_LEVEL, DEFAULT_INTER_CAPPING_LEVEL);
// INTER_CAPPING_TIME
		public const string KEY_INTER_CAPPING_TIME = "INTER_CAPPING_TIME";
		public const int DEFAULT_INTER_CAPPING_TIME = 8;
		public static int INTER_CAPPING_TIME => VirtueSky.DataStorage.GameData.Get(KEY_INTER_CAPPING_TIME, DEFAULT_INTER_CAPPING_TIME);
// ON_OFF_INTER
		public const string KEY_ON_OFF_INTER = "ON_OFF_INTER";
		public const bool DEFAULT_ON_OFF_INTER = true;
		public static bool ON_OFF_INTER => VirtueSky.DataStorage.GameData.Get(KEY_ON_OFF_INTER, DEFAULT_ON_OFF_INTER);
// ON_OFF_BANNER
		public const string KEY_ON_OFF_BANNER = "ON_OFF_BANNER";
		public const bool DEFAULT_ON_OFF_BANNER = true;
		public static bool ON_OFF_BANNER => VirtueSky.DataStorage.GameData.Get(KEY_ON_OFF_BANNER, DEFAULT_ON_OFF_BANNER);
// LEVEL_SHOW_RATE_AND_REVIEW
		public const string KEY_LEVEL_SHOW_RATE_AND_REVIEW = "LEVEL_SHOW_RATE_AND_REVIEW";
		public const int DEFAULT_LEVEL_SHOW_RATE_AND_REVIEW = 3;
		public static int LEVEL_SHOW_RATE_AND_REVIEW => VirtueSky.DataStorage.GameData.Get(KEY_LEVEL_SHOW_RATE_AND_REVIEW, DEFAULT_LEVEL_SHOW_RATE_AND_REVIEW);
// ON_OFF_RATE_AND_REVIEW
		public const string KEY_ON_OFF_RATE_AND_REVIEW = "ON_OFF_RATE_AND_REVIEW";
		public const bool DEFAULT_ON_OFF_RATE_AND_REVIEW = true;
		public static bool ON_OFF_RATE_AND_REVIEW => VirtueSky.DataStorage.GameData.Get(KEY_ON_OFF_RATE_AND_REVIEW, DEFAULT_ON_OFF_RATE_AND_REVIEW);
// VERSION_UPDATE
		public const string KEY_VERSION_UPDATE = "VERSION_UPDATE";
		public const string DEFAULT_VERSION_UPDATE = "1.0";
		public static string VERSION_UPDATE => VirtueSky.DataStorage.GameData.Get(KEY_VERSION_UPDATE, DEFAULT_VERSION_UPDATE);
// CONTENT_UPDATE
		public const string KEY_CONTENT_UPDATE = "CONTENT_UPDATE";
		public const string DEFAULT_CONTENT_UPDATE = "Update Content";
		public static string CONTENT_UPDATE => VirtueSky.DataStorage.GameData.Get(KEY_CONTENT_UPDATE, DEFAULT_CONTENT_UPDATE);

	}
}