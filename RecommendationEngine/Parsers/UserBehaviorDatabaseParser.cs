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

        public List<Databases.DomainModel.RecommenderAction> LoadForSearch()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetMonthlyActivities(1);

            tdb.CloseConnection();

            return actions;
        }

        public List<Databases.DomainModel.RecommenderAction> LoadForAdverts()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetMonthlyActivities(3);

            tdb.CloseConnection();

            return actions;
        }

        public List<Databases.DomainModel.RecommenderAction> LoadForHome()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetWeeklyActivities();

            tdb.CloseConnection();

            return actions;
        }

        public List<Databases.DomainModel.RecommenderAction> LoadForSimilarBoughtProducts()
        {
            List<Databases.DomainModel.RecommenderAction> actions = new List<Databases.DomainModel.RecommenderAction>();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            actions = tdb.GetMonthlyActivities(12);

            tdb.CloseConnection();

            return actions;
        }

        public UserBehaviorDatabase LoadUserBehaviorDatabase(List<Databases.DomainModel.RecommenderAction> actions)
        {
            UserBehaviorDatabase db = new UserBehaviorDatabase();
            MongodbFunctions mongo = new MongodbFunctions();

            db.Categories = mongo.GetCategories();
            db.Users = mongo.GetUsers();
            db.Products = mongo.GetProducts();
            
            foreach(Databases.DomainModel.RecommenderAction a in actions)
            {
                db.UserActions.Add(new Objects.UserAction(a.Action, a.UserId, a.ProductId, a.Grade));
            }

            return db;
        }
    }
}
