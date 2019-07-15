using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Databases;
using MongoDB;

namespace RecommenderSystem.Controllers
{
    public class HomeController : Controller
    {
        [Authorize(Roles ="User")]
        public ActionResult Index()
        {
            MongodbFunctions mongo = new MongodbFunctions();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

            if(!tdb.NotificationSent(user.Id.ToString()) && (30000-tdb.MonthShopping(user.Id.ToString()))<3000)
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
            return View();
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