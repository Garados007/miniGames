using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using MiniGamesInterface;

namespace MiniGamesCore.Plugin
{
    class PluginSearcher
    {
        public PluginWrapper[] Search()
        {
            var l = new List<PluginWrapper>();
            var d = new DirectoryInfo(Environment.CurrentDirectory + "\\Plugins");
            if (d.Exists) SearchDir(d, l);
            return l.ToArray();
        }

        void SearchDir(DirectoryInfo d, List<PluginWrapper> l)
        {
            foreach (var sd in d.GetDirectories()) SearchDir(sd, l);
            foreach (var f in d.GetFiles())
                if (f.Extension.ToLower() == ".dll")
                    SearchFile(f, l);
        }

        void SearchFile(FileInfo f, List<PluginWrapper> l)
        {
            try
            {
                var ass = Assembly.LoadFile(f.FullName);
                var type = typeof(PluginBase);
                foreach (var t in ass.GetTypes())
                {
                    if (t.IsSubclassOf(type))
                    {
                        var o = (PluginBase)Activator.CreateInstance(t);
                        l.Add(new PluginWrapper()
                        {
                            Plugin = o,
                            FileName = f.FullName
                        });
                    }
                }
            }
            catch { }
        }
    }

    static class PluginManager
    {
        public static List<PluginWrapper> DisplayPlugins = new List<PluginWrapper>();

        public static List<PluginWrapper> GamePlugins = new List<PluginWrapper>();

        public static List<PluginWrapper> UIPlugins = new List<PluginWrapper>();

        public static List<PluginWrapper> UnloadedPlugins = new List<PluginWrapper>();

        public static bool Search()
        {
            var l = new PluginSearcher().Search();
            int added = 0;
            foreach (var p in l)
            {
                if (!p.Loaded)
                {
                    UnloadedPlugins.Add(p);
                    //no added++, 'cause it's not started
                }
                else if (p.Type == PluginType.GraphicRenderer)
                {
                    DisplayPlugins.Add(p);
                    added++;
                }
                else if (p.Type == PluginType.Game)
                {
                    GamePlugins.Add(p);
                    added++;
                }
                else if (p.Type == PluginType.UI)
                {
                    UIPlugins.Add(p);
                    added++;
                }
            }
            return added != 0;
        }

        public static PluginStates State;

        public static void CreateState()
        {
            State = new PluginStates();
        }
    }

    class PluginWrapper : PluginHandle
    {
        public PluginBase Plugin;

        public string FileName;

        public PluginType SysType;

        public string SysName;

        public string SysDescription;

        public bool SysEnableLoad;

        public bool SysLoaded;

        public override PluginType Type
        {
            get
            {
                return Plugin?.Type ?? SysType;
            }
        }

        public override string Name
        {
            get
            {
                return Plugin?.Name ?? SysName;
            }
        }

        public override string Description
        {
            get
            {
                return Plugin?.Description ?? SysDescription;
            }
        }

        public override bool EnableLoad
        {
            get
            {
                return SysEnableLoad;
            }

            set
            {
                SysEnableLoad = value;
            }
        }

        public override bool Loaded
        {
            get
            {
                return SysLoaded;
            }
        }
    }

    class FactoryWrapper : PluginMode
    {
        public IFactory Factory;
        public bool SysAutoRun;
        public bool SysEnabled;

        public override bool AutoRun
        {
            get
            {
                return SysAutoRun;
            }

            set
            {
                SysAutoRun = value;
            }
        }

        public override string DisplaySymbolFile
        {
            get
            {
                return Factory.DisplaySymbolFile;
            }
        }

        public override bool Enabled
        {
            get
            {
                return SysEnabled;
            }

            set
            {
                SysEnabled = value;
            }
        }

        public override string Name
        {
            get
            {
                return Factory.Name;
            }
        }
    }

    class PluginStates : MiniGamesInterface.UI.CurrentState
    {
        public override void StartDisplay(IFactory display)
        {
            throw new NotImplementedException();
        }

        public override void StartGame(IFactory game)
        {
            throw new NotImplementedException();
        }

        public override void StartUI(IFactory ui)
        {
            throw new NotImplementedException();
        }
    }
}
