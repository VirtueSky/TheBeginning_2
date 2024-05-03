using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Inspector;

namespace Base.Services
{
    [HideMonoScript]
    public class RuntimeInitialization : BaseMono
    {
        [SerializeField] private ServiceInitialization[] serviceInitializations;
        private void Start()
        {
            foreach (var serviceInitialization in serviceInitializations)
            {
                serviceInitialization.Initialization();
            }
        }
#if UNITY_EDITOR
        [Button]
        void GetServiceInitialization()
        {
            serviceInitializations = FindObjectsByType<ServiceInitialization>(FindObjectsSortMode.None);
        }
#endif
    }
}