namespace VirtueSky.RemoteConfigs
{
	public struct RemoteData
	{
		public const string KEY_RMC_LEVEL_TURN_ON_INTER_ADS = "RMC_LEVEL_TURN_ON_INTER_ADS";
		public const int DEFAULT_RMC_LEVEL_TURN_ON_INTER_ADS = 5;
		public static int RMC_LEVEL_TURN_ON_INTER_ADS => VirtueSky.DataStorage.GameData.Get(KEY_RMC_LEVEL_TURN_ON_INTER_ADS, DEFAULT_RMC_LEVEL_TURN_ON_INTER_ADS);
		public const string KEY_RMC_INTER_CAPPING_LEVEL = "RMC_INTER_CAPPING_LEVEL";
		public const int DEFAULT_RMC_INTER_CAPPING_LEVEL = 2;
		public static int RMC_INTER_CAPPING_LEVEL => VirtueSky.DataStorage.GameData.Get(KEY_RMC_INTER_CAPPING_LEVEL, DEFAULT_RMC_INTER_CAPPING_LEVEL);
		public const string KEY_RMC_INTER_CAPPING_TIME = "RMC_INTER_CAPPING_TIME";
		public const int DEFAULT_RMC_INTER_CAPPING_TIME = 8;
		public static int RMC_INTER_CAPPING_TIME => VirtueSky.DataStorage.GameData.Get(KEY_RMC_INTER_CAPPING_TIME, DEFAULT_RMC_INTER_CAPPING_TIME);
		public const string KEY_RMC_ON_OFF_INTER = "RMC_ON_OFF_INTER";
		public const bool DEFAULT_RMC_ON_OFF_INTER = true;
		public static bool RMC_ON_OFF_INTER => VirtueSky.DataStorage.GameData.Get(KEY_RMC_ON_OFF_INTER, DEFAULT_RMC_ON_OFF_INTER);
		public const string KEY_RMC_ON_OFF_BANNER = "RMC_ON_OFF_BANNER";
		public const bool DEFAULT_RMC_ON_OFF_BANNER = true;
		public static bool RMC_ON_OFF_BANNER => VirtueSky.DataStorage.GameData.Get(KEY_RMC_ON_OFF_BANNER, DEFAULT_RMC_ON_OFF_BANNER);
		public const string KEY_RMC_LEVEL_SHOW_RATE_AND_REVIEW = "RMC_LEVEL_SHOW_RATE_AND_REVIEW";
		public const int DEFAULT_RMC_LEVEL_SHOW_RATE_AND_REVIEW = 3;
		public static int RMC_LEVEL_SHOW_RATE_AND_REVIEW => VirtueSky.DataStorage.GameData.Get(KEY_RMC_LEVEL_SHOW_RATE_AND_REVIEW, DEFAULT_RMC_LEVEL_SHOW_RATE_AND_REVIEW);
		public const string KEY_RMC_ON_OFF_RATE_AND_REVIEW = "RMC_ON_OFF_RATE_AND_REVIEW";
		public const bool DEFAULT_RMC_ON_OFF_RATE_AND_REVIEW = true;
		public static bool RMC_ON_OFF_RATE_AND_REVIEW => VirtueSky.DataStorage.GameData.Get(KEY_RMC_ON_OFF_RATE_AND_REVIEW, DEFAULT_RMC_ON_OFF_RATE_AND_REVIEW);
		public const string KEY_RMC_VERSION_UPDATE = "RMC_VERSION_UPDATE";
		public const string DEFAULT_RMC_VERSION_UPDATE = "1.0";
		public static string RMC_VERSION_UPDATE => VirtueSky.DataStorage.GameData.Get(KEY_RMC_VERSION_UPDATE, DEFAULT_RMC_VERSION_UPDATE);
		public const string KEY_RMC_CONTENT_UPDATE = "RMC_CONTENT_UPDATE";
		public const string DEFAULT_RMC_CONTENT_UPDATE = "Update Content";
		public static string RMC_CONTENT_UPDATE => VirtueSky.DataStorage.GameData.Get(KEY_RMC_CONTENT_UPDATE, DEFAULT_RMC_CONTENT_UPDATE);
	}
}