using DG.Tweening;
using Pancake;
using UnityEngine;


public class UIEffect : MonoBehaviour
{
    [Header("Data config")]
    public UIEffectType UIEffectType;
    public bool PlayOnAwake = true;
    public float Time = .5f;
    
    public Vector3 FromScale = Vector3.zero;
    public Vector3 SaveLocalScale; 
    [Header("Shake Effect")]
    public float Strength = 3f;
    [Header("Move Effect")] 
    [ShowIf("UIEffectType", UIEffectType.Move)] public MoveType MoveType;
    [ShowIf("IsShowAttributeFromPosition")] public Vector3 FromPosition;
    [ShowIf("IsShowAttributesMoveDirection")] public DirectionType DirectionType;
    [ShowIf("IsShowAttributesMoveDirection")] public float Offset;
    [ShowIf("UIEffectType", UIEffectType.Move)] [ReadOnly] public Vector3 SaveArchorPosition;

    private RectTransform rectTransform;
    private Sequence sequence;

    private bool IsShowAttributeFromPosition => UIEffectType == UIEffectType.Move && MoveType == MoveType.Vector3;
    private bool IsShowAttributesMoveDirection => UIEffectType == UIEffectType.Move && MoveType == MoveType.Direction;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        SaveArchorPosition = rectTransform.anchoredPosition;
        SaveLocalScale = rectTransform.localScale;
    }

    public void OnEnable()
    {
        if (PlayOnAwake)
        {
            PlayAnim();
        }
    }

    public void PlayAnim()
    {
        switch (UIEffectType)
        {
            case UIEffectType.OutBack:
                transform.localScale = FromScale;
                sequence = DOTween.Sequence().Append(transform.DOScale(Vector3.one, Time).OnKill(()=>transform.localScale = SaveLocalScale).SetEase(Ease.OutBack));
                break;
            case UIEffectType.Shake:
                sequence = DOTween.Sequence().Append(transform.DOShakeRotation(Time, Strength).SetEase(Ease.Linear));
                break;
            case UIEffectType.Move:
                rectTransform.anchoredPosition = SaveArchorPosition;
                switch (MoveType)
                {
                    case MoveType.Vector3:
                        transform.DOLocalMove(SaveArchorPosition, Time).SetEase(Ease.Linear);
                        break;
                    case MoveType.Direction:
                        switch (DirectionType)
                        {
                            case DirectionType.Up:
                                sequence = DOTween.Sequence().Append(transform.DOLocalMoveY(transform.localPosition.y + Offset, Time).SetEase(Ease.InBack));
                                break;
                            case DirectionType.Down:
                                sequence = DOTween.Sequence().Append(transform.DOLocalMoveY(transform.localPosition.y - Offset, Time).SetEase(Ease.InBack));
                                break;
                            case DirectionType.Left:
                                sequence = DOTween.Sequence().Append(transform.DOLocalMoveX(transform.localPosition.x - Offset, Time).SetEase(Ease.InBack));
                                break;
                            case DirectionType.Right:
                                sequence = DOTween.Sequence().Append(transform.DOLocalMoveX(transform.localPosition.x + Offset, Time).SetEase(Ease.InBack));
                                break;
                        } 
                        break;
                }
                break;
        }
    }

    public void OnDisable()
    {
        Reset();
        sequence?.Kill();
    }
    
    
    public void Reset()
    {
        if (!Application.isPlaying) return;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = SaveArchorPosition;
        rectTransform.localScale = SaveLocalScale;
    }
}

public enum UIEffectType
{
    OutBack,
    Shake,
    Move,
}

public enum MoveType
{
    Vector3,
    Direction,
}

public enum DirectionType
{
    Up,
    Down,
    Left,
    Right,
}