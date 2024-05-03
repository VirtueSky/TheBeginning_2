using VirtueSky.Vibration;

namespace Base.Services
{
    public class VibrationInitialization : ServiceInitialization
    {
        public override void Initialization()
        {
            Vibration.Init();
        }
    }
}