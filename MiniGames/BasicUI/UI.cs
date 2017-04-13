using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGamesInterface.UI;
using MiniGamesInterface.Display;

namespace BasicUI
{
    class BasicUIFactory : UIFactory
    {
        public override string DisplaySymbolFile
        {
            get
            {
                return null;
            }
        }

        public override string Name
        {
            get
            {
                return "Basic UI";
            }
        }

        public override UIBase CreateUI(CurrentState currentState)
        {
            return new UI(currentState);
        }
    }

    class UI : UIBase
    {
        FrameBase MenuFrame;

        public UI(CurrentState currentState) : base(currentState)
        {
        }

        public override void Dispose()
        {
        }

        public override void Hide()
        {
        }

        public override void Show()
        {
            if (MenuFrame == null)
            {
                MenuFrame = CurrentDisplay.CreateFrame();
                MenuFrame.BackgroundColor = System.Drawing.Color.Blue;
            }
            MenuFrame.Show();
        }
    }
}
