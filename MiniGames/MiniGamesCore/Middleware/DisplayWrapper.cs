using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGamesInterface.Display;
using System.Drawing;

namespace MiniGamesCore.Middleware
{
    class DisplayFactoryWrapper : DisplayFactory
    {
        public override string DisplaySymbolFile
        {
            get
            {
                return RuntimeSettings.CurrentDisplayFactory.DisplaySymbolFile;
            }
        }

        public override string Name
        {
            get
            {
                return RuntimeSettings.CurrentDisplayFactory.Name;
            }
        }

        public override DisplayBase CreateDisplay()
        {
            return new DisplayWrapper();
        }

        public override FrameBase CreateFrame()
        {
            return new FrameWrapper();
        }

        public override LoadingMessageBase CreateLoadingMessage(string message)
        {
            return new LoadingMessageWrapper(message);
        }

        public override SpriteBase CreateSprite(string imagePath)
        {
            return new SpriteWrapper(imagePath);
        }
    }

    class DisplayWrapper : DisplayBase
    {
        public DisplayBase Current { get; private set; }

        public DisplayWrapper()
        {
            RuntimeSettings.CurrentDisplayFactoryChanged += RuntimeSettings_CurrentDisplayFactoryChanged;
            Current = RuntimeSettings.CurrentDisplayFactory.CreateDisplay();
        }

        private void RuntimeSettings_CurrentDisplayFactoryChanged()
        {
            Current?.Dispose();
            Current = RuntimeSettings.CurrentDisplayFactory.CreateDisplay();
        }

        public override void Dispose()
        {
            RuntimeSettings.CurrentDisplayFactoryChanged -= RuntimeSettings_CurrentDisplayFactoryChanged;
            Current.Dispose();
        }

        public override void Hide()
        {
            Current.Hide();
        }

        public override void Show()
        {
            Current.Show();
        }

        public override float AspectRatio
        {
            get
            {
                return Current.AspectRatio;
            }
        }
    }

    class SpriteWrapper : SpriteBase
    {
        public SpriteBase Current { get; private set; }
        private string imagePath;

        public SpriteWrapper(string imagePath)
        {
            RuntimeSettings.CurrentDisplayFactoryChanged += RuntimeSettings_CurrentDisplayFactoryChanged;
            Current = RuntimeSettings.CurrentDisplayFactory.CreateSprite(this.imagePath = imagePath);
        }

        private void RuntimeSettings_CurrentDisplayFactoryChanged()
        {
            Current.Dispose();
            Current = RuntimeSettings.CurrentDisplayFactory.CreateSprite(imagePath);
        }

        public override void Dispose()
        {
            RuntimeSettings.CurrentDisplayFactoryChanged -= RuntimeSettings_CurrentDisplayFactoryChanged;
            Current?.Dispose();
        }

        public override float X
        {
            get
            {
                return Current.X;
            }

            set
            {
                Current.X = value;
            }
        }

        public override float Y
        {
            get
            {
                return Current.Y;
            }

            set
            {
                Current.Y = value;
            }
        }

        public override float Width
        {
            get
            {
                return Current.Width;
            }

            set
            {
                Current.Width = value;
            }
        }

        public override float Height
        {
            get
            {
                return Current.Height;
            }

            set
            {
                Current.Height = value;
            }
        }

        public override Color BackgroundColor
        {
            get
            {
                return Current.BackgroundColor;
            }

            set
            {
                Current.BackgroundColor = value;
            }
        }

        public override Color ForegroundColor
        {
            get
            {
                return Current.ForegroundColor;
            }

            set
            {
                Current.ForegroundColor = value;
            }
        }

        public override string ImagePath
        {
            get
            {
                return Current.ImagePath;
            }

            set
            {
                Current.ImagePath = value;
            }
        }
    }

    class LoadingMessageWrapper : LoadingMessageBase
    {
        public LoadingMessageBase Current { get; private set; }

        public LoadingMessageWrapper(string message) : base(message)
        {
            RuntimeSettings.CurrentDisplayFactoryChanged += RuntimeSettings_CurrentDisplayFactoryChanged;
            Current = RuntimeSettings.CurrentDisplayFactory.CreateLoadingMessage(message);
        }

        private void RuntimeSettings_CurrentDisplayFactoryChanged()
        {
            Current?.Dispose();
            Current = RuntimeSettings.CurrentDisplayFactory.CreateLoadingMessage(Message);
        }

        public override void Dispose()
        {
            RuntimeSettings.CurrentDisplayFactoryChanged -= RuntimeSettings_CurrentDisplayFactoryChanged;
            Current?.Dispose();
        }
    }

    class FrameWrapper : FrameBase
    {
        public FrameBase Current { get; private set; }

        public FrameWrapper()
        {
            RuntimeSettings.CurrentDisplayFactoryChanged += RuntimeSettings_CurrentDisplayFactoryChanged;
            Current = RuntimeSettings.CurrentDisplayFactory.CreateFrame();
        }

        private void RuntimeSettings_CurrentDisplayFactoryChanged()
        {
            Current?.Dispose();
            RuntimeSettings.CurrentDisplayFactoryChanged += RuntimeSettings_CurrentDisplayFactoryChanged;
        }

        public override void Dispose()
        {
            RuntimeSettings.CurrentDisplayFactoryChanged -= RuntimeSettings_CurrentDisplayFactoryChanged;
            Current.Dispose();
        }

        public override void Show()
        {
            Current.Show();
        }

        public override void Add(IFrameable element)
        {
            Current.Add(element);
        }

        public override Color BackgroundColor
        {
            get
            {
                return Current.BackgroundColor;
            }

            set
            {
                Current.BackgroundColor = value;
            }
        }

        public override IFrameable[] Elements
        {
            get
            {
                return Current.Elements;
            }
        }

        public override Color ForegroundColor
        {
            get
            {
                return Current.ForegroundColor;
            }

            set
            {
                Current.ForegroundColor = value;
            }
        }

        public override string ImagePath
        {
            get
            {
                return Current.ImagePath;
            }

            set
            {
                Current.ImagePath = value;
            }
        }

        public override void Remove(IFrameable element)
        {
            Current.Remove(element);
        }
    }
}
