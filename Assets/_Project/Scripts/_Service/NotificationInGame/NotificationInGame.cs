using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using VirtueSky.Core;

namespace Base.Services
{
    public class NotificationInGame : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textNoti;
        [SerializeField] private RectTransform container;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private float posYShow = -125;
        [SerializeField] private float posYHide = 125;
        [SerializeField] private float timeMove = .5f;
        private bool isShow = false;
        private static event Action<string> OnShowEvent;
        private static event Action OnHideEvent;

        private void OnEnable()
        {
            OnShowEvent += InternalShow;
            OnHideEvent += InternalHide;
        }

        private void OnDisable()
        {
            OnShowEvent -= InternalShow;
            OnHideEvent -= InternalHide;
        }

        public static void Show(string textNoti) => OnShowEvent?.Invoke(textNoti);
        public static void Hide() => OnHideEvent?.Invoke();

        private void InternalShow(string _textNoti)
        {
            if (!gameConfig.enableNotificationInGame) return;
            if (isShow) return;
            isShow = true;
            gameObject.SetActive(true);
            textNoti.text = _textNoti;
            Tween.UIAnchoredPositionY(container, posYShow, timeMove, Ease.OutBack).OnComplete(() =>
            {
                App.Delay(gameConfig.timeDelayHideNotificationInGame, () => { InternalHide(); });
            });
        }

        private void InternalHide()
        {
            if (!gameConfig.enableNotificationInGame) return;
            if (!isShow) return;
            Tween.UIAnchoredPositionY(container, posYHide, timeMove, Ease.InBack).OnComplete(() =>
            {
                isShow = false;
                gameObject.SetActive(false);
            });
        }
    }
}