using UnityEngine;
using VirtueSky.Inspector;

public class ConfigController : SingletonDontDestroy<ConfigController>
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private SoundConfig soundConfig;
    [SerializeField] private DailyRewardConfig dailyRewardConfig;
    [SerializeField] private CountryConfig countryConfig;
    [SerializeField] private ItemConfig itemConfig;

    public static GameConfig Game;
    public static SoundConfig Sound;
    public static DailyRewardConfig DailyRewardConfig;
    public static CountryConfig CountryConfig;
    public static ItemConfig ItemConfig;

    protected override void Awake()
    {
        base.Awake();
        Game = gameConfig;
        Sound = soundConfig;
        DailyRewardConfig = dailyRewardConfig;
        CountryConfig = countryConfig;
        ItemConfig = itemConfig;
    }

    public void Initialize()
    {
        ItemConfig.Initialize();
    }
#if UNITY_EDITOR
    [Button]
    private void Load()
    {
        string path = "Assets/_Project/Config/";
        gameConfig = GetFile.GetConfigFromFolder<GameConfig>(path);
        soundConfig = GetFile.GetConfigFromFolder<SoundConfig>(path);
        dailyRewardConfig = GetFile.GetConfigFromFolder<DailyRewardConfig>(path);
        countryConfig = GetFile.GetConfigFromFolder<CountryConfig>(path);
        itemConfig = GetFile.GetConfigFromFolder<ItemConfig>(path);
    }
#endif
}