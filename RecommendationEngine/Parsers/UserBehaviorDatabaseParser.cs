using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Databases;
using MongoDB.Bson;

namespace RecommendationEngine.Parsers
{
    public class UserBehaviorDatabaseParser
    { 
        public UserBehaviorDatabaseParser()
        {
        }

        public void LoadForSearch()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetMonthlyActivities(1);

            tdb.CloseConnection();
        }

        public void LoadForAdverts()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetMonthlyActivities(3);

            tdb.CloseConnection();
        }

        public void LoadForHome()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetWeeklyActivities();

            tdb.CloseConnection();
        }

        public UserBehaviorDatabase LoadUserBehaviorDatabase(List<Databases.DomainModel.RecommenderAction> actions)
        {
            UserBehaviorDatabase db = new UserBehaviorDatabase();

            return db;
        }
    }
}
