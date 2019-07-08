using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Databases;

namespace RecommenderSystem.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CategoryProducts(string category)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            if (category.Equals(""))
                return RedirectToAction("Home", "Index");

            TimescaledbFunctions f = new TimescaledbFunctions();
            ViewBag.categoryName = category;
            return View(mongo.GetCategoryProducts(category));
        }
    }
}