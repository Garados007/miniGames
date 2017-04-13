using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGamesInterface.Display;

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

        public static event Action CurrentDisplayFactoryChanged;
    }
}
