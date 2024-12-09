using System;
using Base.Data;
using Base.Game;
using Base.Global;
using UnityEngine;
using VirtueSky.Core;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

namespace Base.UI
{
    public class WinPopup : UIPopup
    {
        [FormerlySerializedAs("gameConfig")] [SerializeField]
        private GameSettings gameSettings;

        private bool isDoneAllCoinGenerate;


        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            isDoneAllCoinGenerate = false;
            CoinGenerate.OnMoveAllCoinDone += OnMoveAllCoinDone;
        }

        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            CoinGenerate.OnMoveAllCoinDone -= OnMoveAllCoinDone;
        }

        void OnMoveAllCoinDone()
        {
            isDoneAllCoinGenerate = true;
        }

        public async void OnClickContinue()
        {
            CoinSystem.AddCoin(gameSettings.winLevelMoney);
            await UniTask.WaitUntil(() => isDoneAllCoinGenerate);
            GameManager.Instance.PlayCurrentLevel();
            Hide();
        }

        public void OnClickAds()
        {
        }
    }
}