using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MiniGamesInterface.Display
{
    public abstract class DisplayFactory : IFactory
    {
        public abstract string Name { get; }

        public abstract string DisplaySymbolFile { get; }

        public abstract DisplayBase CreateDisplay();

        public abstract SpriteBase CreateSprite(string imagePath);

        public abstract LoadingMessageBase CreateLoadingMessage(string message);

        public abstract FrameBase CreateFrame();
    }

    public abstract class DisplayBase : IDisposable
    {
        public abstract void Show();

        public abstract void Hide();

        public abstract void Dispose();

        /// <summary>
        /// Das Seitenverhältnis (Breite/Höhe) der Oberfläche
        /// </summary>
        public virtual float AspectRatio
        {
            get
            {
                return 1;
            }
        }
    }

    /// <summary>
    /// Ein kleines Element, welches auf dem Bildschirm relativ (!) positioniert und animiert werden kann.
    /// </summary>
    public abstract class SpriteBase : IDisposable, IFrameable, IPositionable, IRenderable
    {
        public abstract void Dispose();

        public virtual float X { get; set; }

        public virtual float Y { get; set; }

        public virtual float Width { get; set; }

        public virtual float Height { get; set; }

        public virtual Color BackgroundColor { get; set; }

        public virtual Color ForegroundColor { get; set; }

        public virtual string ImagePath { get; set; }
    }

    /// <summary>
    /// Eine Ladebox, die sich über das aktuelle Frame drüber legt und kurze Ladezeiten darstellt.
    /// </summary>
    public abstract class LoadingMessageBase : IDisposable, IFrameable
    {
        public string Message { get; private set; }

        public LoadingMessageBase(string message)
        {
            Message = message;
        }

        public abstract void Dispose();
    }

    /// <summary>
    /// Stellt einen groben Rahmen und Container für Darstellungselemente dar. Alle Elemente in diesen Frame
    /// werden gemeinsam angezeigt und gemeinsam ausgeblendet. Es kann immer nur ein Frame sichtbar sein, aber 
    /// ein Games oder UIs dürfen mehrere Frames besitzen. Frames sind nicht schachtelbar.
    /// Wie ein Frame dargestellt wird, welches Frame oder ob es dargestellt wird obliegt dem Display. Ein 
    /// Element kann theoretisch mehreren Frames angehören, aber ob dies auch wirklich geht obliegt auch dem 
    /// Display.
    /// </summary>
    public abstract class FrameBase : IDisposable, IRenderable
    {
        public FrameBase()
        {
            ElementList = new List<IFrameable>();
        }

        public abstract void Dispose();

        protected List<IFrameable> ElementList { get; private set; }

        public virtual IFrameable[] Elements { get { return ElementList.ToArray(); } }

        public virtual void Add(IFrameable element)
        {
            if (!ElementList.Contains(element))
                ElementList.Add(element);
        }

        public virtual void Remove(IFrameable element)
        {
            ElementList.Remove(element);
        }

        public abstract void Show();

        public virtual Color BackgroundColor { get; set; }

        public virtual Color ForegroundColor { get; set; }

        public virtual string ImagePath { get; set; }
    }

    #region Interfaces

    
    /// <summary>
    /// Alle Elemente, die man zusammen gruppieren kann.
    /// </summary>
    public interface IFrameable
    {

    }

    /// <summary>
    /// Alle Elemente, die sich relativ (!) positionieren lassen.
    /// </summary>
    public interface IPositionable
    {
        float X { get; set; }

        float Y { get; set; }

        float Width { get; set; }

        float Height { get; set; }
    }

    /// <summary>
    /// Alle Elemente, die sich zeichnen lassen.
    /// </summary>
    public interface IRenderable
    {
        Color BackgroundColor { get; set; }

        Color ForegroundColor { get; set; }

        string ImagePath { get; set; }
    }

    #endregion
}
