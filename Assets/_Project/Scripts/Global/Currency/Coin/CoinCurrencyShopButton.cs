using UnityEngine;
using UnityEngine.UI;

namespace Base.Global.Currency
{
    [RequireComponent(typeof(Button))]
    public class CoinCurrencyShopButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
        }
    }
}
