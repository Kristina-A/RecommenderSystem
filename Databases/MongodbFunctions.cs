using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Databases.DomainModel;

namespace Databases
{
    public class MongodbFunctions
    {
        MongoClient client;
        IMongoDatabase db;

        public MongodbFunctions()
        {

            client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase("webshopdb");

        }

        public void InsertUser(User user)
        {
            var usersCollection = db.GetCollection<User>("users");

            usersCollection.InsertOne(user);
        }

        public void InsertProduct(Product product, string cat)
        {
            var productsCollection = db.GetCollection<Product>("products");
            var categoriesCollection = db.GetCollection<Category>("categories");
            var filter = Builders<Category>.Filter.Eq("Name", cat);

            productsCollection.InsertOne(product);

            Category category = GetCategory(cat);
            category.Products.Add(new MongoDBRef("products", product.Id));
            var update = Builders<Category>.Update.Set("Products", category.Products);

            categoriesCollection.UpdateOne(filter, update);
        }

        public List<string> GetSubcategories(string category)
        {
            var categoriesCollection = db.GetCollection<Category>("categories");

            var filter = Builders<Category>.Filter.Eq("Name", category);
            var categories = categoriesCollection.Find(filter);

            Category cat = categories.First();

            return cat.Subcategories;
        }

        public Category GetCategory(string category)
        {
            var categoriesCollection = db.GetCollection<Category>("categories");

            var filter = Builders<Category>.Filter.Eq("Name", category);
            var categories = categoriesCollection.Find(filter);

            return categories.First();
        }

        public List<Product> GetCategoryProducts(string category)
        {
            var productsCollection = db.GetCollection<Product>("products");

            var filter = Builders<Product>.Filter.Eq("Subcategory", category);

            return productsCollection.Find(filter).ToList();
        }

        public Product GetProduct(ObjectId id)
        {
            var productsCollection = db.GetCollection<Product>("products");

            var filter = Builders<Product>.Filter.Eq("_id", id);
            var products = productsCollection.Find(filter);

            return products.First();
        }

        public List<double> AverageGrade(ObjectId id)//id proizvoda za prosecnu ocenu
        {
            List<double> lista = new List<double>();
            var reviewsCollection = db.GetCollection<Review>("reviews");
            var filter = Builders<Review>.Filter.Eq("Product", new MongoDBRef("products", id));
            var res = reviewsCollection.Aggregate().Match(filter).Group(c => c.Product, g =>
                     new
                     {
                         AvgGrade = g.Average(p => p.Grade),
                         Number = g.Count()
                     }).ToList();

            if (res.Count == 0)
            {
                lista.Add(0.0);//prosecna ocena
                lista.Add(0.0);//br reviewa
                return lista;
            }
            else
            {
                lista.Add(res.Select(_ => _.AvgGrade).First());
                lista.Add(res.Select(_ => _.Number).First());
                return lista;
            }
        }

        public void DeleteProduct(ObjectId id)
        {
            var productsCollection = db.GetCollection<Product>("products");

            var filter = Builders<Product>.Filter.Eq("_id", id);
            productsCollection.DeleteOne(filter);
        }

        public void UpdateProduct(ObjectId id, string name, int price, string path)
        {
            var productsCollection = db.GetCollection<Product>("products");

            var filter = Builders<Product>.Filter.Eq("_id", id);
            var update = Builders<Product>.Update.Set("Name", name)
                                                .Set("Price", price)
                                                .Set("Picture", path);
            productsCollection.UpdateOne(filter, update);
        }

        public void UpdateCharacteristics(ObjectId id, List<string> characteristics)
        {
            var productsCollection = db.GetCollection<Product>("products");

            var filter = Builders<Product>.Filter.Eq("_id", id);
            var update = Builders<Product>.Update.Set("Characteristics", characteristics);
            productsCollection.UpdateOne(filter, update);
        }

        public User GetUser(ObjectId id)
        {
            var usersCollection = db.GetCollection<User>("users");

            var filter = Builders<User>.Filter.Eq("_id", id);
            var users = usersCollection.Find(filter);

            return users.First();
        }

        public Review GetReview(ObjectId id)
        {
            var reviewsCollection = db.GetCollection<Review>("reviews");

            var filter = Builders<Review>.Filter.Eq("_id", id);
            var reviews = reviewsCollection.Find(filter);

            return reviews.First();
        }

