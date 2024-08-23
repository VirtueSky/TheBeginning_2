using UnityEngine;
using UnityEngine.AddressableAssets;
using VirtueSky.Core;
using Cysharp.Threading.Tasks;

public class EntryGame : BaseMono
{
    [SerializeField] private AssetReference launcherScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await Addressables.LoadSceneAsync(launcherScene);
        Destroy(gameObject);
    }
}