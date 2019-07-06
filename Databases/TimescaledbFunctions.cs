using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace Databases
{
    public class TimescaledbFunctions
    {
        NpgsqlConnection conn;
        NpgsqlDataAdapter da;
        DataSet ds;
        DataTable dt;

        public TimescaledbFunctions()
        {
            ds = new DataSet();
            dt = new DataTable();
            string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "127.0.0.1", "5432", "postgres",
                    "diplomski", "webshop");
            conn = new NpgsqlConnection(connstring);
            conn.Open();
        }
    }
}
