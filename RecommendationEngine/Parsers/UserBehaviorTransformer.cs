using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Objects;
using RecommendationEngine.Interfaces;
using MongoDB.Bson;
using Databases;

namespace RecommendationEngine.Parsers
{
    public class UserBehaviorTransformer
    {
        private UserBehaviorDatabase db;

        public UserBehaviorTransformer(UserBehaviorDatabase database)
        {
            db = database;
        }

        /// <summary>
        /// Get a list of all users and their ratings on every article
        /// </summary>
        public UserProductRatingsTable GetUserProductRatingsTable(IRater rater)
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

            var userProductRatingGroup = db.UserActions
                .GroupBy(x => new { x.UserID, x.ProductID })
                .Select(g => new { g.Key.UserID, g.Key.ProductID, Rating = rater.GetRating(g.ToList()) })
                .ToList();

            foreach (var userAction in userProductRatingGroup)
            {
                int userIndex = table.UserIndexToID.IndexOf(userAction.UserID);
                int productIndex = table.ProductIndexToID.IndexOf(userAction.ProductID);

                table.Users[userIndex].ProductRatings[productIndex] = userAction.Rating;
            }

            return table;
        }

        /// <summary>
        /// Get a table of all articles as rows and all tags as columns
        /// </summary>
        public List<ProductCategoryCount> GetProductCategoryCounts()
        {
            List<ProductCategoryCount> productCategories = new List<ProductCategoryCount>();

            foreach (Databases.DomainModel.Product product in db.Products)
            {
                ProductCategoryCount prodCategory = new ProductCategoryCount(product.Id, db.Categories.Count);

                for (int category = 0; category < db.Categories.Count; category++)
                {
                    prodCategory.CategoryCounts[category] = product.Subcategory.Equals(db.Categories[category]) ? 1.0 : 0.0;
                }

                productCategories.Add(prodCategory);
            }

            return productCategories;
        }
    }
}
