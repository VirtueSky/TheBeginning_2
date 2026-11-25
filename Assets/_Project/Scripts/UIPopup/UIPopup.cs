using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Inspector;
using VirtueSky.Tweening;
using VirtueSky.Utils;

namespace Base.UI
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup))]
    public class UIPopup : MonoBehaviour
    {
        [TitleColor("Environment", CustomColor.Violet, CustomColor.Lime)]
        public CanvasGroup canvasGroup;

        public Canvas canvas;
        public bool useAnimation;

        [ShowIf(nameof(useAnimation))] [SerializeField]
        protected GameObject background;

        [ShowIf(nameof(useAnimation))] [SerializeField]
        protected GameObject container;

        [TitleColor("Show Animation", CustomColor.Aqua, CustomColor.Beige)] [ShowIf(nameof(useAnimation))]
        public bool useShowAnimation;

        [ShowIf(nameof(useShowAnimation))] public ShowAnimationType showAnimationType;
        [ShowIf(nameof(useShowAnimation))] public float durationShowPopup = .5f;

        [ShowIf(nameof(ConditionShowMove))] [SerializeField]
        private MovePopupType showMovePopup;

        [ShowIf(nameof(ConditionShowMove))] [SerializeField]
        private float offsetShowMove;

        [ShowIf(nameof(ConditionShowOutBack))] [SerializeField]
        private Vector3 scaleFromShow = new Vector3(.5f, .5f, .5f);

        [ShowIf(nameof(ConditionShowFlip))] [SerializeField]
        private Vector3 eulerAngleShowFrom = new Vector3(0, 180, 0);

        [ShowIf(nameof(showAnimationType), ShowAnimationType.OutBackFromPoint)] [SerializeField]
        private Vector3 pointShowPos;

        [TitleColor("Hide Animation", CustomColor.Burlywood, CustomColor.Gold)] [ShowIf(nameof(useAnimation))]
        public bool useHideAnimation;

        [ShowIf(nameof(useHideAnimation))] public HideAnimationType hideAnimationType;
        [ShowIf(nameof(useHideAnimation))] public float durationHidePopup = .5f;

        [ShowIf(nameof(ConditionHideMove))] [SerializeField]
        private MovePopupType hideMovePopup;

        [ShowIf(nameof(ConditionHideMove))] [SerializeField]
        private float offsetHideMove;

        [ShowIf(nameof(ConditionHideInBack))] [SerializeField]
        private Vector3 scaleFromHide = new Vector3(0, 0, 0);

        [ShowIf(nameof(hideAnimationType), HideAnimationType.InBackToPoint)] [SerializeField]
        private Vector3 pointHidePos;

        private TweenHandle tween;
        private TweenHandle subTween;

        private bool isCachedDefault;
        private Vector3 defaultLocalPos;
        private Vector3 defaultScale;
        private Vector3 defaultLocalEuler;
        private bool isHiding;

        private bool ConditionShowMove =>
            useAnimation && useShowAnimation && showAnimationType == ShowAnimationType.Move;

        private bool ConditionHideMove =>
            useAnimation && useHideAnimation && hideAnimationType == HideAnimationType.Move;

        private bool ConditionShowOutBack =>
            useAnimation && useShowAnimation && showAnimationType == ShowAnimationType.OutBack;

        private bool ConditionHideInBack =>
            useAnimation && useHideAnimation && hideAnimationType == HideAnimationType.InBack;

        private bool ConditionShowFlip =>
            useAnimation && useShowAnimation && showAnimationType == ShowAnimationType.Flip;

        private bool CanAnimateShow => useAnimation && useShowAnimation && container != null;
        private bool CanAnimateHide => useAnimation && useHideAnimation && container != null;

        public virtual void Show()
        {
            CacheDefault();
            StopTweens();
            isHiding = false;
            ResetState();
            OnBeforeShow();
            if (!CanAnimateShow)
            {
                gameObject.SetActive(true);
                OnAfterShow();
                return;
            }

            var t = container.transform;
            switch (showAnimationType)
            {
                case ShowAnimationType.OutBack:
                    t.localScale = scaleFromShow;
                    gameObject.SetActive(true);
                    tween = Tween.Create(scaleFromShow, defaultScale, durationShowPopup).WithEase(Ease.OutBack).WithOnComplete(OnAfterShow).BindToLocalScale(t);
                    break;
                case ShowAnimationType.Flip:
                    t.localEulerAngles = eulerAngleShowFrom;
                    gameObject.SetActive(true);
                    tween = Tween.Create(eulerAngleShowFrom, defaultLocalEuler, durationShowPopup).WithEase(Ease.OutBack).WithOnComplete(OnAfterShow).BindToLocalEulerAngles(t);
                    break;
                case ShowAnimationType.Fade:
                    canvasGroup.alpha = 0;
                    gameObject.SetActive(true);
                    tween = Tween.Create(0f, 1f, durationShowPopup).WithEase(Ease.OutQuad).WithOnComplete(() =>
                    {
                        canvasGroup.alpha = 1;
                        OnAfterShow();
                    }).BindToAlpha(canvasGroup);
                    break;
                case ShowAnimationType.Move:
                    Vector3 showPos = t.position;
                    Vector3 fromPos = showPos + GetMoveOffset(showMovePopup, offsetShowMove);
                    t.position = fromPos;
                    gameObject.SetActive(true);
                    tween = Tween.Create(fromPos, showPos, durationShowPopup).WithEase(Ease.Linear).WithOnComplete(OnAfterShow).BindToPosition(t);
                    break;
                case ShowAnimationType.OutBackFromPoint:
                    Vector3 targetPos = t.position;
                    t.position = pointShowPos;
                    t.localScale = Vector3.zero;
                    gameObject.SetActive(true);
                    subTween = Tween.Create(pointShowPos, targetPos, durationShowPopup).WithEase(Ease.OutSine).BindToPosition(t);
                    tween = Tween.Create(Vector3.zero, defaultScale, durationShowPopup).WithEase(Ease.OutSine).WithOnComplete(OnAfterShow).BindToLocalScale(t);
                    break;
                default:
                    gameObject.SetActive(true);
                    OnAfterShow();
                    break;
            }
        }

        public virtual void Hide()
        {
            // Ignore repeated Hide() calls while the hide animation is still running
            if (isHiding) return;
            isHiding = true;
            CacheDefault();
            StopTweens();
            OnBeforeHide();
            if (!CanAnimateHide)
            {
                FinishHide();
                return;
            }

            var t = container.transform;
            switch (hideAnimationType)
            {
                case HideAnimationType.Fade:
                    tween = Tween.Create(canvasGroup.alpha, 0f, durationHidePopup).WithEase(Ease.InQuad).WithOnComplete(FinishHide).BindToAlpha(canvasGroup);
                    break;
                case HideAnimationType.InBack:
                    tween = Tween.Create(t.localScale, scaleFromHide, durationHidePopup).WithEase(Ease.InBack).WithOnComplete(FinishHide).BindToLocalScale(t);
                    break;
                case HideAnimationType.Move:
                    Vector3 hideTargetPos = t.position + GetMoveOffset(hideMovePopup, offsetHideMove);
                    tween = Tween.Create(t.position, hideTargetPos, durationHidePopup).WithEase(Ease.Linear).WithOnComplete(FinishHide).BindToPosition(t);
                    break;
                case HideAnimationType.InBackToPoint:
                    subTween = Tween.Create(t.position, pointHidePos, durationHidePopup).WithEase(Ease.InSine).BindToPosition(t);
                    tween = Tween.Create(t.localScale, Vector3.zero, durationHidePopup).WithEase(Ease.InSine).WithOnComplete(FinishHide).BindToLocalScale(t);
                    break;
                default:
                    FinishHide();
                    break;
            }
        }

        private void FinishHide()
        {
            if (subTween.IsActive) subTween.Cancel();
            gameObject.SetActive(false);
            ResetState();
            isHiding = false;
            OnAfterHide();
        }

        /// <summary>
        /// Cache the original transform of container once, so an interrupted tween
        /// never leaves a half-scaled / half-moved state as the new default.
        /// </summary>
        private void CacheDefault()
        {
            if (isCachedDefault || container == null) return;
            var t = container.transform;
            defaultLocalPos = t.localPosition;
            defaultScale = t.localScale;
            defaultLocalEuler = t.localEulerAngles;
            isCachedDefault = true;
        }

        private void StopTweens()
        {
            if (tween.IsActive) tween.Cancel();
            if (subTween.IsActive) subTween.Cancel();
        }

        private void ResetState()
        {
            canvasGroup.alpha = 1;
            if (!isCachedDefault) return;
            var t = container.transform;
            t.localPosition = defaultLocalPos;
            t.localScale = defaultScale;
            t.localEulerAngles = defaultLocalEuler;
        }

        private static Vector3 GetMoveOffset(MovePopupType type, float offset)
        {
            switch (type)
            {
                case MovePopupType.Left: return Vector3.left * offset;
                case MovePopupType.Right: return Vector3.right * offset;
                case MovePopupType.Up: return Vector3.up * offset;
                case MovePopupType.Down: return Vector3.down * offset;
                default: return Vector3.zero;
            }
        }

        protected virtual void OnBeforeShow()
        {
        }

        protected virtual void OnAfterShow()
        {
        }

        protected virtual void OnBeforeHide()
        {
        }

        protected virtual void OnAfterHide()
        {
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
        }
#endif
    }

    public enum ShowAnimationType
    {
        OutBack,
        Flip,
        Move,
        Fade,
        OutBackFromPoint,
    }

    public enum HideAnimationType
    {
        InBack,
        Fade,
        Move,
        InBackToPoint,
    }

    public enum MovePopupType
    {
        Left,
        Right,
        Up,
        Down
    }
}
