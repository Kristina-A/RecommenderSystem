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
                    "localhost", "5432", "postgres",
                    "diplomski", "webshop");
            conn = new NpgsqlConnection(connstring);
            conn.Open();
        }

        public void ViewProduct(string userID, string prodID)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into viewedproducts values (@t,@u,@p)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.Parameters.Add(new NpgsqlParameter("@u", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@p", prodID));

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }

        public void BuyProduct(string userID, string prodID, double price)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into boughtproducts values (@t,@u,@p,@price)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.Parameters.Add(new NpgsqlParameter("@u", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@p", prodID));
            cmd.Parameters.Add(new NpgsqlParameter("@price", price));

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }

        public void ReviewProduct(string userID, string prodID, int grade)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into reviewedproducts values (@t,@u,@p,@grade)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.Parameters.Add(new NpgsqlParameter("@u", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@p", prodID));
            cmd.Parameters.Add(new NpgsqlParameter("@grade", grade));

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }

        public void SeeReviews(string userID, string prodID)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into seenreviews values (@t,@u,@p)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.Parameters.Add(new NpgsqlParameter("@u", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@p", prodID));

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }

        public void SendNotification(string userID, string notID)
        {

            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into notifications values (@t,@u,@p)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.Parameters.Add(new NpgsqlParameter("@u", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@p", notID));

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            conn.Close();
        }

        public List<string> GetNotifications(string userID)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from notifications where userid=@id and time>=@t";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddDays(-7)));
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();
            conn.Close();

            List<string> notifications = new List<string>();

            return notifications;
        }
    }
}
