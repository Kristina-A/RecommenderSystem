using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class Category
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<string> Subcategories { get; set; }
        public List<MongoDBRef> Products { get; set; }

        public Category()
        {
            Products = new List<MongoDBRef>();
            Subcategories = new List<string>();
        }
    }
}
