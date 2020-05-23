using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlibrarySystem
{
    static class Connection
    {
        private static string fullpath = System.IO.Directory.GetCurrentDirectory();
        private static int index = fullpath.IndexOf("bin");
        private static string path = fullpath + "\\";
        public static string connectionString { get; }
        static Connection()
        {
            if (index!=-1)
            {
                path = fullpath.Substring(0, index);
            }
            connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}LibraryDB.mdf;Integrated Security=True";
        }
    }
}
