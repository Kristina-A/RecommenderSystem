using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Objects;

namespace RecommendationEngine.Interfaces
{
    public interface IRecommender
    {
        void Train(UserBehaviorDatabase db);

        List<Suggestion> GetSuggestions(int userId, int numSuggestions);

        double GetRating(int userId, int productId);
    }
}
