using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VirtueSky.Audio;
using VirtueSky.Core;
using VirtueSky.Vibration;

namespace Base.UI
{
    public class Switcher : BaseMono
    {
        [Header("Datas")] public SwitchState switchState = SwitchState.Idle;
        public bool isOn;
        [Header("Components")] public SettingType SettingType;
        public Sprite on;
        public Sprite off;
        public Image switchBar;
        public Transform offPos;
        public Transform onPos;
        public TextMeshProUGUI switchText;

        [Header("Config attribute")] [Range(0.1f, 3f)]
        public float timeSwitching = .5f;


        private void SetupData()
        {
            switch (SettingType)
            {
                case SettingType.BackgroundMusic:
                    isOn = MusicChanged;
                    break;
                case SettingType.SoundFx:
                    isOn = SoundFxChanged;
                    break;
                case SettingType.Vibration:
                    isOn = VibrateChanged;
                    break;
            }
        }

        private void SetupUI()
        {
            //if (SwitchText) SwitchText.text = IsOn ? "On" : "Off";
            if (isOn)
            {
                switchBar.sprite = on;
            }
            else
            {
                switchBar.sprite = off;
            }
        }

        private void Setup()
        {
            SetupData();
            SetupUI();
        }

        private void OnEnable()
        {
            Setup();
            switchBar.transform.position = isOn ? onPos.position : offPos.position;
        }

        public void Switching()
        {
            if (switchState == SwitchState.Moving) return;
            switchState = SwitchState.Moving;
            if (isOn)
            {
                switchBar.transform.DOMove(offPos.position, timeSwitching);
            }
            else
            {
                switchBar.transform.DOMove(onPos.position, timeSwitching);
            }

            DOTween.Sequence().AppendInterval(timeSwitching / 2f).SetEase(Ease.Linear).AppendCallback(
                () =>
                {
                    switch (SettingType)
                    {
                        case SettingType.BackgroundMusic:
                            MusicChanged = !isOn;
                            break;
                        case SettingType.SoundFx:
                            SoundFxChanged = !isOn;
                            break;
                        case SettingType.Vibration:
                            VibrateChanged = !isOn;
                            break;
                    }

                    Setup();
                }).OnComplete(() => { switchState = SwitchState.Idle; });
        }

        private bool MusicChanged
        {
            get => AudioManager.Instance.MusicVolume >= 0.99f;
            set => AudioManager.Instance.MusicVolume = value ? 1 : 0;
        }

        private bool SoundFxChanged
        {
            get => AudioManager.Instance.SfxVolume >= 0.99f;
            set => AudioManager.Instance.SfxVolume = value ? 1 : 0;
        }

        private bool VibrateChanged
        {
            get => Vibration.EnableVibration;
            set => Vibration.EnableVibration = value;
        }
    }

    public enum SettingType
    {
        BackgroundMusic,
        SoundFx,
        Vibration,
    }

    public enum SwitchState
    {
        Idle,
        Moving,
    }
}