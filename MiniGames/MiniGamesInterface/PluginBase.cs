using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface
{
    public abstract class PluginBase
    {
        public virtual PluginType Type { get; }

        public string Name { get; }

        public string Description { get; }


    }

    public enum PluginType
    {
        GraphicRenderer,
        Game
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


    }

    public abstract class GamePluginBase : PluginBase
    {

    }

    
}