        public Review GetReview(ObjectId userId, ObjectId prodId)
        {
            var reviewsCollection = db.GetCollection<Review>("reviews");

            var filter = Builders<Review>.Filter.And(Builders<Review>.Filter.Eq("User", new MongoDBRef("users",userId)),
                                                      Builders<Review>.Filter.Eq("Product",new MongoDBRef("products",prodId)));
            var reviews = reviewsCollection.Find(filter).ToList();

            if (reviews.Count == 0)
                return null;
            else
                return reviews.First();
        }

        public Message GetComment(ObjectId id)
        {
            var commentsCollection = db.GetCollection<Message>("messages");

            var filter = Builders<Message>.Filter.Eq("_id", id);
            var comments = commentsCollection.Find(filter);

            return comments.First();
        }

        public AdminResponse GetResponse(ObjectId id)
        {
            var responsesCollection = db.GetCollection<AdminResponse>("adminresponses");

            var filter = Builders<AdminResponse>.Filter.Eq("_id", id);
            var responses = responsesCollection.Find(filter);

            return responses.First();
        }

        public void AddComment(Message message, string prodId, string email)
        {
            User user = GetUser(email);

            message.User = new MongoDBRef("users", user.Id);
            var commentsCollection = db.GetCollection<Message>("messages");
            var usersCollection = db.GetCollection<User>("users");
            var productsCollection = db.GetCollection<Product>("products");

            commentsCollection.InsertOne(message);

            user.Messages.Add(new MongoDBRef("messages", message.Id));
            Product prod = GetProduct(new ObjectId(prodId));
            prod.Messages.Add(new MongoDBRef("messages", message.Id));

            var update = Builders<User>.Update.Set("Messages", user.Messages);
            var filter = Builders<User>.Filter.Eq("Email", email);
            var update1 = Builders<Product>.Update.Set("Messages", prod.Messages);
            var filter1 = Builders<Product>.Filter.Eq("_id", new ObjectId(prodId));

            usersCollection.UpdateOne(filter, update);
            productsCollection.UpdateOne(filter1, update1);
        }

        public User GetUser(string email)
        {
            var usersCollection = db.GetCollection<User>("users");

            var filter = Builders<User>.Filter.Eq("Email", email);

            return usersCollection.Find(filter).First();
        }

        public void AddReview(Review review, string prodId, string email)
        {
            User user = GetUser(email);

            review.User = new MongoDBRef("users", user.Id);
            var reviewsCollection = db.GetCollection<Review>("reviews");
            var usersCollection = db.GetCollection<User>("users");
            var productsCollection = db.GetCollection<Product>("products");

            reviewsCollection.InsertOne(review);

            user.Reviews.Add(new MongoDBRef("reviews", review.Id));
            Product prod = GetProduct(new ObjectId(prodId));
            prod.Reviews.Add(new MongoDBRef("reviews", review.Id));

            var update = Builders<User>.Update.Set("Reviews", user.Reviews);
            var filter = Builders<User>.Filter.Eq("Email", email);
            var update1 = Builders<Product>.Update.Set("Reviews", prod.Reviews);
            var filter1 = Builders<Product>.Filter.Eq("_id", new ObjectId(prodId));

            usersCollection.UpdateOne(filter, update);
            productsCollection.UpdateOne(filter1, update1);
        }

        public void AddResponse(AdminResponse response, ObjectId messId)
        {
            Message message = GetComment(messId);

            var responsesCollection = db.GetCollection<AdminResponse>("adminresponses");
            var commentsCollection = db.GetCollection<Message>("messages");

            responsesCollection.InsertOne(response);

            message.Responses.Add(new MongoDBRef("adminresponses", response.Id));

            var update = Builders<Message>.Update.Set("Responses", message.Responses);
            var filter = Builders<Message>.Filter.Eq("_id", messId);

            commentsCollection.UpdateOne(filter, update);
        }

        public Order GetOpenOrder(ObjectId id)
        {
            var ordersCollection = db.GetCollection<Order>("orders");

            var filter = Builders<Order>.Filter.And(Builders<Order>.Filter.Eq("User", new MongoDBRef("users", id))
                                                    , Builders<Order>.Filter.Eq("Status", "opened"));

            var orders = ordersCollection.Find(filter).ToList();

            if (orders.Count == 0)
                return null;
            else
                return orders.First();
        }

        public void AddUpdateOrder(Order order, string email, string addupdate)
        {
            var ordersCollection = db.GetCollection<Order>("orders");

            if (addupdate.Equals("add"))
            {
                var usersCollection = db.GetCollection<User>("users");

                User user = GetUser(email);
                order.User = new MongoDBRef("users", user.Id);

                ordersCollection.InsertOne(order);

                user.Orders.Add(new MongoDBRef("orders", order.Id));

                var update = Builders<User>.Update.Set("Orders", user.Orders);
                var filter = Builders<User>.Filter.Eq("Email", email);

                usersCollection.UpdateOne(filter, update);
            }
            else
            {
                var update = Builders<Order>.Update.Set("Products", order.Products);
                var filter = Builders<Order>.Filter.Eq("_id", order.Id);

                ordersCollection.UpdateOne(filter, update);
            }
        }

