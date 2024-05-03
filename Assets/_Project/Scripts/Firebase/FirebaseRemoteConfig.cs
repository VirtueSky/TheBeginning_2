using VirtueSky.Core;

public class FirebaseRemoteConfig : Singleton<FirebaseRemoteConfig>
{
    private bool isInitialized = false;
    public bool IsInitialized => isInitialized;
}