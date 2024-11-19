using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Inspector;

namespace Base.Services
{
    [EditorIcon("icon_controller"), HideMonoScript]
    public class RuntimeInitialization : BaseMono
    {
        [SerializeField] private ServiceInitialization[] serviceInitializations;
        private void Awake()
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
            serviceInitializations = GetComponents<ServiceInitialization>();
        }
#endif
    }
}