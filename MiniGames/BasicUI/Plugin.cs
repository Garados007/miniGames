using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniGamesInterface;
using MiniGamesInterface.UI;

namespace BasicUI
{
    public class Plugin : UIPluginBase
    {
        public override string Description
        {
            get
            {
                return "UI module with basic functions";
            }
        }

        public override string Name
        {
            get
            {
                return "Basic UI";
            }
        }

        public override UIFactory[] GetAllUIs()
        {
            return new UIFactory[]
            {
                new BasicUIFactory()
            };
        }
    }
}
