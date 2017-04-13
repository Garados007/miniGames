using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGamesInterface.Display;
using MiniGamesInterface.Game;
using MiniGamesInterface.UI;

namespace MiniGamesCore
{
    static class RuntimeSettings
    {
        private static DisplayFactory currentDisplayFactory = null;
        public static DisplayFactory CurrentDisplayFactory
        {
            get
            {
                return currentDisplayFactory;
            }
            set
            {
                currentDisplayFactory = value;
                CurrentDisplayFactoryChanged?.Invoke();
            }
        }

        public static DisplayBase CurrentDisplay { get; set; }
        public static GameBase  CurrentGame { get; set; }
        public static UIBase CurrentUI { get; set; }

        public static Plugin.FactoryWrapper CurrentDisplayFactoryWrapper = null;
        public static Plugin.FactoryWrapper CurrentGameFactoryWrapper = null;
        public static Plugin.FactoryWrapper CurrentUIFactoryWrapper = null;



        public static event Action CurrentDisplayFactoryChanged;
    }
}
