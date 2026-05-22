using System;
using TMPro;
using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Tweening;

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

        private void Awake()
        {
            OnShowEvent += InternalShow;
            OnHideEvent += InternalHide;
        }

        private void OnDestroy()
        {
            OnShowEvent -= InternalShow;
            OnHideEvent -= InternalHide;
        }
        
        public static void Show(string textNoti) => OnShowEvent?.Invoke(textNoti);
        public static void Hide() => OnHideEvent?.Invoke();

        private void InternalShow(string _textNoti)
        {
            if (!gameConfig.EnableNotificationInGame) return;
            if (isShow) return;
            isShow = true;
            gameObject.SetActive(true);
            textNoti.text = _textNoti;
            Tween.Create(posYHide, posYShow, timeMove).WithEase(Ease.OutBack).WithOnComplete(() =>
            {
                App.Delay(gameConfig.TimeDelayHideNotificationInGame, InternalHide);
            }).BindToAnchoredPositionY(container);
        }

        private void InternalHide()
        {
            if (!gameConfig.EnableNotificationInGame) return;
            if (!isShow) return;
            Tween.Create(posYShow, posYHide, timeMove).WithEase(Ease.InBack).WithOnComplete(() =>
            {
                    isShow = false;
                    gameObject.SetActive(false);
            }).BindToAnchoredPositionY(container);
        }
    }
}