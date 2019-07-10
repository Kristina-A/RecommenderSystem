using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class Message
    {
        public ObjectId Id { get; set; }
        public string Content { get; set; }
        public MongoDBRef User { get; set; }
        public MongoDBRef Product { get; set; }
        public List<MongoDBRef> Responses { get; set; }

        public Message()
        {
            Responses = new List<MongoDBRef>();
        }
    }
}
