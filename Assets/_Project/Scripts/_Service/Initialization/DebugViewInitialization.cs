using Base.Data;
using Consolation;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Pattern;

namespace Base.Services
{
    [HideMonoScript]
    public class DebugViewInitialization : ServiceInitialization
    {
        [SerializeField] private DebugSheet debugViewSheet;
        [SerializeField] private ConsoleInGame consoleInGamePrefab;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ItemConfig itemConfig;
        [HeaderLine("Icon"), SerializeField] private Sprite iconTool;
        [SerializeField] private Sprite iconAds;
        [SerializeField] private Sprite iconLevel;
        [SerializeField] private Sprite iconWin;
        [SerializeField] private Sprite iconLose;
        [SerializeField] private Sprite iconNext;
        [SerializeField] private Sprite iconBack;
        [SerializeField] private Sprite iconToggle;
        [SerializeField] private Sprite iconInput;
        [SerializeField] private Sprite iconOke;
        [SerializeField] private Sprite iconAnalysis;
        [SerializeField] private Sprite iconFps;
        [SerializeField] private Sprite iconAudio;
        [SerializeField] private Sprite iconRam;
        [SerializeField] private Sprite iconAdvanced;
        [SerializeField] private Sprite iconCoinDebug;
        [SerializeField] private Sprite iconOutfitDebug;
        [SerializeField] private Sprite iconConsoleLog;
        [SerializeField] private Sprite iconSlider;

        private bool isSetup;

        public override void Initialization()
        {
            EventName.DebugViewChanged.AddListener<bool>(OnDebugViewChanged);
            OnDebugViewChanged(UserData.EnableDebugView);
        }

        private void OnDestroy()
        {
            EventName.DebugViewChanged.RemoveListener<bool>(OnDebugViewChanged);
        }

        void OnDebugViewChanged(bool isEnable)
        {
            debugViewSheet.gameObject.SetActive(isEnable);
            if (isEnable && !isSetup) SetupDebugView();
        }

        void SetupDebugView()
        {
            isSetup = true;
            SetupConsoleInGame();
            var initPage = debugViewSheet.GetOrCreateInitialPage("TheBeginning2 Debug");
            // Game Page
            initPage.AddPageLinkButton<GameDebugPage>("Game Debug", icon: iconTool, onLoad:
                debugView =>
                {
                    debugView.page.Init(itemConfig, iconInput, iconOke, iconToggle, iconCoinDebug, iconOutfitDebug);
                });
            // Ads Page
            initPage.AddPageLinkButton<AdsDebugPage>("Ads Debug", icon: iconAds,
                onLoad: debugView => { debugView.page.Init(iconToggle); });
            // Level Page
            initPage.AddPageLinkButton<LevelDebugPage>("Level Debug", icon: iconLevel,
                onLoad: debugView =>
                {
                    debugView.page.Init(iconNext, iconBack, iconWin, iconLose, iconInput, iconOke);
                });
            // Add system analysis page
            initPage.AddPageLinkButton<SystemAnalysisDebugPage>("System analysis", icon: iconAnalysis, onLoad:
                debugView => { debugView.page.Init(iconFps, iconRam, iconAudio, iconAdvanced); });
            // Add Console pag
            initPage.AddPageLinkButton<ConsoleLogDebugPage>("Console Log", icon: iconConsoleLog,
                onLoad: debugView => { debugView.page.Init(iconToggle, iconInput, iconOke, iconSlider); });
            initPage.Reload();
        }

        void SetupConsoleInGame()
        {
            Instantiate(consoleInGamePrefab);
        }
    }
}