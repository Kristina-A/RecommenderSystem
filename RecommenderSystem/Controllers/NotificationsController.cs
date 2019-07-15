﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using Databases;

namespace RecommenderSystem.Controllers
{
    [Authorize(Roles ="User")]
    public class NotificationsController : Controller
    {
        [HttpPost]
        public JsonResult GetNotifications()
        {
            MongodbFunctions mongo = new MongodbFunctions();
            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

            TimescaledbFunctions tdb = new TimescaledbFunctions();
            List<string> notifications = tdb.GetNotifications(user.Id.ToString());

            List<Databases.DomainModel.NotificationShow> nots = new List<Databases.DomainModel.NotificationShow>();

            int count=0;
            if (notifications.Count != 0)
            {
                foreach(string notId in notifications)
                {
                    Databases.DomainModel.Notification not = mongo.GetNotification(new ObjectId(notId));
                    if (!not.Read)
                        count++;
                    Databases.DomainModel.NotificationShow nshow = new Databases.DomainModel.NotificationShow
                    {
                        Id=not.Id.ToString(),
                        Title=not.Title,
                        Date=not.Date.Date,
                        Read=not.Read
                    };
                    nots.Add(nshow);
                }
            }

            return Json(new { number = count, alerts=nots },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ActivateDiscount()
        {
            MongodbFunctions mongo = new MongodbFunctions();
            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

            TimescaledbFunctions tdb = new TimescaledbFunctions();

            Databases.DomainModel.Notification notification = new Databases.DomainModel.Notification
            {
                Content = "Poštovani, ostvarili ste popust od 10% na sledeću kupovinu, koji možete iskoristiti u roku od nedelju dana.",
                Title = "Popust 10%",
                Date = DateTime.Now.Date,
                Tag = "l_popust",
                Read = false,
                User = new MongoDB.Driver.MongoDBRef("users", user.Id)
            };

            tdb.SendNotification(user.Id.ToString(), mongo.AddNotification(notification, user.Email).ToString(), "l_popust");
        }

        [HttpPost]
        public void UseDiscount(string notId, string tag)
        {
            MongodbFunctions mongo = new MongodbFunctions();
            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

            TimescaledbFunctions tdb = new TimescaledbFunctions();

            mongo.UpdateNotification(new ObjectId(notId), tag);
            tdb.UpdateNotification(user.Id.ToString(), notId, tag);
        }
    }
}