using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Databases;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using RecommendationEngine;

namespace RecommenderSystem.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult CategoryProducts(string category)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            if (category.Equals(""))
                return RedirectToAction("Home", "Index");

            ViewBag.categoryName = category;
            return View(mongo.GetCategoryProducts(category));
        }

        public ActionResult ProductDetails(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            if (id.Equals(""))
                return RedirectToAction("Home", "Index");

            ObjectId objID = new ObjectId(id);

            Databases.DomainModel.Product product = mongo.GetProduct(objID);
            if (product != null)
                return View(product);
            else
                return HttpNotFound();
        }

        [HttpPost]
        public JsonResult AverageGrade(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            ObjectId objID = new ObjectId(id);

            List<double> lista = mongo.AverageGrade(objID);

            return Json(new { number = lista[1], grade = lista[0] }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void DeleteProduct(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();
            ObjectId objID = new ObjectId(id);
            mongo.DeleteProduct(objID);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void EditProduct(string id, string name, int price)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            ObjectId objID = new ObjectId(id);

            var picture = Request.Files["picture"];
            string path;
            if (picture != null)
            {
                string savePath = System.IO.Path.Combine(Server.MapPath("~/Resources"), name + System.IO.Path.GetExtension(picture.FileName));
                picture.SaveAs(savePath);
                path = name + System.IO.Path.GetExtension(picture.FileName);
            }
            else
            {
                Databases.DomainModel.Product product = mongo.GetProduct(objID);
                path = product.Picture;
            }
            mongo.UpdateProduct(objID, name, price, path);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void EditCharacteristics(string id, string charName, string charValue, string oldN, string oldV)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            ObjectId objID = new ObjectId(id);
            Databases.DomainModel.Product product = mongo.GetProduct(objID);

            List<string> chars = product.Characteristics;
            string newChar = charName + ":" + charValue;

            if (oldN.Equals("") || oldV.Equals(""))
            {
                chars.Add(newChar);
            }
            else
            {
                int index = chars.IndexOf(oldN + ":" + oldV);
                chars.RemoveAt(index);
                chars.Insert(index, newChar);
            }

            mongo.UpdateCharacteristics(objID, chars);
        }

        [HttpPost]
        public JsonResult GetReviews(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            ObjectId objID = new ObjectId(id);

            Databases.DomainModel.Product product = mongo.GetProduct(objID);
            List<MongoDBRef> rev = product.Reviews;
            int count = product.Reviews.Count;

            List<Databases.DomainModel.ReviewShow> reviews = new List<Databases.DomainModel.ReviewShow>();
            List<Databases.DomainModel.UserShow> users = new List<Databases.DomainModel.UserShow>();

            foreach (MongoDBRef r in rev)
            {
                Databases.DomainModel.Review review = mongo.GetReview(new ObjectId(r.Id.ToString()));
                Databases.DomainModel.User user = mongo.GetUser(new ObjectId(review.User.Id.ToString()));

                Databases.DomainModel.UserShow userShow = new Databases.DomainModel.UserShow
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Address = user.Address
                };

                Databases.DomainModel.ReviewShow reviewShow = new Databases.DomainModel.ReviewShow
                {
                    Id = review.Id,
                    Grade = review.Grade,
                    Comment = review.Comment
                };
                reviews.Add(reviewShow);
                users.Add(userShow);
            }

            //Databases.DomainModel.User u = mongo.GetUser(User.Identity.Name);

            //TimescaledbFunctions tdb = new TimescaledbFunctions();
            //tdb.SeeReviews(u.Id.ToString(), id);

            return Json(new { number = count, revs = reviews, people = users }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetComments(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            ObjectId objID = new ObjectId(id);

            Databases.DomainModel.Product product = mongo.GetProduct(objID);
            List<MongoDBRef> mess = product.Messages;
            int count = product.Messages.Count;
            string role;
            if (User.IsInRole("Admin"))
                role = "Admin";
            else
                role = "User";

            List<Databases.DomainModel.MessageShow> comments = new List<Databases.DomainModel.MessageShow>();
            List<Databases.DomainModel.UserShow> users = new List<Databases.DomainModel.UserShow>();

            foreach (MongoDBRef r in mess)
            {
                Databases.DomainModel.Message comment = mongo.GetComment(new ObjectId(r.Id.ToString()));
                Databases.DomainModel.User user = mongo.GetUser(new ObjectId(comment.User.Id.ToString()));
                List<string> responses = new List<string>();

                foreach (MongoDBRef rr in comment.Responses)
                {
                    Databases.DomainModel.AdminResponse response = mongo.GetResponse(new ObjectId(rr.Id.ToString()));
                    responses.Add(response.Content);
                }

                Databases.DomainModel.MessageShow messShow = new Databases.DomainModel.MessageShow
                {
                    Id = comment.Id.ToString(),
                    Content = comment.Content,
                    Responses = responses
                };
                Databases.DomainModel.UserShow userShow = new Databases.DomainModel.UserShow
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Address = user.Address
                };
                comments.Add(messShow);
                users.Add(userShow);
            }

            return Json(new { number = count, status = role, com = comments, people = users }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public void AddComment(string prodId, string content)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            Databases.DomainModel.Message newMessage = new Databases.DomainModel.Message
            {
                Content = content,
                Product = new MongoDBRef("products", new ObjectId(prodId))
            };

            mongo.AddComment(newMessage, prodId, User.Identity.Name);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public void AddReview(string id, int grade, string comment)
        {
            MongodbFunctions mongo = new MongodbFunctions();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Review review = mongo.GetReview(user.Id, new ObjectId(id));

            if (review==null)
            {

                Databases.DomainModel.Review newReview = new Databases.DomainModel.Review
                {
                    Grade = grade,
                    Comment = comment,
                    Product = new MongoDBRef("products", new ObjectId(id))
                };

                mongo.AddReview(newReview, id, User.Identity.Name);

                tdb.ReviewProduct(user.Id.ToString(), id, grade);
            }
            else
            {
                mongo.UpdateReview(review.Id, grade, comment);
                tdb.UpdateReview(user.Id.ToString(), id, grade);
            }

            if(tdb.LowGrades(user.Id.ToString())>=4)
            {
                Databases.DomainModel.Notification notification = new Databases.DomainModel.Notification
                {
                    Content = "Poštovani, primetili smo da ste više proizvoda ocenili lošom ocenom. Ako želite da zamenite neki od njih, " +
                    "pozovite naš call centar i dogovorite se sa operaterom. Takođe, imate opciju popusta od 10% prilikom sledeće kupovine. " +
                    "Popust možete aktivirati klikom na link u ovom obaveštenju i važi nedelju dana. U jednom trenutku možete aktivirati samo jedan popust.",
                    Title = "Niste zadovoljni kupljenim proizvodima?",
                    Date = DateTime.Now.Date,
                    Tag = "lose_ocene",
                    Read = false,
                    User = new MongoDB.Driver.MongoDBRef("users", user.Id)
                };

                tdb.SendNotification(user.Id.ToString(), mongo.AddNotification(notification, user.Email).ToString(), "lose_ocene");
            }

            tdb.CloseConnection();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void AddResponse(string id, string content)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            ObjectId messId = new ObjectId(id);

            Databases.DomainModel.AdminResponse newResponse = new Databases.DomainModel.AdminResponse
            {
                Content = content,
                Message = new MongoDBRef("messages", messId)
            };

            mongo.AddResponse(newResponse, messId);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public void ViewProduct(string prodID)
        {
            MongodbFunctions mongo = new MongodbFunctions();
            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

            //TimescaledbFunctions tdb = new TimescaledbFunctions();
            //tdb.ViewProduct(user.Id.ToString(), prodID);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult NewProduct()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public void AddNewProduct(string name, string category, string subcategory, int price, string characteristics)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            var picture = Request.Files["picture"];
            string path = System.IO.Path.Combine(Server.MapPath("~/Resources"), name + System.IO.Path.GetExtension(picture.FileName));
            picture.SaveAs(path);

            Databases.DomainModel.Category cat = mongo.GetCategory(category);
            Databases.DomainModel.Product newProduct = new Databases.DomainModel.Product
            {
                Name = name,
                Price = price,
                Subcategory = subcategory,
                Picture = name + System.IO.Path.GetExtension(picture.FileName),
                Category = new MongoDBRef("categories", cat.Id),
                Characteristics = JsonConvert.DeserializeObject<List<string>>(characteristics)
            };

            mongo.InsertProduct(newProduct, category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult ReturnSubcategories(string option)
        {
            MongodbFunctions mongo = new MongodbFunctions();
            List<string> subcategories = mongo.GetSubcategories(option);

            return Json(new { subcat = subcategories }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchProduct(string name)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            if (name.Equals(""))
                return RedirectToAction("Home", "Index");

            List<Databases.DomainModel.Product> products = mongo.SearchForProductsByName(name);

            return View(products);
        }

        [HttpPost]
        public JsonResult GetSuggestions()
        {
            List<string> products = new List<string>();

            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                MongodbFunctions mongo = new MongodbFunctions();
                Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);

                RecommendationEngine.Interfaces.IRater rater = new LinearRater(1.0, 5.0);
                RecommendationEngine.Interfaces.IComparer comparer = new CosineComparer();
                RecommendationEngine.Interfaces.IRecommender recommender1 = new RecommendationEngine.Recommenders.ItemCollaborativeFilterRecommender(comparer, rater, 20);

                RecommendationEngine.Parsers.UserBehaviorDatabaseParser parser = new RecommendationEngine.Parsers.UserBehaviorDatabaseParser();
                RecommendationEngine.Parsers.UserBehaviorDatabase db = parser.LoadUserBehaviorDatabase(parser.LoadForSearch());

                recommender1.Train(db);

                List<RecommendationEngine.Objects.Suggestion> suggestions1 = recommender1.GetSuggestions(user.Id, 5);

                foreach (RecommendationEngine.Objects.Suggestion s in suggestions1)
                {
                    Databases.DomainModel.Product product = mongo.GetProduct(s.ProductID);
                    if (!products.Exists(x => x.Equals(product.Name)))
                        products.Add(product.Name);
                }
            }

            return Json(new { prods = products }, JsonRequestBehavior.AllowGet);
        }
    }
}