        public void RemoveProduct(Order order)
        {
            var ordersCollection = db.GetCollection<Order>("orders");

            var update = Builders<Order>.Update.Set("Products", order.Products);
            var filter = Builders<Order>.Filter.Eq("_id", order.Id);

            ordersCollection.UpdateOne(filter, update);
        }

        public void DeleteOrder(ObjectId id)
        {
            var ordersCollection = db.GetCollection<Order>("orders");

            var filter = Builders<Order>.Filter.Eq("_id", id);

            ordersCollection.DeleteOne(filter);
        }

        public void CloseOrder(Order order)
        {
            var ordersCollection = db.GetCollection<Order>("orders");

            var update = Builders<Order>.Update.Set("Status", "closed")
                                               .Set("Note", order.Note)
                                               .Set("PayingMethod", order.PayingMethod)
                                               .Set("Address", order.Address);
            var filter = Builders<Order>.Filter.Eq("_id", order.Id);

            ordersCollection.UpdateOne(filter, update);
        }

        public void UpdateAddresses(User user)
        {
            var usersCollection = db.GetCollection<User>("users");

            var filter = Builders<User>.Filter.Eq("_id", user.Id);
            var update = Builders<User>.Update.Set("Address", user.Address);

            usersCollection.UpdateOne(filter, update);
        }

        public Notification GetNotification(ObjectId id)
        {
            var notificationsCollection = db.GetCollection<Notification>("notifications");

            var filter = Builders<Notification>.Filter.Eq("_id", id);
            var notifications = notificationsCollection.Find(filter);

            return notifications.First();
        }

        public ObjectId AddNotification(Notification notification, string email)
        {
            User user = GetUser(email);

            notification.User = new MongoDBRef("users", user.Id);
            var notificationsCollection = db.GetCollection<Notification>("notifications");
            var usersCollection = db.GetCollection<User>("users");

            notificationsCollection.InsertOne(notification);
            user.Notifications.Add(new MongoDBRef("notifications", notification.Id));

            var update = Builders<User>.Update.Set("Notifications", user.Notifications);
            var filter = Builders<User>.Filter.Eq("Email", email);

            usersCollection.UpdateOne(filter, update);

            return notification.Id;
        }

        public void UpdateNotification(ObjectId notId, string tag)
        {
            var notificationsCollection = db.GetCollection<Notification>("notifications");

            var filter = Builders<Notification>.Filter.Eq("_id", notId);
            var update = Builders<Notification>.Update.Set("Tag", tag);

            notificationsCollection.UpdateOne(filter, update);
        }

        public void UpdateNotificationStatus(ObjectId notId)
        {
            var notificationsCollection = db.GetCollection<Notification>("notifications");

            var filter = Builders<Notification>.Filter.Eq("_id", notId);
            var update = Builders<Notification>.Update.Set("Read", true);

            notificationsCollection.UpdateOne(filter, update);
        }

        public void UpdateReview(ObjectId id, double grade, string comment)
        {
            var reviewsCollection = db.GetCollection<Review>("reviews");

            var filter = Builders<Review>.Filter.Eq("_id", id);
            var update = Builders<Review>.Update.Set("Grade", grade)
                                                   .Set("Comment", comment);

            reviewsCollection.UpdateOne(filter, update);
        }

        public List<User> GetUsers()
        {
            var usersCollection = db.GetCollection<User>("users");

            var filter = Builders<User>.Filter.Ne("Name", "Admin");

            return usersCollection.Find(filter).ToList();
        }

        public List<Product> GetProducts()
        {
            var productsCollection = db.GetCollection<Product>("products");

            return productsCollection.Find(Builders<Product>.Filter.Empty).ToList();
        }

        public List<string> GetCategories()
        {
            List<string> subcategories = new List<string>();
            var categoriesCollection = db.GetCollection<Category>("categories");

            List<Category> categories= categoriesCollection.Find(Builders<Category>.Filter.Empty).ToList();

            foreach (Category c in categories)
                subcategories.AddRange(c.Subcategories);

            return subcategories;
        }

        public void InsertAd(Advert advert)
        {
            var advertsCollection = db.GetCollection<Advert>("adverts");

            advertsCollection.InsertOne(advert);
        }
    }
}
