using System.Threading.Tasks;
using Base.Data;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace Base.Services
{
    public class GameDebugPage : DefaultDebugPageBase
    {
        private ItemConfig itemConfig;
        private Sprite iconInput;
        private Sprite iconOk;
        private Sprite iconToggle;
        private string _targetCoin = "";
        protected override string Title => "Game Debug";

        public void Init(ItemConfig _itemConfig, Sprite _iconInput, Sprite _iconOk, Sprite _iconToggle)
        {
            itemConfig = _itemConfig;
            iconInput = _iconInput;
            iconOk = _iconOk;
            iconToggle = _iconToggle;
        }

        public override Task Initialize()
        {
            AddButton("Add 10000 Coin", clicked: () => UserData.CoinTotal += 10000);
            AddInputField("Input Coin:", valueChanged: s => _targetCoin = s, icon: iconInput);
            AddButton("Enter Input Coin", clicked: () =>
                {
                    if (_targetCoin != "")
                    {
                        UserData.CoinTotal = int.Parse(_targetCoin);
                    }
                },
                icon: iconOk);
            AddButton("Unlock All Skin", clicked: () => itemConfig.UnlockAllSkins());
            AddSwitch(UserData.IsOffUIDebug, "Is Hide UI", valueChanged: b => UserData.IsOffUIDebug = b,
                icon: iconToggle);
            AddSwitch(UserData.IsTestingDebug, "Is Testing", valueChanged: b => UserData.IsTestingDebug = b,
                icon: iconToggle);
            Reload();
            return base.Initialize();
        }
    }
}