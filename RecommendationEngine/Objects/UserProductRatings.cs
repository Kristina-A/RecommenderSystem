using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Objects
{
    public class UserProductRatings
    {
        public string UserID { get; set; }

        public double[] ProductRatings { get; set; }

        public double Score { get; set; }

        public UserProductRatings(string userId, int articlesCount)
        {
            UserID = userId;
            ProductRatings = new double[articlesCount];
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
