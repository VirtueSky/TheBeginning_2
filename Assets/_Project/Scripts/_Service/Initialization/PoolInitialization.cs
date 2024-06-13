using VirtueSky.Inspector;
using VirtueSky.ObjectPooling;

namespace Base.Services
{
    [HideMonoScript]
    public class PoolInitialization : ServiceInitialization
    {
        public override void Initialization()
        {
            Pool.InitPool();
        }
    }
}