using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface.UI
{
    public abstract class UIFactory : IFactory
    {
        public abstract string Name { get; }

        public abstract string DisplaySymbolFile { get; }

        public abstract UIBase CreateUI(CurrentState currentState);
    }

    public abstract class UIBase : IDisposable
    {
        public Display.DisplayFactory CurrentDisplay { get; set; }

        public CurrentState CurrentState { get; private set; }

        public UIBase(CurrentState currentState)
        {
            CurrentState = currentState;
        }

        public abstract void Show();

        public abstract void Hide();

        public abstract void Dispose();
    }

    public abstract class CurrentState
    {
        public virtual IFactory[] AllDisplays { get; protected set; }

        public virtual IFactory[] AllGames { get; protected set; }

        public virtual IFactory[] AllUIs { get; protected set; }

        public virtual PluginHandle[] AllPlugins { get; protected set; }

        public virtual IFactory CurrentDisplay { get; protected set; }

        public virtual IFactory CurrentGame { get; protected set; }

        public virtual IFactory[] RunningGames { get; protected set; }

        public virtual IFactory CurrentUI { get; protected set; }

        public abstract void StartDisplay(IFactory display);

        public abstract void StartGame(IFactory game);

        public abstract void StartUI(IFactory ui);
    }
}
