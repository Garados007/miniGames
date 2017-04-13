using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniGamesInterface.Display;

namespace SmallFormDisplay
{
    public partial class Display : Form
    {
        public Display()
        {
            InitializeComponent();
        }

        public void InvokeOrRun(Action action)
        {
            if (InvokeRequired) Invoke(action);
            else action();
        }
    }

    public class DisplayWorker : DisplayBase
    {
        public Display Display;

        public override void Dispose()
        {
            Display?.InvokeOrRun(() => Display.Dispose());
            Display = null;
        }

        public override void Hide()
        {
            Display?.InvokeOrRun(() => Display.Close());
        }

        public override void Show()
        {
            Display = new Display();
            Application.Run(Display);
        }
    }

    public class DisplayFactory : MiniGamesInterface.Display.DisplayFactory
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
                return "Small Form Display";
            }
        }

        public override DisplayBase CreateDisplay()
        {
            return new DisplayWorker();
        }

        public override LoadingMessageBase CreateLoadingMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override SpriteBase CreateSprite(string imagePath)
        {
            throw new NotImplementedException();
        }
    }
}
