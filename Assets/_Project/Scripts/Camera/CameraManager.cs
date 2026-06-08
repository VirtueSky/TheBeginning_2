using UnityEngine;
using VirtueSky.DataType;
using VirtueSky.Pattern;

public class CameraManager : Singleton<CameraManager>
{
    private DictionaryCustom<CameraType, Camera> _cameras = new DictionaryCustom<CameraType, Camera>();

    public void AddCamera(CameraType type, Camera camera)
    {
        if (!_cameras.ContainsKey(type))
        {
            _cameras.Add(type, camera);
        }
    }
    public void RemoveCamera(CameraType type)
    {
        if (_cameras.ContainsKey(type))
        {
            _cameras.Remove(type);
        }
    }

    public Camera GetCamera(CameraType type)
    {
        if (_cameras.ContainsKey(type))
        {
            return _cameras[type];
        }

        return null;
    }
}

public enum CameraType
{
    Loading,
    UI_Service,
    UI_Gameplay,
    Gameplay
}