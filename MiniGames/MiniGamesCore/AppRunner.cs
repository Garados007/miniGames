using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGamesCore
{
    public static class AppRunner
    {
        public static bool EnableAppRun = true;

        public static void Main(string[] args)
        {
            //test db
            using (var query = DB.Create("select * from sqlite_master"))
            using (var reader = query.ExecuteReader()) ;
                //detect Plugins
            if (!Plugin.PluginManager.Search()) return; //no runable Plugins found
            if (Plugin.PluginManager.UIPlugins.Count == 0 || //no menu found
                Plugin.PluginManager.DisplayPlugins.Count == 0) return; //no displays found
            //prepair Plugins
            Plugin.PluginManager.CreateState();
            Plugin.PluginManager.RunAutostart();

            //Render Loop
            while (EnableAppRun)
            {
                RuntimeSettings.CurrentDisplay.Show();
            }


            //Close
            DB.Close();
        }
    }
}
