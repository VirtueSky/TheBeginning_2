using System;
using System.Collections.Generic;
using System.Linq;
using Base.Data;
using PrimeTween;
using UnityEngine;
using VirtueSky.Audio;
using VirtueSky.Core;
using VirtueSky.ObjectPooling;
using VirtueSky.Threading.Tasks;
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

        public Action moveOneCoinDone;
        public Action moveAllCoinDone;
        public Action decreaseCoin;
        private bool isScaleIconTo = false;

        private List<GameObject> coinsActive = new List<GameObject>();
        private List<GameObject> listTo = new List<GameObject>();
        private int cacheCurrentCoin;
        private Vector3 from;
        private GameObject to;

        public override void OnEnable()
        {
            base.OnEnable();
            Observer.CoinTotalChanged += HandleGenerateCoin;
            SaveCache();
            SetFrom(holder.position);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Observer.CoinTotalChanged -= HandleGenerateCoin;
        }

        private void SaveCache()
        {
            cacheCurrentCoin = UserData.CoinTotal;
        }

        private void HandleGenerateCoin()
        {
            if (UserData.CoinTotal > cacheCurrentCoin)
            {
                GenerateCoin();
            }
            else
            {
                decreaseCoin?.Invoke();
                SaveCache();
            }
        }

        public void SetFrom(Vector3 from)
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
            listTo.Remove(obj);
            if (listTo.Count > 0)
            {
                to = listTo.Last();
            }
        }


        public async void GenerateCoin()
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

                    moveOneCoinDone?.Invoke();
                    if (coinsActive.Count == 0)
                    {
                        moveAllCoinDone?.Invoke();
                        SaveCache();
                        SetFrom(holder.position);
                    }
                });
            }
        }

        private void MoveToTarget(GameObject coin, Action completed)
        {
            coin.transform
                .DOMove(coin.transform.position + (Vector3)Random.insideUnitCircle * offsetNear,
                    durationNear)
                .SetEase(easeNear)
                .OnComplete(
                    () =>
                    {
                        coin.transform.DOMove(to.transform.position, durationTarget).SetEase(easeTarget)
                            .OnComplete(completed);
                    });
        }


        private void ScaleIconTo()
        {
            Vector3 currentScale = Vector3.one;
            Vector3 nextScale = currentScale + new Vector3(.1f, .1f, .1f);
            to.transform.DOScale(nextScale, durationTarget).SetEase(Ease.OutBack)
                .OnComplete((() => { to.transform.DOScale(currentScale, durationTarget / 2).SetEase(Ease.InBack); }));
        }
    }
}