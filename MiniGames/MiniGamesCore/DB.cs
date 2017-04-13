using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SQLite;
using System.Collections;

namespace MiniGamesCore
{
    public static class DB
    {
        static DB()
        {
            Open();
        }

        static SQLiteFactory factory;
        public static SQLiteConnection Connection { get; private set; }
        static object lockObject = new object();
        static bool SetPassword = !System.Diagnostics.Debugger.IsAttached && false;

        public static void Open()
        {
            factory = new SQLiteFactory();
            Connection = (SQLiteConnection)factory.CreateConnection();
            Connection.ConnectionString = "Data Source=" + "MiniGames.db";
            if (SetPassword) Connection.SetPassword(GetPassword());
            Connection.Open();
            if (!CheckValidity())
                CreateTables();
        }

        static string GetPassword()
        {
            const string pb = "oasdhfqwe%/(&/)=()?ui9zhr 9q2uzftzd rset5s5oiz7ed34aqr99012u3";
            var temp1 = "";
            var temp2 = "";
            for (var i = 0; i < pb.Length; i++)
                if ((i & 1) == 0) temp1 += pb[i];
                else temp2 += pb[i];
            var finder = new PrimFinder();
            finder.MoveNext(); //skip the 2
            var border = Math.Min(temp1.Length, temp2.Length);
            var temp = "";
            for (var i = 0; i < border; ++i)
            {
                finder.MoveNext();
                var num = (finder.Current & (~1)) >> 1;
                if ((num & 1) == 0) temp += temp1[i];
                else temp += temp2[i];
            }
            finder.Dispose(); //pw=  asfwe//=(uiz q2ft sesoi7dar90u
            return temp;
        }

        class PrimFinder : IEnumerator<int>
        {
            public PrimFinder()
            {
                Current = 1;
            }

            public int Current { get; private set; }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                var c = Current + 1;
                while (c > Current)
                {
                    bool isPrim = true;
                    var border = Math.Min(c - 1, (int)Math.Floor(Math.Sqrt(c)));
                    for (int i = 2; i <= border; ++i)
                        if ((c % i) == 0)
                        {
                            isPrim = false;
                            break;
                        }
                    if (isPrim)
                    {
                        Current = c;
                        return true;
                    }
                    c++;
                }
                return false;
            }

            public void Reset()
            {
                Current = 1;
            }
        }

        static bool CheckValidity()
        {
            using (var query = Create(Properties.Resources.CheckTables))
            using (var reader = query.ExecuteReader())
            {
                if (!reader.Read()) return false;
                return reader.GetInt32(0) == 0;
            }
        }

        static void CreateTables()
        {
            using (var query = Create(Properties.Resources.CreateTables))
                query.ExecuteNonQuery();
            if (SetPassword) Connection.ChangePassword(GetPassword());
        }

        public static Query Create(string sql)
        {
            return new Query(Connection, sql);
        }

        public static void Execute(Action method)
        {
            bool NeedWait = false;
            lock (lockObject)
            {
                if (BigTransaction != null && !TransactionThread.Equals(Thread.CurrentThread))
                    NeedWait = true;
            }
            if (NeedWait)
            {
                Mutex.WaitOne();
            }
            lock (lockObject)
            {
                if (NeedWait) Mutex.ReleaseMutex();
                if (BigTransaction == null)
                    using (var transaction = Connection.BeginTransaction())
                    {
                        method();
                        transaction.Commit();
                    }
                else method();
            }
        }

        static Mutex Mutex = new Mutex(false);
        static SQLiteTransaction BigTransaction = null;
        static Thread TransactionThread;
        static object lockCheckThread = new object();

        public static void BeginTransaction()
        {
            Mutex.WaitOne();
            lock (lockObject)
            {
                BigTransaction = Connection.BeginTransaction();
                TransactionThread = Thread.CurrentThread;
            }
        }

        public static void StopTransaction()
        {
            lock (lockObject)
            {
                if (BigTransaction == null && TransactionThread != Thread.CurrentThread) return;
                BigTransaction.Commit();
                BigTransaction = null;
                TransactionThread = null;
            }
            Mutex.ReleaseMutex();
        }

        public static void Close()
        {
            factory.Dispose();
            Connection.Dispose();
        }
    }

    public class Query : IDisposable
    {
        public SQLiteCommand Command { get; set; }

        public List<SQLiteParameter> Parameter { get; set; }

        public Query(SQLiteConnection connection, string sql)
        {
            Command = new SQLiteCommand(connection);
            Command.CommandText = sql;
            var count = sql.Count((c) => c == '?');
            Parameter = new List<SQLiteParameter>(count);
            for (int i = 0; i < count; ++i)
                Parameter.Add(new SQLiteParameter());
            Command.Parameters.AddRange(Parameter.ToArray());
        }

        public object this[int index]
        {
            get { return Parameter[index].Value; }
            set
            {
                if (index < 0 || index >= Parameter.Count) return;
                Parameter[index].Value = value;
            }
        }

        public void SetValues(params object[] values)
        {
            for (int i = 0; i < values.Length; ++i)
                this[i] = values[i];
        }

        public int ExecuteNonQuery()
        {
            int result = 0;
            DB.Execute(() => result = Command.ExecuteNonQuery());
            return result;
            //return Command.ExecuteNonQuery();
        }

        public SQLiteDataReader ExecuteReader()
        {
            SQLiteDataReader reader = null;
            DB.Execute(() => reader = Command.ExecuteReader());
            return reader;
            //return Command.ExecuteReader();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
                }

                // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
                // TODO: große Felder auf Null setzen.

                Command.Dispose();
                Command = null;
                Parameter.Clear();
                disposedValue = true;
            }
        }

        // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
        // ~Query() {
        //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
        //   Dispose(false);
        // }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
            // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
