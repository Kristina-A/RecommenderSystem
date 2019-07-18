using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace RecommendationEngine.Objects
{
    public class Suggestion
    {
        public ObjectId UserID { get; set; }

        public ObjectId ProductID { get; set; }

        public double Rating { get; set; }

        public Suggestion(ObjectId userId, ObjectId productId, double assurance)
        {
            UserID = userId;
            ProductID = productId;
            Rating = assurance;
        }
    }
}
