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
    public class UserCollaborativeFilterRecommender
    {
        private IComparer comparer;
        private IRater rater;
        private UserProductRatingsTable ratings;

        private int neighborCount;

        public UserCollaborativeFilterRecommender(IComparer userComparer, IRater implicitRater, int numberOfNeighbors)
        {
            comparer = userComparer;
            rater = implicitRater;
            neighborCount = numberOfNeighbors;
        }

        public List<Suggestion> GetSuggestions(ObjectId userId, int numSuggestions)
        {
            int userIndex = ratings.UserIndexToID.IndexOf(userId);
            UserProductRatings user = ratings.Users[userIndex];
            List<Suggestion> suggestions = new List<Suggestion>();

            var neighbors = GetNearestNeighbors(user, neighborCount);

            for (int productIndex = 0; productIndex < ratings.ProductIndexToID.Count; productIndex++)
            {
                // If the user in question hasn't rated the given product yet
                if (user.ProductRatings[productIndex] == 0)
                {
                    double score = 0.0;
                    int count = 0;
                    for (int u = 0; u < neighbors.Count; u++)
                    {
                        if (neighbors[u].ProductRatings[productIndex] != 0)
                        {
                            // Calculate the weighted score for this product   
                            score += neighbors[u].ProductRatings[productIndex];
                            count++;
                        }
                    }
                    if (count > 0)
                    {
                        score /= count;
                    }

                    suggestions.Add(new Suggestion(userId, ratings.ProductIndexToID[productIndex], score));
                }
            }

            suggestions.Sort((c, n) => n.Rating.CompareTo(c.Rating));

            return suggestions.Take(numSuggestions).ToList();
        }

        public void Train(UserBehaviorDatabase db)
        {
            UserBehaviorTransformer ubt = new UserBehaviorTransformer(db);
            ratings = ubt.GetUserProductRatingsTable(rater);
        }

        private List<UserProductRatings> GetNearestNeighbors(UserProductRatings user, int numUsers)
        {
            List<UserProductRatings> neighbors = new List<UserProductRatings>();

            for (int i = 0; i < ratings.Users.Count; i++)
            {
                if (ratings.Users[i].UserID == user.UserID)
                {
                    ratings.Users[i].Score = double.NegativeInfinity;
                }
                else
                {
                    ratings.Users[i].Score = comparer.CompareVectors(ratings.Users[i].ProductRatings, user.ProductRatings);
                }
            }

            var similarUsers = ratings.Users.OrderByDescending(x => x.Score);

            return similarUsers.Take(numUsers).ToList();
        }
    }
}
