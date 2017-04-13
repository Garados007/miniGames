using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface.Display
{
    public abstract class DisplayFactory
    {
        public abstract string Name { get; }

        public abstract string DisplaySymbolFile { get; }

        public abstract DisplayBase CreateDisplay();

        public abstract SpriteBase CreateSprite(string imagePath);

        public abstract LoadingMessageBase CreateLoadingMessage(string message);
    }

    public abstract class DisplayBase : IDisposable
    {
        public abstract void Show();

        public abstract void Hide();

        public abstract void Dispose();
    }

    public abstract class SpriteBase : IDisposable
    {
        public abstract void Dispose();

        public virtual float X { get; set; }

        public virtual float Y { get; set; }

        public virtual float Width { get; set; }

        public virtual float Height { get; set; }
    }

    public abstract class LoadingMessageBase : IDisposable
    {
        public string Message { get; private set; }

        public LoadingMessageBase(string message)
        {
            Message = message;
        }

        public abstract void Dispose();
    }
}
