using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Interfaces;

namespace RecommendationEngine.Objects
{
    public class UserProductRatingsTable
    {
        public List<UserProductRatings> Users { get; set; }

        public List<int> UserIndexToID { get; set; }

        public List<int> ProductIndexToID { get; set; }

        public UserProductRatingsTable()
        {
            Users = new List<UserProductRatings>();
            UserIndexToID = new List<int>();
            ProductIndexToID = new List<int>();
        }

        public void AppendUserFeatures(double[][] userFeatures)
        {
            for (int i = 0; i < UserIndexToID.Count; i++)
            {
                Users[i].AppendRatings(userFeatures[i]);
            }
        }

        public void AppendProductFeatures(double[][] productFeatures)
        {
            for (int f = 0; f < productFeatures[0].Length; f++)
            {
                UserProductRatings newFeature = new UserProductRatings("", ProductIndexToID.Count);

                for (int a = 0; a < ProductIndexToID.Count; a++)
                {
                    newFeature.ProductRatings[a] = productFeatures[a][f];
                }

                Users.Add(newFeature);
            }
        }

        internal void AppendProductFeatures(List<ProductCategoryCount> productCategories)
        {
            double[][] features = new double[productCategories.Count][];

            for (int a = 0; a < productCategories.Count; a++)
            {
                features[a] = new double[productCategories[a].CategoryCounts.Length];

                for (int f = 0; f < productCategories[a].CategoryCounts.Length; f++)
                {
                    features[a][f] = productCategories[a].CategoryCounts[f];
                }
            }

            AppendProductFeatures(features); 
        }

    }
}
