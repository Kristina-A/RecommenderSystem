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
        private List<Databases.DomainModel.RecommenderAction> actions { get; set; }
        public UserBehaviorDatabaseParser()
        {
        }

        public void LoadForSearch()
        {
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetMonthlyActivities();

            tdb.CloseConnection();
        }

        public void LoadForAdverts()
        {

        }

        public void LoadForHome()
        {

        }
    }
}
