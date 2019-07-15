using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class Notification
    {
        public ObjectId Id { get; set; }
        public string Content { get; set; }
        public string Tag { get; set; }
        public bool Read { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public MongoDBRef User { get; set; }
    }
}
