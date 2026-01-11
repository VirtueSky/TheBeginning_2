using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EntryGame : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await Addressables.LoadSceneAsync(Constant.LAUNCHER_SCENE);
        Destroy(gameObject);
    }
    
}
