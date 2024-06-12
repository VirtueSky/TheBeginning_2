using System.Threading.Tasks;
using Base.Data;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace Base.Services
{
    public class GameDebugPage : DefaultDebugPageBase
    {
        private ItemConfig itemConfig;
        private string _targetCoin = "";
        protected override string Title => "Game Debug";

        public void Init(ItemConfig _itemConfig)
        {
            itemConfig = _itemConfig;
        }

        public override Task Initialize()
        {
            AddButton("Add 10000 Coin", clicked: () => UserData.CoinTotal += 10000);
            AddInputField("Input Coin:", valueChanged: s => _targetCoin = s, icon: DebugViewStatic.IconInputDebug);
            AddButton("Enter Input Coin", clicked: () =>
                {
                    if (_targetCoin != "")
                    {
                        UserData.CoinTotal = int.Parse(_targetCoin);
                        _targetCoin = "";
                    }
                },
                icon: DebugViewStatic.IconOkeDebug);
            AddButton("Unlock All Skin", clicked: () => itemConfig.UnlockAllSkins());
            AddSwitch(UserData.IsOffUIDebug, "Is Hide UI", valueChanged: b => UserData.IsOffUIDebug = b,
                icon: DebugViewStatic.IconToggleDebug);
            AddSwitch(UserData.IsTestingDebug, "Is Testing", valueChanged: b => UserData.IsTestingDebug = b,
                icon: DebugViewStatic.IconToggleDebug);
            Reload();
            return base.Initialize();
        }
    }
}