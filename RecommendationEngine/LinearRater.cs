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
        private double viewWeight;
        private double buyWeight;
        private double reviewWeight;
        private double seeReviewsWeight;

        public LinearRater(double view, double buy)
        {
            viewWeight = view;
            buyWeight = buy;
            reviewWeight = 0.0;
            seeReviewsWeight = 0.0;
        }

        public double GetRating(List<UserAction> actions)
        {
            if (actions.Where(x=>x.Action=="Review").First()!=null)
                reviewWeight = actions.Where(x => x.Action == "Review").First().Rate;//ocena kojom je ocenio, samo jednom moze da oceni

            int view = actions.Count(x => x.Action == "View");
            int buy = actions.Count(x => x.Action == "Buy");
            int seeRev = actions.Count(x => x.Action == "SeeReviews");

            if (seeRev != 0)
                seeReviewsWeight = actions.Where(x => x.Action == "SeeReviews").First().Rate;//prosecna ocena svih reviewa

            double rating = view * viewWeight + buy * buyWeight + reviewWeight + seeRev * seeReviewsWeight;

            return rating;
        }
    }
}
