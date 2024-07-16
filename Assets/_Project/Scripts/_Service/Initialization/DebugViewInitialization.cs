using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VirtueSky.Inspector;

namespace Base.Services
{
    [HideMonoScript]
    public class DebugViewInitialization : ServiceInitialization
    {
        [SerializeField] private DebugSheet debugViewSheet;
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

        public override void Initialization()
        {
            if (!gameConfig.enableDebugView)
            {
                debugViewSheet.gameObject.SetActive(false);
            }

            debugViewSheet.gameObject.SetActive(true);
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
            initPage.Reload();
        }
    }
}