using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StediApp
{
    public class Constants
    {
        public const string DatabaseFile = "StedibaSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath 
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(basePath, DatabaseFile);
            }
        }
    }
}
