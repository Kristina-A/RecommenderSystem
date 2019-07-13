using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using Databases;

namespace RecommenderSystem.Controllers
{
    public class NotificationsController : Controller
    {
        [HttpPost]
        public JsonResult GetNotifications()
        {
            MongodbFunctions mongo = new MongodbFunctions();
            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

            TimescaledbFunctions tdb = new TimescaledbFunctions();
            List<string> notifications = tdb.GetNotifications(user.Id.ToString());

            List<string> contents = new List<string>();

            int count=0;
            if (notifications.Count != 0)
            {
                foreach(string notId in notifications)
                {
                    Databases.DomainModel.Notification not = mongo.GetNotification(new ObjectId(notId));
                    if (!not.Read)
                        count++;
                    contents.Add(not.Content);
                }
            }

            return Json(new { number = count, notif=contents },JsonRequestBehavior.AllowGet);
        }
    }
}