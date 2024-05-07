using VirtueSky.Core;

namespace Base.Services
{
    public class RequireInternet : Singleton<RequireInternet>
    {
        private void Start()
        {
            Show(false);
        }

        public void Show(bool isShow)
        {
            if (gameObject.activeSelf == isShow) return;
            gameObject.SetActive(isShow);
        }
    }
}