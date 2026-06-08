using UnityEngine;
using UnityEngine.Rendering.Universal;
using VirtueSky.DataType;
using VirtueSky.Pattern;

public class CameraManager : Singleton<CameraManager>
{
    // Thứ tự ưu tiên chọn Base camera (từ cao xuống thấp).
    private static readonly CameraType[] BasePriority =
    {
        CameraType.Loading,
        CameraType.GameplayLevel,
        CameraType.UI_Gameplay,
    };

    // Thứ tự overlay xếp vào cameraStack (đầu list = đáy, cuối list = trên cùng).
    private static readonly CameraType[] OverlayOrder =
    {
        CameraType.Loading,
        CameraType.GameplayLevel,
        CameraType.UI_Gameplay,
        CameraType.UI_Service,
    };

    private DictionaryCustom<CameraType, Camera> _cameras = new DictionaryCustom<CameraType, Camera>();

    public void AddCamera(CameraType type, Camera camera)
    {
        if (camera == null) return;
        if (_cameras.ContainsKey(type)) return;

        _cameras.Add(type, camera);
        RebuildStack();
    }

    public void RemoveCamera(CameraType type)
    {
        if (!_cameras.ContainsKey(type)) return;

        _cameras.Remove(type);
        RebuildStack();
    }

    public Camera GetCamera(CameraType type)
    {
        return _cameras.ContainsKey(type) ? _cameras[type] : null;
    }

    private void RebuildStack()
    {
        var baseCam = ResolveBaseCamera();
        if (baseCam == null) return;

        var baseUrp = baseCam.GetUniversalAdditionalCameraData();
        if (baseUrp == null) return;

        baseUrp.renderType = CameraRenderType.Base;
        baseUrp.cameraStack.Clear();

        foreach (var type in OverlayOrder)
        {
            if (!_cameras.ContainsKey(type)) continue;
            var cam = _cameras[type];
            if (cam == null || cam == baseCam) continue;

            var urp = cam.GetUniversalAdditionalCameraData();
            if (urp == null) continue;

            urp.renderType = CameraRenderType.Overlay;
            baseUrp.cameraStack.Add(cam);
        }
    }

    private Camera ResolveBaseCamera()
    {
        foreach (var type in BasePriority)
        {
            if (!_cameras.ContainsKey(type)) continue;
            var cam = _cameras[type];
            if (cam != null && cam.gameObject.activeInHierarchy) return cam;
        }

        return null;
    }
}

public enum CameraType
{
    Loading,
    UI_Service,
    UI_Gameplay,
    GameplayLevel
}