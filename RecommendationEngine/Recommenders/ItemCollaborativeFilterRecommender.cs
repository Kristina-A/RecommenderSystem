using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Objects;
using RecommendationEngine.Parsers;

namespace RecommendationEngine.Recommenders
{
    public class ItemCollaborativeFilterRecommender : IRecommender
    {
        private IComparer comparer;
        private IRater rater;
        private UserProductRatingsTable ratings;
        private double[][] transposedRatings;

        private int neighborCount;

        public ItemCollaborativeFilterRecommender(IComparer itemComparer, IRater implicitRater, int numberOfNeighbors)
        {
            comparer = itemComparer;
            rater = implicitRater;
            neighborCount = numberOfNeighbors;
        }

        private void FillTransposedRatings()
        {
            int features = ratings.Users.Count;
            transposedRatings = new double[ratings.ProductIndexToID.Count][];

            // Precompute a transposed ratings matrix where each row becomes an article and each column a user or tag
            for (int a = 0; a < ratings.ProductIndexToID.Count; a++)
            {
                transposedRatings[a] = new double[features];

                for (int f = 0; f < features; f++)
                {
                    transposedRatings[a][f] = ratings.Users[f].ProductRatings[a];
                }
            }
        }
        public double GetRating(ObjectId userId, ObjectId productId)
        {
            throw new NotImplementedException();
        }

        public List<Suggestion> GetSuggestions(ObjectId userId, int numSuggestions)
        {
            throw new NotImplementedException();
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            ratings = ubt.GetUserProductRatingsTable(rater);

            List<ProductCategoryCount> articleTags = ubt.GetProductCategoryCounts();
            ratings.AppendProductFeatures(articleTags);

            FillTransposedRatings();
        }
    }
}
