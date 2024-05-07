using PrimeTween;
using TMPro;
using UnityEngine;
using VirtueSky.Core;

namespace Base.Services
{
    public class NotificationInGame : Singleton<NotificationInGame>
    {
        [SerializeField] private TextMeshProUGUI textNoti;
        [SerializeField] private RectTransform container;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private float posYShow = -125;
        [SerializeField] private float posYHide = 125;
        [SerializeField] private float timeMove = .5f;
        private bool isShow = false;


        public void Show(string _textNoti)
        {
            if (!gameConfig.enableNotificationInGame) return;
            if (isShow) return;
            isShow = true;
            gameObject.SetActive(true);
            textNoti.text = _textNoti;
            Tween.UIAnchoredPositionY(container, posYShow, timeMove, Ease.OutBack).OnComplete(() =>
            {
                App.Delay(gameConfig.timeDelayHideNotificationInGame, () => { Hide(); });
            });
        }

        public void Hide()
        {
            if (!gameConfig.enableNotificationInGame) return;
            if (!isShow) return;
            isShow = false;
            Tween.UIAnchoredPositionY(container, posYHide, timeMove, Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}