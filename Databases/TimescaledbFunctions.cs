using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using MongoDB.Bson;

namespace Databases
{
    public class TimescaledbFunctions
    {
        NpgsqlConnection conn;
        NpgsqlDataAdapter da;
        DataTable dt;

        public TimescaledbFunctions()
        {
            string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "localhost", "5432", "postgres",
                    "diplomski", "webshop");
            conn = new NpgsqlConnection(connstring);
            conn.Open();
        }

        public void CloseConnection()
        {
            conn.Close();
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
        }

        public void SendNotification(string userID, string notID, string tag)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into notifications values (@t,@u,@n,@tag)";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.Parameters.Add(new NpgsqlParameter("@u", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@n", notID));
            cmd.Parameters.Add(new NpgsqlParameter("@tag", tag));

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public List<string> GetNotifications(string userID)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select notificationid from notifications where userid=@id and time>=@t";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddDays(-7)));
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();

            List<string> notifications = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                notifications.Add(dr["notificationid"].ToString());
            }

            return notifications;
        }

        public bool NotificationSent(string userID)//provera za notifikacije koje jednom nedeljno obavestavaju za mali iznos do popusta
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from notifications where userid=@id and time>@t and tag='do_popusta'";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddDays(-7)));
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public double MonthShopping(string userID)//vraca ukupan iznos kupovina u proteklih mesec dana
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText ="select last(time,time) from notifications where userid=@id and (tag='popust' or tag='iskorisceno')"; // ako je vec dobio popust, gleda se od dobijenog na dalje u toku mesec dana
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));           
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows[0]["last"].GetType() !=typeof(System.DBNull))
            {
                cmd.CommandText = "select sum(price) as total from boughtproducts where userid=@id and time>@t and time>@tn";
                cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddMonths(-1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tn", dt.Rows[0]["last"]));
            }
            else
            {
                cmd.CommandText = "select sum(price) as total from boughtproducts where userid=@id and time>@t";
                cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddMonths(-1)));
            }
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();

            if (dt.Rows[0]["total"].GetType() == typeof(System.DBNull))
                return 0;
            else
                return ((double)dt.Rows[0]["total"]);
        }

        public int LowGrades(string userID)// koliko ocena manjih od 3 u poslednjih mesec dana, samo za kupljene proizvode (kupljeni max pre 2 meseca)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select last(time,time) from notifications where userid=@id and tag='lose_ocene'";           
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));          
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if(dt.Rows[0]["last"].GetType() != typeof(System.DBNull))
            {
                cmd.CommandText = "select count(grade) as grades from (select distinct(reviewedproducts.productid), grade" +
                    " from reviewedproducts inner join boughtproducts on " +
                    "(reviewedproducts.userid=boughtproducts.userid and reviewedproducts.productid=boughtproducts.productid) " +
                    "where reviewedproducts.userid=@id and reviewedproducts.time>@t and reviewedproducts.time>@tn" +
                    " and grade<3 and boughtproducts.time>@tb) as subgrades";
                cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddMonths(-1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tb", DateTime.Now.AddMonths(-2)));
                cmd.Parameters.Add(new NpgsqlParameter("@tn", dt.Rows[0]["last"]));
            }
            else
            {
                cmd.CommandText = "select count(grade) as grades from (select distinct(reviewedproducts.productid), grade" +
                    " from reviewedproducts inner join boughtproducts on " +
                    "(reviewedproducts.userid=boughtproducts.userid and reviewedproducts.productid=boughtproducts.productid) " +
                    "where reviewedproducts.userid=@id and reviewedproducts.time>@t and grade<3 and boughtproducts.time>@tb) as subgrades";
                cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddMonths(-1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tb", DateTime.Now.AddMonths(-2)));
            }
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();

            return (int.Parse(dt.Rows[0]["grades"].ToString()));
        }

        public void UpdateNotification(string userID, string notID, string tag)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "update notifications set time=@t, tag=@tag where userid=@id and notificationid=@nid";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@tag", tag));
            cmd.Parameters.Add(new NpgsqlParameter("@nid", notID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void UpdateReview(string userID, string prodID, int grade)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "update reviewedproducts set time=@t, grade=@grade where userid=@id and productid=@pid";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@grade", grade));
            cmd.Parameters.Add(new NpgsqlParameter("@pid", prodID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now));
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public List<string> GetDiscounts(string userID)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select notificationid from notifications where userid=@id and time>@t and (tag='popust' or tag='l_popust')";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddDays(-7)));
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();

            List<string> discounts = new List<string>();

            foreach (DataRow dr in dt.Rows)
            {
                discounts.Add(dr["notificationid"].ToString());
            }

            return discounts;
        }

        public bool ActivatedDiscount(string userID)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select notificationid from notifications where userid=@id and time>@t and tag='l_popust'";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@id", userID));
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddDays(-7)));
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            cmd.Dispose();

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public List<DomainModel.RecommenderAction> GetMonthlyActivities(int months)
        {
            List<DomainModel.RecommenderAction> actions = new List<DomainModel.RecommenderAction>();

            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select userid, productid from viewedproducts where time>@t";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddMonths(-1*months)));
            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            foreach(DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "View";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());

                actions.Add(action);
            }

            cmd.CommandText = "select userid, productid, grade from reviewedproducts where time>@t";
            dt.Clear();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "Review";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());
                action.Grade = int.Parse(dr["grade"].ToString());

                actions.Add(action);
            }

            cmd.CommandText = "select userid, productid from boughtproducts where time>@t";
            dt.Clear();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "Buy";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());

                actions.Add(action);
            }

            cmd.CommandText = "select userid, productid from seenreviews where time>@t";
            dt.Clear();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "SeeReviews";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());

                actions.Add(action);
            }

            cmd.Dispose();

            return actions;
        }

        public List<DomainModel.RecommenderAction> GetWeeklyActivities()
        {
            List<DomainModel.RecommenderAction> actions = new List<DomainModel.RecommenderAction>();

            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select userid, productid from viewedproducts where time>@t union select userid, productid from viewedproducts where " +
                "time>=@tp and time<=@tk";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new NpgsqlParameter("@t", DateTime.Now.AddDays(-21)));
            if (DateTime.Now.Month >= 3 && DateTime.Now.Month <= 5)
            {
                cmd.Parameters.Add(new NpgsqlParameter("@tp", new DateTime(DateTime.Now.Year - 1, 3, 1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tk", new DateTime(DateTime.Now.Year - 1, 5, 31)));
            }
            else if(DateTime.Now.Month >= 6 && DateTime.Now.Month <= 8)
            {
                cmd.Parameters.Add(new NpgsqlParameter("@tp", new DateTime(DateTime.Now.Year - 1, 6, 1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tk", new DateTime(DateTime.Now.Year - 1, 8, 31)));
            }
            else if(DateTime.Now.Month >= 9 && DateTime.Now.Month <= 11)
            {
                cmd.Parameters.Add(new NpgsqlParameter("@tp", new DateTime(DateTime.Now.Year - 1, 9, 1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tk", new DateTime(DateTime.Now.Year - 1, 11, 30)));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("@tp", new DateTime(DateTime.Now.Year - 2, 12, 1)));
                cmd.Parameters.Add(new NpgsqlParameter("@tk", new DateTime(DateTime.Now.Year - 1, 2, 28)));
            }

            da = new NpgsqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "View";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());

                actions.Add(action);
            }

            cmd.CommandText = "select userid, productid, grade from reviewedproducts where time>@t union select userid, productid, grade from reviewedproducts where " +
                "time>=@tp and time<=@tk";
            dt.Clear();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "Review";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());
                action.Grade = int.Parse(dr["grade"].ToString());

                actions.Add(action);
            }

            cmd.CommandText = "select userid, productid from boughtproducts where time>@t union select userid, productid from boughtproducts where " +
                "time>=@tp and time<=@tk";
            dt.Clear();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "Buy";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());

                actions.Add(action);
            }

            cmd.CommandText = "select userid, productid from seenreviews where time>@t union select userid, productid from seenreviews where " +
                "time>=@tp and time<=@tk";
            dt.Clear();
            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                DomainModel.RecommenderAction action = new DomainModel.RecommenderAction();
                action.Action = "SeeReviews";
                action.UserId = new ObjectId(dr["userid"].ToString());
                action.ProductId = new ObjectId(dr["productid"].ToString());

                actions.Add(action);
            }

            cmd.Dispose();

            return actions;
        }
    }
}
