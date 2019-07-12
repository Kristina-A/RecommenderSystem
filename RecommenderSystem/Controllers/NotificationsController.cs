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

            return Json(new { },JsonRequestBehavior.AllowGet);
        }
    }
}