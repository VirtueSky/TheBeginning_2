using System;
using Base.Data;
using Base.Game;
using Base.Global;
using UnityEngine;
using VirtueSky.Core;
using VirtueSky.Threading.Tasks;

namespace Base.UI
{
    public class PopupWin : UIPopup
    {
        [SerializeField] private GameConfig gameConfig;
        private bool isDoneAllCoinGenerate;


        protected override void OnBeforeShow()
        {
            base.OnBeforeShow();
            isDoneAllCoinGenerate = false;
            CoinGenerate.Instance.moveAllCoinDone += OnMoveAllCoinDone;
        }

        protected override void OnBeforeHide()
        {
            base.OnBeforeHide();
            CoinGenerate.Instance.moveAllCoinDone -= OnMoveAllCoinDone;
        }

        void OnMoveAllCoinDone()
        {
            isDoneAllCoinGenerate = true;
        }

        public async void OnClickContinue()
        {
            UserData.CoinTotal += gameConfig.winLevelMoney;
            await UniTask.WaitUntil(() => isDoneAllCoinGenerate);
            GameManager.Instance.PlayCurrentLevel();
            Hide();
        }

        public void OnClickAds()
        {
        }
    }
}