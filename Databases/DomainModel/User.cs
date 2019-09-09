using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<string> Address { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> Interests { get; set; }

        public List<MongoDBRef> Orders { get; set; }

        public List<MongoDBRef> Reviews { get; set; }

        public List<MongoDBRef> Messages { get; set; }

        public List<MongoDBRef> Notifications { get; set; }

        public User()
        {
            Orders = new List<MongoDBRef>();
            Reviews = new List<MongoDBRef>();
            Messages = new List<MongoDBRef>();
            Notifications = new List<MongoDBRef>();
            Address = new List<string>();
            Interests = new List<string>();
        }
    }
}
