using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class Review
    {
        public ObjectId Id { get; set; }
        public double Grade { get; set; }
        public string Comment { get; set; }
        public MongoDBRef User { get; set; }
        public MongoDBRef Product { get; set; }
    }
}
