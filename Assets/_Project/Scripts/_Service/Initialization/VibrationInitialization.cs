using VirtueSky.Inspector;
using VirtueSky.Vibration;

namespace Base.Services
{
    [HideMonoScript]
    public class VibrationInitialization : ServiceInitialization
    {
        public override void Initialization()
        {
            Vibration.Init();
        }
    }
}