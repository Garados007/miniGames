using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MiniGamesInterface.Display;

namespace SmallFormDisplay
{
    public partial class Display : Form
    {
        public static Display Current;

        public Display()
        {
            Current = this;
            InitializeComponent();
            buffer = BufferedGraphicsManager.Current.Allocate(CreateGraphics(), new Rectangle(new Point(), ClientSize));
            new Thread(run).Start();
        }

        BufferedGraphics buffer;
        Thread renderer;
        public static Frame CurrentFrame;
        
        void run()
        {
            while (!Disposing && !IsDisposed)
            {
                CurrentFrame?.Render(buffer.Graphics, ClientSize);
                buffer.Render();
                Thread.Sleep(20);
            }
            buffer.Dispose();
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

        public override float AspectRatio
        {
            get
            {
                var s = Display.ClientSize;
                return (float)s.Width / s.Height;
            }
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

        public override FrameBase CreateFrame()
        {
            return new Frame();
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

    public class Frame : FrameBase, IRenderable
    {
        public override void Dispose()
        {
        }

        public void Render(Graphics g, Size size)
        {
            using (var brush = new SolidBrush(BackgroundColor))
                g.FillRectangle(brush, 0, 0, size.Width, size.Height);
            foreach (var e in Elements)
                if (e is IRenderable)
                    ((IRenderable)e).Render(g, size);
        }

        public override void Show()
        {
            Display.CurrentFrame = this;
        }
    }

    public interface IRenderable
    {
        void Render(Graphics g, Size size);
    }
}
