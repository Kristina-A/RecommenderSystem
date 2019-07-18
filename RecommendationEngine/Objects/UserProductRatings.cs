using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace RecommendationEngine.Objects
{
    public class UserProductRatings
    {
        public ObjectId UserID { get; set; }

        public double[] ProductRatings { get; set; }

        public double Score { get; set; }

        public UserProductRatings(ObjectId userId, int productsCount)
        {
            UserID = userId;
            ProductRatings = new double[productsCount];
        }

        public void AppendRatings(double[] ratings)
        {
            List<double> allRatings = new List<double>();

            allRatings.AddRange(ProductRatings);
            allRatings.AddRange(ratings);

            ProductRatings = allRatings.ToArray();
        }
    }
}
