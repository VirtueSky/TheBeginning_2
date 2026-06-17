using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VirtueSky.Audio;
using VirtueSky.Inspector;
using VirtueSky.ObjectPooling;
using VirtueSky.Pattern;
using VirtueSky.Tweening;
using Random = UnityEngine.Random;

namespace Base.Global.Currency
{
    /// <summary>
    /// Abstract base class cho tất cả currency animators.
    /// Quản lý animation thu thập currency từ source đến target icon.
    /// </summary>
    /// <typeparam name="T">Concrete currency animator type</typeparam>
    public abstract class BaseCurrencyAnimator<T> : Singleton<T> where T : MonoBehaviour
    {

        /// <summary>
        /// Prefab được sử dụng cho animation (pooled).
        /// </summary>
        protected abstract GameObject AnimationPrefab { get; }

        /// <summary>
        /// Sound effect phát khi animation hoàn tất.
        /// </summary>
        protected abstract SoundData CollectSound { get; }

        // /// <summary>
        // /// Sound effect phát khi coin bắt đầu bung ra. Optional — null = không play.
        // /// </summary>
         protected virtual SoundData SpawnSound => null;
        
        [HeaderLine("Near", false)]
        [SerializeField] protected float durationNear = 0.3f;
        [SerializeField] protected Ease easeNear = Ease.OutQuad;
        [SerializeField] protected float offsetNear = 1f;
        [HeaderLine("Target", false)]
        [SerializeField] protected float durationTarget = 0.5f;
        [SerializeField] protected Ease easeTarget = Ease.InQuad;
        [HeaderLine("General", false)]
        [SerializeField] protected float scale = 1f;
        [SerializeField] protected int spawnCount = 10;
        [SerializeField] private Transform holder;
        [SerializeField] private int randomDelay = 100;



        /// <summary>
        /// Kích hoạt khi một prefab reach target.
        /// </summary>
        public Action OnMoveOneCoinDone;

        /// <summary>
        /// Kích hoạt khi tất cả prefabs reach target.
        /// </summary>
        public Action OnMoveAllCoinDone;



        private Stack<GameObject> _targetStack = new Stack<GameObject>();
        private List<GameObject> _activePrefabs = new List<GameObject>();
        private bool _hasPlayedSound = false;

        // Cache cho Resources.Load
        private static GameObject _cachedPrefab;
        private static SoundData _cachedSound;
        private bool isFirstCoinMoveDone = false;


        protected override void Awake()
        {
            base.Awake();

            // Cache assets
            if (_cachedPrefab == null)
            {
                _cachedPrefab = AnimationPrefab;
            }

            if (_cachedSound == null)
            {
                _cachedSound = CollectSound;
            }

            // Prewarm pool
            if (_cachedPrefab != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    var instance = _cachedPrefab.Spawn(GetHolder());
                    instance.DeSpawn();
                }
            }
        }
        private Transform GetHolder()
        {
            return holder != null ? holder : this.transform;
        }


        private GameObject CurrentTarget => _targetStack.Count > 0 ? _targetStack.Peek() : null;

        /// <summary>
        /// Push target mới vào stack.
        /// </summary>
        public void PushTarget(GameObject target)
        {
            _targetStack.Push(target);
        }

        /// <summary>
        /// Pop target hiện tại ra khỏi stack.
        /// </summary>
        public void PopTarget()
        {
            if (_targetStack.Count > 0)
                _targetStack.Pop();
        }

        /// <summary>
        /// Xóa toàn bộ stack.
        /// </summary>
        public void ClearTargets()
        {
            _targetStack.Clear();
        }

        /// <summary>
        /// Animate currency collection từ source position đến target.
        /// </summary>
        /// <param name="amount">Số lượng currency (không sử dụng, spawn theo SpawnCount)</param>
        /// <param name="from">Vị trí nguồn để spawn animation</param>
        public async void AnimateCollection(int amount, Vector3 from)
        {
            if (CurrentTarget == null)
            {
                Debug.LogWarning($"[{typeof(T).Name}] Target not set, skipping animation");
                return;
            }

            if (_cachedPrefab == null)
            {
                Debug.LogWarning($"[{typeof(T).Name}] AnimationPrefab is null, skipping animation");
                return;
            }

            if (from == default)
            {
                from = holder.transform.position;
            }

            _hasPlayedSound = false;
            isFirstCoinMoveDone = false;
            SpawnSound?.PlaySfx();
            for (int i = 0; i < spawnCount; i++)
            {
                await UniTask.Delay(Random.Range(0, randomDelay)); // Random delay 0-0.2s

                GameObject prefab = _cachedPrefab.Spawn(GetHolder());
                prefab.transform.position = from;
                prefab.transform.localScale = Vector3.one * scale;
                _activePrefabs.Add(prefab);

                AnimatePrefabToTarget(prefab, () =>
                {
                    _activePrefabs.Remove(prefab);
                    prefab.DeSpawn();

                    // Play sound và scale icon chỉ một lần
                    if (!_hasPlayedSound)
                    {
                        _hasPlayedSound = true;
                        PlayCollectSound();
                        ScaleTargetIcon();
                    }

                    if (!isFirstCoinMoveDone)
                    {
                        isFirstCoinMoveDone = true;
                        OnMoveOneCoinDone?.Invoke();
                    }
                    
                    // Khi tất cả prefabs done
                    if (_activePrefabs.Count == 0)
                    {
                        OnMoveAllCoinDone?.Invoke();
                        OnAnimationComplete();
                    }
                });
            }
        }



        /// <summary>
        /// Animate một prefab từ position hiện tại đến target với two-phase motion.
        /// </summary>
        private void AnimatePrefabToTarget(GameObject prefab, Action onComplete)
        {
            // Phase 1: Random offset gần source
            Vector3 nearPosition = prefab.transform.position + (Vector3)Random.insideUnitCircle * offsetNear;
            
            Tween.Create(prefab.transform.position, nearPosition, durationNear).WithEase(easeNear).WithOnComplete(() =>
            {
                Tween.Create(prefab.transform.position, CurrentTarget.transform.position, durationTarget).WithEase(easeTarget).WithOnComplete(onComplete).BindToPosition(prefab.transform);
            }).BindToPosition(prefab.transform);
        }

        /// <summary>
        /// Scale target icon với bounce effect.
        /// </summary>
        private void ScaleTargetIcon()
        {
            if (CurrentTarget == null) return;

            Vector3 originalScale = CurrentTarget.transform.localScale;
            Vector3 bounceScale = originalScale * 1.2f;
            
            Tween.Create(originalScale, bounceScale, durationTarget).WithEase(Ease.OutBack).WithOnComplete(() =>
            {
                Tween.Create(bounceScale, originalScale, durationTarget).WithEase(Ease.InBack).BindToLocalScale(CurrentTarget.transform);
            }).BindToLocalScale(CurrentTarget.transform);
        }

        /// <summary>
        /// Phát collect sound effect.
        /// </summary>
        private void PlayCollectSound()
        {
            if (_cachedSound != null)
            {
                _cachedSound.PlaySfx();
            }
        }



        /// <summary>
        /// Hook được gọi khi animation sequence hoàn tất.
        /// Override để customize behavior.
        /// </summary>
        protected virtual void OnAnimationComplete() { }

    }
}
