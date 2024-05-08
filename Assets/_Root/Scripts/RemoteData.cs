namespace VirtueSky.RemoteConfigs
{
	public struct RemoteData
	{
		public const string KEY_RMC_LEVEL_TURN_ON_INTER_ADS = "RMC_LEVEL_TURN_ON_INTER_ADS";
		public static int RMC_LEVEL_TURN_ON_INTER_ADS => VirtueSky.DataStorage.GameData.Get<int>(KEY_RMC_LEVEL_TURN_ON_INTER_ADS);
		public const string KEY_RMC_INTER_CAPPING_LEVEL = "RMC_INTER_CAPPING_LEVEL";
		public static int RMC_INTER_CAPPING_LEVEL => VirtueSky.DataStorage.GameData.Get<int>(KEY_RMC_INTER_CAPPING_LEVEL);
		public const string KEY_RMC_INTER_CAPPING_TIME = "RMC_INTER_CAPPING_TIME";
		public static int RMC_INTER_CAPPING_TIME => VirtueSky.DataStorage.GameData.Get<int>(KEY_RMC_INTER_CAPPING_TIME);
		public const string KEY_RMC_ON_OFF_INTER = "RMC_ON_OFF_INTER";
		public static bool RMC_ON_OFF_INTER => VirtueSky.DataStorage.GameData.Get<bool>(KEY_RMC_ON_OFF_INTER);
		public const string KEY_RMC_ON_OFF_BANNER = "RMC_ON_OFF_BANNER";
		public static bool RMC_ON_OFF_BANNER => VirtueSky.DataStorage.GameData.Get<bool>(KEY_RMC_ON_OFF_BANNER);
		public const string KEY_RMC_LEVEL_SHOW_RATE_AND_REVIEW = "RMC_LEVEL_SHOW_RATE_AND_REVIEW";
		public static int RMC_LEVEL_SHOW_RATE_AND_REVIEW => VirtueSky.DataStorage.GameData.Get<int>(KEY_RMC_LEVEL_SHOW_RATE_AND_REVIEW);
		public const string KEY_RMC_ON_OFF_RATE_AND_REVIEW = "RMC_ON_OFF_RATE_AND_REVIEW";
		public static bool RMC_ON_OFF_RATE_AND_REVIEW => VirtueSky.DataStorage.GameData.Get<bool>(KEY_RMC_ON_OFF_RATE_AND_REVIEW);
		public const string KEY_RMC_VERSION_UPDATE = "RMC_VERSION_UPDATE";
		public static string RMC_VERSION_UPDATE => VirtueSky.DataStorage.GameData.Get<string>(KEY_RMC_VERSION_UPDATE);
		public const string KEY_RMC_CONTENT_UPDATE = "RMC_CONTENT_UPDATE";
		public static string RMC_CONTENT_UPDATE => VirtueSky.DataStorage.GameData.Get<string>(KEY_RMC_CONTENT_UPDATE);
	}
}