using UnityEngine;

public class SaveCameraComponent : MonoBehaviour
{
    [SerializeField] private CameraType cameraType;
    [SerializeField] private Camera camera;

    private void Awake()
    {
        CameraManager.Instance.AddCamera(cameraType, camera);
    }
    private void OnDestroy()
    {
        CameraManager.Instance.RemoveCamera(cameraType);
    }
#if UNITY_EDITOR
    private void Reset()
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }
    }
#endif
}
