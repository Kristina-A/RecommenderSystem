using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Objects;

namespace RecommendationEngine.Interfaces
{
    public interface IRater
    {
        double GetRating(List<UserAction> actions);
    }
}
