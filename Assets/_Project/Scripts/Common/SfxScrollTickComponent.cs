using System;
using UnityEngine;
using UnityEngine.UI;
using VirtueSky.Audio;

public class SfxScrollTickComponent : MonoBehaviour
{
    public enum TickMode
    {
        PerDistance,
        PerItem
    }

    [SerializeField] private ScrollRect scrollRect;
    [Header("Tick Logic")] public TickMode mode = TickMode.PerDistance;

    [Tooltip("Khoảng cách (pixel) cần di chuyển để kêu 1 tick (PerDistance).")] [Min(1f)]
    public float pixelsPerTick = 60f;

    [Tooltip("Kích thước 1 item theo trục scroll (pixel) (PerItem).")] [Min(1f)]
    public float itemSize = 200f;

    [Tooltip("Vận tốc tối thiểu (px/s) để cho phép phát tick.")] [Min(0f)]
    public float minVelocity = 100f;

    [Tooltip("Thời gian tối thiểu giữa 2 tick (giây).")] [Min(0f)]
    public float minInterval = 0.07f;

    [Tooltip("Giới hạn tick tối đa mỗi giây.")] [Min(1f)]
    public int maxTicksPerSecond = 10;

    [SerializeField] SoundData tickSound;

    // State
    private Vector2 _lastContentPos;
    private float _accumulated; // cho PerDistance
    private int _lastItemIndex; // cho PerItem
    private float _lastTickTime = -999f;

    // tick budget theo giây
    private int _ticksThisSecond = 0;
    private float _tickWindowTimer = 0f;

    void OnEnable()
    {
        _lastContentPos = scrollRect.content ? scrollRect.content.anchoredPosition : Vector2.zero;
        _accumulated = 0f;
        _lastItemIndex = CalcItemIndex(_lastContentPos);
        _lastTickTime = -999f;
        _ticksThisSecond = 0;
        _tickWindowTimer = 0f;

        scrollRect.onValueChanged.AddListener(ScrollOnValueChanged);
    }

    void OnDisable()
    {
        scrollRect.onValueChanged.RemoveListener(ScrollOnValueChanged);
    }

    private void ScrollOnValueChanged(Vector2 v2)
    {
        if (scrollRect.content == null) return;

        // Cập nhật ngân sách tick/giây bằng unscaledTime (ổn định khi Time.timeScale != 1)
        _tickWindowTimer += Time.unscaledDeltaTime;
        if (_tickWindowTimer >= 1f)
        {
            _tickWindowTimer -= 1f;
            _ticksThisSecond = 0;
        }

        // Kiểm tra vận tốc để tránh tick khi lướt rất chậm
        if (scrollRect.velocity.magnitude < minVelocity)
        {
            // cập nhật vị trí để không tích lũy sai, nhưng không tick
            _lastContentPos = scrollRect.content.anchoredPosition;
            return;
        }

        // Tính thay đổi theo trục chính
        var currentPos = scrollRect.content.anchoredPosition;
        var delta = currentPos - _lastContentPos;
        float axisDelta = scrollRect.horizontal ? Mathf.Abs(delta.x) : Mathf.Abs(delta.y);

        bool shouldTick = false;

        if (mode == TickMode.PerDistance)
        {
            _accumulated += axisDelta;

            // chặn giá trị quá nhỏ tránh noise
            float threshold = Mathf.Max(4f, pixelsPerTick);
            if (_accumulated >= threshold)
            {
                _accumulated = 0f;
                shouldTick = true;
            }
        }
        else // PerItem
        {
            int currentIndex = CalcItemIndex(currentPos);
            if (currentIndex != _lastItemIndex)
            {
                _lastItemIndex = currentIndex;
                shouldTick = true;
            }
        }

        // Gate cuối: cooldown + giới hạn tick/giây
        if (shouldTick &&
            (Time.unscaledTime - _lastTickTime) >= minInterval &&
            _ticksThisSecond < maxTicksPerSecond)
        {
            // Phát âm: dùng PlayOneShot của hệ thống âm thanh của bạn
            tickSound.PlaySfx();

            _lastTickTime = Time.unscaledTime;
            _ticksThisSecond++;
        }

        _lastContentPos = currentPos;
    }

    private int CalcItemIndex(Vector2 anchoredPos)
    {
        float axisPos = scrollRect.horizontal ? Mathf.Abs(anchoredPos.x) : Mathf.Abs(anchoredPos.y);
        return Mathf.RoundToInt(axisPos / Mathf.Max(1f, itemSize));
    }
#if UNITY_EDITOR
    private void Reset()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
#endif
}