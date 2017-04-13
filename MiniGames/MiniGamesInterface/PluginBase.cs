using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface
{
    public abstract class PluginBase
    {
        public abstract PluginType Type { get; }

        public string Name { get; }

        public string Description { get; }


    }

    public enum PluginType
    {
        GraphicRenderer,
        Game,
        UI
    }

    public abstract class GraphicPluginBase : PluginBase
    {
        public sealed override PluginType Type
        {
            get
            {
                return PluginType.GraphicRenderer;
            }
        }

        public abstract Display.DisplayFactory[] GetAllDisplays();
    }

    public abstract class GamePluginBase : PluginBase
    {
        public sealed override PluginType Type
        {
            get
            {
                return PluginType.Game;
            }
        }

        public abstract Game.GameFactory[] GetAllGames();
    }

    public abstract class UIPluginBase : PluginBase
    {
        public sealed override PluginType Type
        {
            get
            {
                return PluginType.UI;
            }
        }

        public abstract UI.UIFactory[] GetAllUIs();
    }

    public abstract class PluginHandle
    {
        public abstract PluginType Type { get;}

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract bool EnableLoad { get; set; }

        public abstract bool Loaded { get; }
    }
}
