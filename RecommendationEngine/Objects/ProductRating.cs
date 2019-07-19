using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace RecommendationEngine.Objects
{
    public class ProductRating
    {
        public ObjectId ProductID { get; set; }

        public double Rating { get; set; }

        public ProductRating(ObjectId productId, double rating)
        {
            ProductID = productId;
            Rating = rating;
        }
    }
}
