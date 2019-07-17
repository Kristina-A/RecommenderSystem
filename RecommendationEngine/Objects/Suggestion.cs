using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Objects
{
    public class Suggestion
    {
        public string UserID { get; set; }

        public string ArticleID { get; set; }

        public double Rating { get; set; }

        public Suggestion(string userId, string articleId, double assurance)
        {
            UserID = userId;
            ArticleID = articleId;
            Rating = assurance;
        }
    }
}
