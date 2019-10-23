using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlawHess
{
    class dbConnection
    {
        /// <summary>
        /// Database Connection
        /// to myDB.accdb
        /// </summary>
        public static string dbConnect = @"Provider=Microsoft.ACE.OLEDB.16.0;"
            + "Data Source="
            + Directory.GetCurrentDirectory()//relative reference to file location
            + @"\OutlawHessDb.accdb;"; //name of database

    }
}
