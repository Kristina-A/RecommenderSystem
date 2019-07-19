using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Objects;

namespace RecommendationEngine
{
    public class LinearRater : IRater
    {
        public double GetRating(List<UserAction> actions)
        {
            throw new NotImplementedException();
        }
    }
}
