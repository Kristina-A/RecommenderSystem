using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Databases;
using MongoDB;
using RecommendationEngine;

namespace RecommenderSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Databases.DomainModel.Product> products = new List<Databases.DomainModel.Product>();
            MongodbFunctions mongo = new MongodbFunctions();
            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                TimescaledbFunctions tdb = new TimescaledbFunctions();

                Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

                if (!tdb.NotificationSent(user.Id.ToString()) && (30000 - tdb.MonthShopping(user.Id.ToString())) < 3000)
                {
                    Databases.DomainModel.Notification notification = new Databases.DomainModel.Notification
                    {
                        Content = "Poštovani, do ostvarivanja popusta od 20% ostalo Vam je manje od 3000 dinara.",
                        Title = "Mali iznos do popusta",
                        Date = DateTime.Now.Date,
                        Tag = "do_popusta",
                        Read = false,
                        User = new MongoDB.Driver.MongoDBRef("users", user.Id)
                    };


                    tdb.SendNotification(user.Id.ToString(), mongo.AddNotification(notification, user.Email).ToString(), "do_popusta");
                }

                tdb.CloseConnection();

                //recommend za proizvode
                RecommendationEngine.Interfaces.IRater rater = new LinearRater(1.0, 5.0);
                RecommendationEngine.Interfaces.IComparer comparer = new CosineComparer();
                RecommendationEngine.Recommenders.ItemCollaborativeFilterRecommender recommender1 = new RecommendationEngine.Recommenders.ItemCollaborativeFilterRecommender(comparer, rater, 5);
                RecommendationEngine.Interfaces.IRecommender recommender2 = new RecommendationEngine.Recommenders.UserCollaborativeFilterRecommender(comparer, rater, 5);

                RecommendationEngine.Parsers.UserBehaviorDatabaseParser parser = new RecommendationEngine.Parsers.UserBehaviorDatabaseParser();
                List<Databases.DomainModel.RecommenderAction> actions = parser.LoadForHome();
                RecommendationEngine.Parsers.UserBehaviorDatabase db = parser.LoadUserBehaviorDatabase(actions);

                recommender1.Train(db);
                recommender2.Train(db);

                if (actions.Count(x=>x.UserId.Equals(user.Id))>0)//if user has actions, recommendation can be done
                {
                    List<RecommendationEngine.Objects.Suggestion> suggestions1 = recommender1.GetSuggestions(user.Id, 6);
                    List<RecommendationEngine.Objects.Suggestion> suggestions2 = recommender2.GetSuggestions(user.Id, 0);

                    foreach (RecommendationEngine.Objects.Suggestion s in suggestions1)
                    {
                        Databases.DomainModel.Product product = mongo.GetProduct(s.ProductID);
                        if (!products.Exists(x => x.Id.Equals(product.Id)))
                            products.Add(product);
                    }

                    foreach (RecommendationEngine.Objects.Suggestion s in suggestions2)
                    {
                        Databases.DomainModel.Product product = mongo.GetProduct(s.ProductID);
                        if (!products.Exists(x => x.Id.Equals(product.Id)))
                            products.Add(product);
                    }
                }
                else//no actions, recommendation using gender and birth date
                {
                    List<RecommendationEngine.Objects.Suggestion> suggestions1=recommender1.GetFirstSuggestions(db, user, 10);

                    foreach (RecommendationEngine.Objects.Suggestion s in suggestions1)
                    {
                        Databases.DomainModel.Product product = mongo.GetProduct(s.ProductID);
                        if (!products.Exists(x => x.Id.Equals(product.Id)))
                            products.Add(product);
                    }
                }

                //recommend za reklame
                List<Databases.DomainModel.Advert> adverts = new List<Databases.DomainModel.Advert>();

                actions = parser.LoadForAdverts();
                db = parser.LoadUserBehaviorDatabase(actions);

                recommender1.Train(db);

                List<RecommendationEngine.Objects.Suggestion> suggestions = new List<RecommendationEngine.Objects.Suggestion>();

                if (actions.Count(x => x.UserId.Equals(user.Id)) > 0)
                    suggestions = recommender1.GetSuggestions(user.Id, 5);
                else
                    suggestions = recommender1.GetFirstSuggestions(db, user, 10);

                foreach (RecommendationEngine.Objects.Suggestion s in suggestions)
                {
                    Databases.DomainModel.Product product = mongo.GetProduct(s.ProductID);

                    foreach (Databases.DomainModel.Advert advert in mongo.GetAdverts(product.Subcategory))
                    {
                        if (!adverts.Exists(x => x.Id.Equals(advert.Id)))
                            adverts.Add(advert);
                    }
                }
                ViewBag.Adverts = adverts;
            }
            else //admin logged
            {
                products = mongo.GetProducts();
            }

            return View(products);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}