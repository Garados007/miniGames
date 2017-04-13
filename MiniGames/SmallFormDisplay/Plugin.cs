using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGamesInterface;
using MiniGamesInterface.Display;

namespace SmallFormDisplay
{
    public class Plugin : GraphicPluginBase
    {
        public Plugin()
        {

        }

        public override string Description
        {
            get
            {
                return "Small and light Win Form";
            }
        }

        public override string Name
        {
            get
            {
                return "Small Form Display Plugin";
            }
        }

        public override MiniGamesInterface.Display.DisplayFactory[] GetAllDisplays()
        {
            return new MiniGamesInterface.Display.DisplayFactory[]
            {
                new DisplayFactory()
            };
        }


    }
}
