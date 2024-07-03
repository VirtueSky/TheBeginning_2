using UnityEngine;
using VirtueSky.Audio;
using VirtueSky.Core;

public class SoundClickButton : MonoBehaviour
{
    [SerializeField] private SoundData sfxClickButton;

    private void Awake()
    {
        GlobalStatic.OnClickButtonEvent += PlaySoundClickButton;
    }

    private void OnDestroy()
    {
        GlobalStatic.OnClickButtonEvent -= PlaySoundClickButton;
    }

    void PlaySoundClickButton()
    {
        sfxClickButton.PlaySfx();
    }
}