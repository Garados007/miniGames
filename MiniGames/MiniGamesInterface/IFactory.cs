using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesInterface
{
    public interface IFactory
    {
        string Name { get; }

        string DisplaySymbolFile { get; }
    }

    public abstract class PluginMode : IFactory
    {
        public abstract string DisplaySymbolFile { get; }
        public abstract string Name { get; }

        public bool Enabled { get; set; }

        public bool AutoRun { get; set; }
    }
}
