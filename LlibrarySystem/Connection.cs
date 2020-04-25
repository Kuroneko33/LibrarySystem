using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    class Connection
    {
        private static string fullpath = System.IO.Directory.GetCurrentDirectory();
        private static string path = fullpath.Substring(0, fullpath.IndexOf("bin"));
        public static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}LibraryDB.mdf;Integrated Security=True";
    }
}
