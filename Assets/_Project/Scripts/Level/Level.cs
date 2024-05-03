using UnityEditor;
using UnityEngine;
using VirtueSky.Inspector;
using VirtueSky.Misc;

public class Level : MonoBehaviour
{
    [ReadOnly] public int BonusMoney;

    private bool _isFingerDown;
    private bool _isFingerDrag;

#if UNITY_EDITOR
    [Button]
    private void StartLevel()
    {
        UserData.CurrentLevel = gameObject.name.GetNumberInAString();

        EditorApplication.isPlaying = true;
    }
#endif


    private void Start()
    {
        Observer.WinLevel += OnWin;
        Observer.LoseLevel += OnLose;
    }

    private void OnDestroy()
    {
        Observer.WinLevel -= OnWin;
        Observer.LoseLevel -= OnLose;
    }

    public void OnWin(Level level)
    {
    }

    public void OnLose(Level level)
    {
    }
}