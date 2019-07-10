using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class AdminResponse
    {
        public ObjectId Id { get; set; }
        public string Content { get; set; }
        public MongoDBRef Message { get; set; }
    }
}
