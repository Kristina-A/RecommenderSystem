using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Databases;
using Newtonsoft.Json;

namespace RecommenderSystem.Controllers
{
    public class AdvertController : Controller
    {
        // GET: Advert
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void AddNewAdvert(string categories)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            var picture = Request.Files["picture"];
            string path = System.IO.Path.Combine(Server.MapPath("~/Resources/Adverts"), picture.FileName);
            picture.SaveAs(path);

            Databases.DomainModel.Advert newAdvert = new Databases.DomainModel.Advert
            {
                Picture = picture.FileName,
                Subcategories = JsonConvert.DeserializeObject<List<string>>(categories)
            };

            mongo.InsertAd(newAdvert);
        }
    }
}