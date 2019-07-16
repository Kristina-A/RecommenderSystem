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
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        public ActionResult Checkout()
        {
            MongodbFunctions mongo = new MongodbFunctions();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            Databases.DomainModel.CheckoutDetails details = new Databases.DomainModel.CheckoutDetails();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Order order = mongo.GetOpenOrder(user.Id);
            details.User = user;
            List<Databases.DomainModel.Product> products = new List<Databases.DomainModel.Product>();
            List<Databases.DomainModel.Notification> discounts = new List<Databases.DomainModel.Notification>();
            List<string> disc = tdb.GetDiscounts(user.Id.ToString());

            if (order != null)
            {
                foreach (MongoDBRef r in order.Products)
                {
                    Databases.DomainModel.Product product = mongo.GetProduct(new ObjectId(r.Id.ToString()));
                    products.Add(product);
                }

                if(disc.Count!=0)
                {
                    foreach(string d in disc)
                    {
                        Databases.DomainModel.Notification notification = mongo.GetNotification(new ObjectId(d));
                        discounts.Add(notification);
                    }
                }
            }
            details.Products = products;
            details.Discounts = discounts;
            tdb.CloseConnection();

            return View(details);
        }

        [HttpPost]
        public void AddToChart(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Order order = mongo.GetOpenOrder(user.Id);

            if (order == null)
            {
                List<MongoDBRef> products = new List<MongoDBRef>();
                products.Add(new MongoDBRef("products", new ObjectId(id)));

                order = new Databases.DomainModel.Order
                {
                    Date = DateTime.Now,
                    Status = "opened",
                    Products = products
                };

                mongo.AddUpdateOrder(order, user.Email, "add");
            }
            else
            {
                order.Products.Add(new MongoDBRef("products", new ObjectId(id)));
                mongo.AddUpdateOrder(order, user.Email, "update");
            }
        }

        [HttpPost]
        public void DeleteFromChart(string id)
        {
            MongodbFunctions mongo = new MongodbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Order order = mongo.GetOpenOrder(user.Id);

            order.Products.Remove(new MongoDBRef("products", new ObjectId(id)));

            if (order.Products.Count > 0)
                mongo.RemoveProduct(order);
            else
                mongo.DeleteOrder(order.Id);
        }

        [HttpPost]
        public void DeleteOrder()
        {
            MongodbFunctions mongo = new MongodbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Order order = mongo.GetOpenOrder(user.Id);

            mongo.DeleteOrder(order.Id);
        }

        [HttpPost]
        public void SubmitOrder(string address, string note, string pay)
        {
            MongodbFunctions mongo = new MongodbFunctions();
            TimescaledbFunctions tdb = new TimescaledbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Order order = mongo.GetOpenOrder(user.Id);

            List<string> disc = tdb.GetDiscounts(user.Id.ToString());

            if (!user.Address.Contains(address))
            {
                user.Address.Add(address);
                mongo.UpdateAddresses(user);
            }

            order.Note = note;
            order.PayingMethod = pay;
            order.Address = address;

            mongo.CloseOrder(order);

            foreach(string d in disc)
            {
                Databases.DomainModel.Notification notification = mongo.GetNotification(new ObjectId(d));

                if(notification.Tag.Equals("popust"))
                {
                    mongo.UpdateNotification(notification.Id, "iskorisceno");
                    tdb.UpdateNotification(user.Id.ToString(), d, "iskorisceno");
                }
                else if(notification.Tag.Equals("l_popust"))
                {
                    mongo.UpdateNotification(notification.Id, "l_iskorisceno");
                    tdb.UpdateNotification(user.Id.ToString(), d, "l_iskorisceno");
                }
            }

            foreach (MongoDBRef p in order.Products)
            {
                Databases.DomainModel.Product prod = mongo.GetProduct(new ObjectId(p.Id.ToString()));
                tdb.BuyProduct(user.Id.ToString(), prod.Id.ToString(), prod.Price);
            }

            if (!tdb.NotificationSent(user.Id.ToString()) && (30000 - tdb.MonthShopping(user.Id.ToString())) < 3000 && (30000 - tdb.MonthShopping(user.Id.ToString())) >0)//notifikacija do popusta
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
            else if(tdb.MonthShopping(user.Id.ToString())>=30000)
            {
                Databases.DomainModel.Notification notification = new Databases.DomainModel.Notification
                {
                    Content = "Poštovani, ostvarili ste popust od 20% na sledeću kupovinu, koji možete iskoristiti u roku od nedelju dana.",
                    Title = "Popust 20%",
                    Date = DateTime.Now.Date,
                    Tag = "popust",
                    Read = false,
                    User = new MongoDB.Driver.MongoDBRef("users", user.Id)
                };

                tdb.SendNotification(user.Id.ToString(), mongo.AddNotification(notification, user.Email).ToString(), "popust");
            }

            tdb.CloseConnection();
        }

        [HttpPost]
        public JsonResult UpdateChart()
        {
            MongodbFunctions mongo = new MongodbFunctions();

            Databases.DomainModel.User user = mongo.GetUser(User.Identity.Name);
            Databases.DomainModel.Order order = mongo.GetOpenOrder(user.Id);//vraca opened order, samo 1 po useru moze da postoji

            int count;
            List<Databases.DomainModel.ProductShow> products = new List<Databases.DomainModel.ProductShow>();

            if (order == null)
                count = 0;
            else
            {
                count = order.Products.Count;

                foreach (MongoDBRef r in order.Products)
                {
                    Databases.DomainModel.Product product = mongo.GetProduct(new ObjectId(r.Id.ToString()));

                    Databases.DomainModel.ProductShow pr = new Databases.DomainModel.ProductShow
                    {
                        Id = product.Id.ToString(),
                        Name = product.Name,
                        Price = product.Price
                    };

                    products.Add(pr);
                }
            }

            return Json(new { number = count, prod = products }, JsonRequestBehavior.AllowGet);
        }
    }
}