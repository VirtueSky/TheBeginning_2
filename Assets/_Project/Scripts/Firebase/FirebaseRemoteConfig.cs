using VirtueSky.Core;
using VirtueSky.Inspector;

[EditorIcon("icon_controller"), HideMonoScript]
public class FirebaseRemoteConfig : Singleton<FirebaseRemoteConfig>
{
    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;
}