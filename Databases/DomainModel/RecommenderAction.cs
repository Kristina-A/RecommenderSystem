using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Databases.DomainModel
{
    public class RecommenderAction
    {
        public ObjectId UserId { get; set; }
        public ObjectId ProductId { get; set; }
        public string Action { get; set; }
        public double Grade { get; set; }

        public RecommenderAction()
        {
            Grade = 0;
        }
    }
}
