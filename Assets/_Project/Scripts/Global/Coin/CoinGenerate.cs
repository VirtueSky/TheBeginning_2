using System;
using System.Collections.Generic;
using System.Linq;
using Base.Data;
using UnityEngine;
using VirtueSky.Audio;
using VirtueSky.Core;
using VirtueSky.ObjectPooling;
using Cysharp.Threading.Tasks;
using VirtueSky.Pattern;
using VirtueSky.Tweening;
using Random = UnityEngine.Random;

namespace Base.Global
{
    public class CoinGenerate : Singleton<CoinGenerate>
    {
        [SerializeField] private Transform holder;
        [SerializeField] private int numberCoin;
        [SerializeField] private int delay;
        [SerializeField] private float durationNear;
        [SerializeField] private float durationTarget;
        [SerializeField] private Ease easeNear;
        [SerializeField] private Ease easeTarget;
        [SerializeField] private float scale = 1;
        [SerializeField] private float offsetNear = 1;
        [SerializeField] private GameObject iconCoinPrefab;
        [SerializeField] private SoundData soundCoinMove;

        public static event Action OnMoveOneCoinDone;
        public static event Action OnMoveAllCoinDone;
        public static event Action OnDecreaseCoin;

        private bool isScaleIconTo = false;

        private List<GameObject> coinsActive = new List<GameObject>();
        private List<GameObject> listTo = new List<GameObject>();
        private int cacheCurrentCoin;
        private Vector3 from;
        private GameObject to;

        public override void OnEnable()
        {
            base.OnEnable();
            CoinSystem.OnAddCoinCompletedEvent += GenerateCoin;
            CoinSystem.OnMinusCoinCompletedEvent += DecreaseCoin;
            CoinSystem.OnSetFromCoinGenerateEvent += OnSetFrom;
            SaveCache();
            OnSetFrom(holder.position);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CoinSystem.OnAddCoinCompletedEvent -= GenerateCoin;
            CoinSystem.OnMinusCoinCompletedEvent -= DecreaseCoin;
            CoinSystem.OnSetFromCoinGenerateEvent -= OnSetFrom;
        }

        private void SaveCache()
        {
            cacheCurrentCoin = CoinSystem.GetCurrentCoin();
        }

        private void DecreaseCoin()
        {
            OnDecreaseCoin?.Invoke();
            SaveCache();
        }

        private void OnSetFrom(Vector3 from)
        {
            this.from = from;
        }

        public void AddTo(GameObject obj)
        {
            listTo.Add(obj);
            to = listTo.Last();
        }

        public void RemoveTo(GameObject obj)
        {
            if (listTo.Count == 0) return;
            listTo.Remove(obj);
            if (listTo.Count > 0)
            {
                to = listTo.Last();
            }
        }


        private async void GenerateCoin()
        {
            isScaleIconTo = false;
            for (int i = 0; i < this.numberCoin; i++)
            {
                await UniTask.Delay(Random.Range(0, delay));
                GameObject coin = iconCoinPrefab.Spawn(holder);
                coin.transform.localScale = Vector3.one * scale;
                coinsActive.Add(coin);
                coin.transform.position = from;
                MoveToTarget(coin, () =>
                {
                    coinsActive.Remove(coin);
                    coin.DeSpawn();
                    if (!isScaleIconTo)
                    {
                        isScaleIconTo = true;
                        soundCoinMove.PlaySfx();
                        ScaleIconTo();
                    }

                    OnMoveOneCoinDone?.Invoke();
                    if (coinsActive.Count == 0)
                    {
                        OnMoveAllCoinDone?.Invoke();
                        SaveCache();
                        OnSetFrom(holder.position);
                    }
                });
            }
        }

        private void MoveToTarget(GameObject coin, Action completed)
        {
            Tween.Create(coin.transform.position, coin.transform.position + (Vector3)Random.insideUnitCircle * offsetNear, durationNear)
                .WithEase(easeNear)
                .WithOnComplete(() =>
                {
                    Tween.Create(coin.transform.position, to.transform.position, durationTarget)
                        .WithEase(easeTarget)
                        .WithOnComplete(completed).BindToPosition(coin.transform);
                }).BindToPosition(coin.transform);
            
        }


        private void ScaleIconTo()
        {
            Vector3 currentScale = to.transform.localScale;
            Vector3 nextScale = currentScale + new Vector3(.1f, .1f, .1f);
            Tween.Create(currentScale, nextScale, durationTarget).WithEase(Ease.OutBack).WithOnComplete(() =>
            {
                Tween.Create(nextScale, currentScale, durationTarget).WithEase(Ease.OutBack).BindToLocalScale(to.transform);
            }).BindToLocalScale(to.transform);
        }
    }
}