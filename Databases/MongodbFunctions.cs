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
    }
}
