using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Databases.DomainModel
{
    public class Advert
    {
        public ObjectId Id { get; set; }
        public string Picture { get; set; }
        public List<string> Subcategories { get; set; }

        public Advert()
        {
            Subcategories = new List<string>();
        }
    }
}
