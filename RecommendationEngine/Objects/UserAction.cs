using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Objects
{
    public class UserAction
    {
        public string Action { get; set; }

        public string UserID { get; set; }

        public string ArticleID { get; set; }

        public UserAction(string action, string userid, string articleid)
        {
            Action = action;
            UserID = userid;
            ArticleID = articleid;
        }

        //public override string ToString()
        //{
        //    return Day + "," + Action + "," + UserID + "," + UserName + "," + ArticleID + "," + ArticleName;
        //}
    }
}
