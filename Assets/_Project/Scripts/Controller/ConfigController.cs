using Pancake;
using UnityEngine;

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
        ItemConfig.InitItem();
    }
#if UNITY_EDITOR
    [Button]
    private void Load()
    {
        gameConfig = LoadConfig.GetConfig<GameConfig>("Assets/_Project/Config/");
        soundConfig = LoadConfig.GetConfig<SoundConfig>("Assets/_Project/Config/");
        dailyRewardConfig = LoadConfig.GetConfig<DailyRewardConfig>("Assets/_Project/Config/");
        countryConfig = LoadConfig.GetConfig<CountryConfig>("Assets/_Project/Config/");
        itemConfig = LoadConfig.GetConfig<ItemConfig>("Assets/_Project/Config/");
    }
#endif
}