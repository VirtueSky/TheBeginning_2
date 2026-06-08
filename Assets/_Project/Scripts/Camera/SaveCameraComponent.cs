using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SaveCameraComponent : MonoBehaviour
{
    [SerializeField] private CameraType cameraType;
    [SerializeField] private Camera camera;

    private void OnEnable()
    {
        if (camera == null || CameraManager.Instance == null) return;
        CameraManager.Instance.AddCamera(cameraType, camera);
    }

    private void OnDisable()
    {
        if (CameraManager.Instance == null) return;
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
