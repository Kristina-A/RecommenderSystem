using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Objects;
using RecommendationEngine.Interfaces;
using MongoDB.Bson;

namespace RecommendationEngine.Parsers
{
    public class UserBehaviorTransformer
    {
        private UserBehaviorDatabase db;

        public UserBehaviorTransformer(UserBehaviorDatabase database)
        {
            db = database;
        }

        public UserProductRatingsTable GetUserArticleRatingsTable(IRater rater)
        {
            UserProductRatingsTable table = new UserProductRatingsTable();

            table.UserIndexToID = db.Users.OrderBy(x => x.Id)
                .Select(x => x.Id).Distinct().ToList();
            table.ProductIndexToID = db.Products.OrderBy(x => x.Id)
                .Select(x => x.Id).Distinct().ToList();

            foreach (ObjectId userId in table.UserIndexToID)
            {
                table.Users.Add(new UserProductRatings(userId, table.ProductIndexToID.Count));
            }

            var userArticleRatingGroup = db.UserActions
                .GroupBy(x => new { x.UserID, x.ProductID })
                .Select(g => new { g.Key.UserID, g.Key.ProductID, Rating = rater.GetRating(g.ToList()) })
                .ToList();

            foreach (var userAction in userArticleRatingGroup)
            {
                int userIndex = table.UserIndexToID.IndexOf(userAction.UserID);
                int articleIndex = table.ProductIndexToID.IndexOf(userAction.ProductID);

                table.Users[userIndex].ProductRatings[articleIndex] = userAction.Rating;
            }

            return table;
        }
    }
}
