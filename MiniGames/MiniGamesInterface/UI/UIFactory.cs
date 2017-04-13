using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface.UI
{
    public abstract class UIFactory
    {
        public abstract string Name { get; }

        public abstract string DisplaySymbolFile { get; }

        public abstract UIBase CreateUI(CurrentState currentState);
    }

    public abstract class UIBase : IDisposable
    {
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
        public IFactory[] AllDisplays { get; protected set; }

        public IFactory[] AllGames { get; protected set; }

        public IFactory[] AllUIs { get; protected set; }

        public PluginHandle[] AllPlugins { get; protected set; }

        public IFactory CurrentDisplay { get; protected set; }

        public IFactory CurrentGame { get; protected set; }

        public IFactory[] RunningGames { get; protected set; }

        public IFactory CurrentUI { get; protected set; }

        public abstract void StartDisplay(IFactory display);

        public abstract void StartGame(IFactory game);

        public abstract void StartUI(IFactory ui);
    }
}
