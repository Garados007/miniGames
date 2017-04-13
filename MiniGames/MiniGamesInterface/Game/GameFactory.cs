using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface.Game
{
    public abstract class GameFactory
    {
        public abstract string GameName { get; }

        public abstract string GameSymbolFile { get; }

        public abstract GameBase CreateGame();
    }

    public abstract class GameBase
    {
        public Display.DisplayFactory CurrentDisplay { get; set; }

        public abstract void Run();

        public abstract void Pause();

        public abstract void Stop();
    }
}
