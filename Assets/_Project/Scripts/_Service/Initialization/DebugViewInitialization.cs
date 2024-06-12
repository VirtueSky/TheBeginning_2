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

        public override void Initialization()
        {
            if (!gameConfig.enableDebugView)
            {
                debugViewSheet.gameObject.SetActive(false);
            }

            debugViewSheet.gameObject.SetActive(true);
            var initPage = debugViewSheet.GetOrCreateInitialPage("TheBeginning2 Debug");
            // Game Page
            initPage.AddPageLinkButton<GameDebugPage>("Game Debug", icon: DebugViewStatic.IconToolDebug, onLoad:
                debugView => { debugView.page.Init(itemConfig); });
            // Ads Page
            initPage.AddPageLinkButton<AdsDebugPage>("Ads Debug", icon: DebugViewStatic.IconAdsDebug);
            // Level Page
            initPage.AddPageLinkButton<LevelDebugPage>("Level Debug", icon: DebugViewStatic.IconLevelDebug);
            initPage.Reload();
        }
    }
}