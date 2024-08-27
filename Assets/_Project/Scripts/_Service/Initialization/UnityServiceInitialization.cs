using Base.Services;
using Unity.Services.Core;
using VirtueSky.Inspector;

[HideMonoScript]
public class UnityServiceInitialization : ServiceInitialization
{
    public override void Initialization()
    {
        UnityServices.InitializeAsync();
    }
}