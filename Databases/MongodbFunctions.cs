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
    }
}
