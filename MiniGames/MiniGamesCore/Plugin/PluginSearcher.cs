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
        Query fetchInfo, addInfo, enabled, fetchId;

        public PluginWrapper[] Search()
        {
            using (enabled = DB.Create("WITH d AS (SELECT Enable FROM PluginFiles WHERE Path=?), "+
                "e AS (SELECT COUNT(*) c FROM d WHERE Enable = 1), " +
                "a AS (SELECT COUNT(*) c FROM d) " +
                "SELECT(CASE WHEN a.c = 0 THEN - 1 ELSE e.c END) " +
                "FROM e, a"))
            using (fetchInfo = DB.Create("SELECT Id,PluginType,Name,Description,Enable FROM PluginFiles WHERE Path=?"))
            using (fetchId = DB.Create("SELECT Id FROM PluginFiles WHERE Path=? AND NETType=?"))
            using (addInfo = DB.Create("INSERT OR IGNORE INTO PluginFiles (Path,NETType,PluginType,Name,Description) VALUES (?,?,?,?,?)"))
            {
                var l = new List<PluginWrapper>();
                var d = new DirectoryInfo(Environment.CurrentDirectory + "\\Plugins");
                if (d.Exists) SearchDir(d, l);
                return l.ToArray();
            }
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
            enabled.SetValues(f.FullName);
            using (var r = enabled.ExecuteReader())
            {
                r.Read();
                if (r.GetInt32(0) == 0)
                {
                    fetchInfo.SetValues(f.FullName);
                    using (var r2 = fetchInfo.ExecuteReader())
                        while (r2.Read())
                            l.Add(new PluginWrapper()
                            {
                                Plugin = null,
                                FileName = f.FullName,
                                SysEnableLoad = false,
                                Id = r2.GetInt32(0),
                                SysType = (PluginType)Enum.Parse(typeof(PluginType), r2.GetString(1)),
                                SysName = r2.IsDBNull(2) ? null : r2.GetString(2),
                                SysDescription = r2.IsDBNull(3) ? null : r2.GetString(3),
                                SysLoaded = false
                            });
                }
                else
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
                                addInfo.SetValues(f.FullName, t.ToString(), o.Type.ToString(), o.Name, o.Description);
                                addInfo.ExecuteNonQuery();
                                fetchId.SetValues(f.FullName, t.ToString());
                                int id;
                                using (var r2 = fetchId.ExecuteReader())
                                {
                                    r2.Read();
                                    id = r2.GetInt32(0);
                                }
                                l.Add(new PluginWrapper()
                                {
                                    Plugin = o,
                                    FileName = f.FullName,
                                    SysEnableLoad = true,
                                    SysLoaded = true,
                                    Id = id
                                });
                            }
                        }
                    }
                    catch { }
                }
            }
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
            var l = new List<PluginHandle>(DisplayPlugins.Count + GamePlugins.Count + UIPlugins.Count + UnloadedPlugins.Count);
            l.AddRange(DisplayPlugins);
            l.AddRange(GamePlugins);
            l.AddRange(UIPlugins);
            l.AddRange(UnloadedPlugins);
            State.AllPlugins = l.ToArray();
            using (var get = DB.Create("SELECT Id,Autorun,Enabled FROM PluginFactorys WHERE Plugin=? AND NETType=?"))
            using (var set = DB.Create("INSERT INTO PluginFactorys(Plugin,NETType,Autorun,Enabled) VALUES (?,?,0,1)"))
            using (var getid = DB.Create("SELECT Id FROM PluginFactorys WHERE Plugin=? AND NETType=?"))
            {
                State.AllDisplays = convertFactorys(get, set, getid, (p) => (p as GraphicPluginBase).GetAllDisplays(), DisplayPlugins);
                State.AllGames = convertFactorys(get, set, getid, (p) => (p as GamePluginBase).GetAllGames(), GamePlugins);
                State.AllUIs = convertFactorys(get, set, getid, (p) => (p as UIPluginBase).GetAllUIs(), UIPlugins);
                State.RunningGames = new IFactory[0];
            }
        }

        public static void RunAutostart()
        {
            var display = FirstAutostart(State.AllDisplays);
            if (display == null) { if (State.AllDisplays.Length > 0) State.StartDisplay(State.AllDisplays[0]); }
            else State.StartDisplay(display);
            var ui = FirstAutostart(State.AllUIs);
            if (ui == null) { if (State.AllUIs.Length > 0) State.StartUI(State.AllUIs[0]); }
            else State.StartUI(ui);
            foreach (var f in State.AllGames)
                if (f is FactoryWrapper && ((FactoryWrapper)f).AutoRun)
                    State.StartGame(f);
        }

        static FactoryWrapper FirstAutostart(IFactory[] list)
        {
            foreach (var f in list)
                if (f is FactoryWrapper && ((FactoryWrapper)f).AutoRun)
                    return (FactoryWrapper)f;
            return null;
        }

        static IFactory[] convertFactorys(Query get, Query set, Query getid, Func<PluginBase, IFactory[]> fetch, List<PluginWrapper> source)
        {
            List<IFactory> fl = new List<IFactory>();
            foreach (var p in source)
                foreach (var f in fetch(p.Plugin))
                {
                    get.SetValues(p.Id, f.GetType().ToString());
                    int id; bool autorun, enabled;
                    using (var r = get.ExecuteReader())
                        if (r.Read())
                        {
                            id = r.GetInt32(0);
                            autorun = r.GetInt32(1) != 0;
                            enabled = r.GetInt32(2) != 0;
                        }
                        else
                        {
                            set.SetValues(p.Id, f.GetType().ToString());
                            set.ExecuteNonQuery();
                            getid.SetValues(p.Id, f.GetType().ToString());
                            using (var r2 = getid.ExecuteReader())
                            {
                                r2.Read();
                                id = r2.GetInt32(0);
                                autorun = false;
                                enabled = true;
                            }
                        }
                    fl.Add(new FactoryWrapper()
                    {
                        Factory = f,
                        SysAutoRun = autorun,
                        SysEnabled = enabled,
                        Id = id
                    });
                }
            return fl.ToArray();
        }
    }

    class PluginWrapper : PluginHandle
    {
        public PluginBase Plugin;

        public string FileName;

        public int Id;

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
        public object Tag;
        public bool SysAutoRun;
        public bool SysEnabled;
        public int Id;

        public override bool AutoRun
        {
            get
            {
                return SysAutoRun;
            }

            set
            {
                SysAutoRun = value;
                using (var query = DB.Create("UPDATE PluginFactorys SET Autorun=? WHERE Id=?"))
                {
                    query.SetValues(value ? 1 : 0, Id);
                    query.ExecuteNonQuery();
                }
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
                using (var query = DB.Create("UPDATE PluginFactorys SET Enabled=? WHERE Id=?"))
                {
                    query.SetValues(value ? 1 : 0, Id);
                    query.ExecuteNonQuery();
                }
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
            if (!(display is FactoryWrapper)) throw new ArgumentException("display", "wrong factory type");
            if (display != RuntimeSettings.CurrentDisplayFactoryWrapper)
            {
                var old = RuntimeSettings.CurrentDisplay;
                RuntimeSettings.CurrentDisplayFactoryWrapper = (FactoryWrapper)display;
                RuntimeSettings.CurrentDisplay = ((MiniGamesInterface.Display.DisplayFactory)RuntimeSettings.CurrentDisplayFactoryWrapper.Factory)
                    .CreateDisplay();
                RuntimeSettings.CurrentDisplayFactory = (MiniGamesInterface.Display.DisplayFactory)RuntimeSettings.CurrentDisplayFactoryWrapper.Factory;
                PluginManager.State.CurrentDisplay = display;
                old?.Hide();
                old?.Dispose();
            }
        }

        public override void StartGame(IFactory game)
        {
            if (!(game is FactoryWrapper)) throw new ArgumentException("game", "wrong factory type");
            if (game != RuntimeSettings.CurrentGameFactoryWrapper)
            {
                RuntimeSettings.CurrentGame?.Pause();
                RuntimeSettings.CurrentGameFactoryWrapper = (FactoryWrapper)game;
                bool found = false;
                for (var i = 0; i<PluginManager.State.RunningGames.Length; ++i)
                    if (PluginManager.State.RunningGames[i] == game)
                    {
                        found = true;
                        RuntimeSettings.CurrentGame = (MiniGamesInterface.Game.GameBase)RuntimeSettings.CurrentGameFactoryWrapper.Tag;
                        break;
                    }
                if (!found)
                {
                    RuntimeSettings.CurrentGame = ((MiniGamesInterface.Game.GameFactory)RuntimeSettings.CurrentGameFactoryWrapper.Factory)
                        .CreateGame();
                    RuntimeSettings.CurrentGame.CurrentDisplay = new Middleware.DisplayFactoryWrapper();
                    var l = PluginManager.State.RunningGames.ToList();
                    l.Add(game);
                    PluginManager.State.RunningGames = l.ToArray();
                    RuntimeSettings.CurrentGameFactoryWrapper.Tag = RuntimeSettings.CurrentGame;
                }
            }
            PluginManager.State.CurrentGame = game;
            RuntimeSettings.CurrentUI.Hide();
            RuntimeSettings.CurrentGame.Run();
        }

        public override void StartUI(IFactory ui)
        {
            if (!(ui is FactoryWrapper)) throw new ArgumentException("ui", "wrong factory type");
            if (ui != RuntimeSettings.CurrentUIFactoryWrapper)
            {
                if (RuntimeSettings.CurrentUI != null)
                {
                    RuntimeSettings.CurrentUI.Hide();
                    RuntimeSettings.CurrentUI.Dispose();
                }
                RuntimeSettings.CurrentUIFactoryWrapper = (FactoryWrapper)ui;
                RuntimeSettings.CurrentUI = ((MiniGamesInterface.UI.UIFactory)RuntimeSettings.CurrentUIFactoryWrapper.Factory)
                    .CreateUI(PluginManager.State);
                RuntimeSettings.CurrentUI.CurrentDisplay = new Middleware.DisplayFactoryWrapper();

                PluginManager.State.CurrentUI = ui;
            }
            PluginManager.State.CurrentGame = null;
            RuntimeSettings.CurrentGame?.Pause();
            RuntimeSettings.CurrentUI.Show();
        }

        public new IFactory[] AllDisplays
        {
            get
            {
                return base.AllDisplays;
            }
            set
            {
                base.AllDisplays = value;
            }
        }

        public new IFactory[] AllGames
        {
            get
            {
                return base.AllGames;
            }

            set
            {
                base.AllGames = value;
            }
        }

        public new PluginHandle[] AllPlugins
        {
            get
            {
                return base.AllPlugins;
            }

            set
            {
                base.AllPlugins = value;
            }
        }

        public new IFactory[] AllUIs
        {
            get
            {
                return base.AllUIs;
            }

            set
            {
                base.AllUIs = value;
            }
        }

        public new IFactory CurrentDisplay
        {
            get
            {
                return base.CurrentDisplay;
            }

            set
            {
                base.CurrentDisplay = value;
            }
        }

        public new IFactory CurrentGame
        {
            get
            {
                return base.CurrentGame;
            }

            set
            {
                base.CurrentGame = value;
            }
        }

        public new IFactory CurrentUI
        {
            get
            {
                return base.CurrentUI;
            }

            set
            {
                base.CurrentUI = value;
            }
        }

        public new IFactory[] RunningGames
        {
            get
            {
                return base.RunningGames;
            }

            set
            {
                base.RunningGames = value;
            }
        }
    }
}
