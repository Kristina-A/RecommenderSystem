using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Objects;
using RecommendationEngine.Parsers;
using MongoDB.Bson;

namespace RecommendationEngine.Interfaces
{
    public interface IRecommender
    {
        void Train(UserBehaviorDatabase db);

        List<Suggestion> GetSuggestions(ObjectId userId, int numSuggestions);

        double GetRating(ObjectId userId, ObjectId productId);//predikcija da li ce korisniku da se svidja preporucen proizvod
    }
}
