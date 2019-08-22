using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Objects;
using RecommendationEngine.Parsers;
using Databases;

namespace RecommendationEngine.Recommenders
{
    public class ItemCollaborativeFilterRecommender
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

            // Precompute a transposed ratings matrix where each row becomes a product and each column a user or category
            for (int a = 0; a < ratings.ProductIndexToID.Count; a++)
            {
                transposedRatings[a] = new double[features];

                for (int f = 0; f < features; f++)
                {
                    transposedRatings[a][f] = ratings.Users[f].ProductRatings[a];
                }
            }
        }

        public List<Suggestion> GetSuggestions(ObjectId userId, int numSuggestions)
        {
            int userIndex = ratings.UserIndexToID.IndexOf(userId);
            List<int> products = GetHighestRatedProductsForUser(userIndex).Take(5).ToList();
            List<Suggestion> suggestions = new List<Suggestion>();

            foreach (int productIndex in products)
            {
                ObjectId productId = ratings.ProductIndexToID[productIndex];
                List<ProductRating> neighboringProducts = GetNearestNeighbors(productId, neighborCount);

                foreach (ProductRating neighbor in neighboringProducts)
                {
                    int neighborProductIndex = ratings.ProductIndexToID.IndexOf(neighbor.ProductID);
                    var nonZeroRatings = transposedRatings[neighborProductIndex].Where(x => x != 0);
                    double averageProductRating = nonZeroRatings.Count() > 0 ? nonZeroRatings.Average()+neighbor.Rating : 0;

                    if(!suggestions.Exists(x=>x.ProductID.Equals(neighbor.ProductID)) && !products.Exists(x=>x==neighborProductIndex))
                        suggestions.Add(new Suggestion(userId, neighbor.ProductID, averageProductRating));
                }
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));
            return suggestions.Take(numSuggestions).ToList();
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            ratings = ubt.GetUserProductRatingsTable(rater);

            List<ProductCategoryCount> productCategories = ubt.GetProductCategoryCounts();
            ratings.AppendProductFeatures(productCategories);

            FillTransposedRatings();
        }

        private List<int> GetHighestRatedProductsForUser(int userIndex)
        {
            List<Tuple<int, double>> items = new List<Tuple<int, double>>();

            for (int productIndex = 0; productIndex < ratings.ProductIndexToID.Count; productIndex++)
            {
                // Create a list of every product this user has viewed
                if (ratings.Users[userIndex].ProductRatings[productIndex] != 0)
                {
                    items.Add(new Tuple<int, double>(productIndex, ratings.Users[userIndex].ProductRatings[productIndex]));
                }
            }

            // Sort the products by rating
            items.Sort((c, n) => n.Item2.CompareTo(c.Item2));

            return items.Select(x => x.Item1).ToList();
        }

        private List<ProductRating> GetNearestNeighbors(ObjectId productId, int numArticles)
        {
            List<ProductRating> neighbors = new List<ProductRating>();
            int mainProductIndex = ratings.ProductIndexToID.IndexOf(productId);

            for (int productIndex = 0; productIndex < ratings.ProductIndexToID.Count; productIndex++)
            {
                ObjectId searchProductId = ratings.ProductIndexToID[productIndex];
                if (productIndex != mainProductIndex)
                {                
                    double score = comparer.CompareVectors(transposedRatings[mainProductIndex], transposedRatings[productIndex]);

                    neighbors.Add(new ProductRating(searchProductId, score));
                }
                else
                {
                    double score = double.NegativeInfinity;
                    neighbors.Add(new ProductRating(searchProductId, score));
                }
            }

            neighbors.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return neighbors.Take(numArticles).ToList();
        }

        public List<Suggestion> GetFirstSuggestions(UserBehaviorDatabase db, Databases.DomainModel.User user, int numSuggestions)
        {
            var userActionGroup = db.UserActions
                .GroupBy(x => new { x.UserID })
                .Select(g => new { g.Key.UserID })
                .ToList();

            List<Suggestion> suggestions = new List<Suggestion>();
            MongodbFunctions mongo = new MongodbFunctions();

            foreach (var a in userActionGroup)
            {
                Databases.DomainModel.User u = mongo.GetUser(a.UserID);

                if (u.Gender.Equals(user.Gender) || u.BirthDate.Year == user.BirthDate.Year)
                {
                    int userIndex = ratings.UserIndexToID.IndexOf(u.Id);
                    List<int> products = GetHighestRatedProductsForUser(userIndex).Take(3).ToList();

                    foreach (int productIndex in products)
                    {
                        ObjectId productId = ratings.ProductIndexToID[productIndex];
                        suggestions.Add(new Suggestion(u.Id, productId, ratings.Users[userIndex].ProductRatings[productIndex]));
                    }
                }
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return suggestions.Take(numSuggestions).ToList();
        }
    }
}
