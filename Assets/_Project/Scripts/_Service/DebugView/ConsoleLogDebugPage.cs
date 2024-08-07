using System.Threading.Tasks;
using Consolation;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace Base.Services
{
    public class ConsoleLogDebugPage : DefaultDebugPageBase
    {
        private Sprite iconToggle;
        private Sprite iconInput;
        private Sprite iconOk;
        private Sprite iconSlider;
        protected override string Title => "Console Log";
        private bool isCustomSizeWindow;
        private bool isShowConsole;
        private string targetFontSize;
        private string targetScaleFactor;

        public void Init(Sprite _iconToggle, Sprite _iconInput, Sprite _iconOk, Sprite _iconSlider)
        {
            iconToggle = _iconToggle;
            iconInput = _iconInput;
            iconOk = _iconOk;
            iconSlider = _iconSlider;
        }

        public override Task Initialize()
        {
            AddSwitch(isShowConsole, "Show", icon: iconToggle, valueChanged: b =>
            {
                isShowConsole = b;
                ConsoleInGame.Activate(isShowConsole);
            });
            FontSize();
            ScaleFactor();
            AddSwitch(ConsoleInGame.IsCustomSizeWindowConsole, "Enable Custom Size Window", icon: iconToggle,
                valueChanged: b => ConsoleInGame.IsCustomSizeWindowConsole = b);
            AddSlider(ConsoleInGame.CustomWidth, 600, 1080, "Custom Width", icon: iconSlider,
                valueChanged: f => ConsoleInGame.CustomWidth = f);
            AddSlider(ConsoleInGame.CustomHeight, 500, 1920, "Custom Height", icon: iconSlider,
                valueChanged: f => ConsoleInGame.CustomHeight = f);
            return base.Initialize();
        }

        void FontSize()
        {
            AddInputField("Input Font Size", valueChanged: s => targetFontSize = s, icon: iconInput);
            AddButton("Enter Font Size", icon: iconOk, clicked: () =>
            {
                if (targetFontSize != "")
                {
                    int size = int.Parse(targetFontSize);
                    if (size < 10)
                    {
                        size = 10;
                    }

                    ConsoleInGame.LogFontSize = size;
                }
            });
        }

        void ScaleFactor()
        {
            AddInputField("Input Scale Factor", valueChanged: s => targetScaleFactor = s,
                icon: iconInput);
            AddButton("Enter Font Size", icon: iconOk, clicked: () =>
            {
                if (targetScaleFactor != "")
                {
                    float size = float.Parse(targetScaleFactor);
                    if (size < 1)
                    {
                        size = 1;
                    }

                    ConsoleInGame.ScaleFactor = size;
                }
            });
        }
    }
}