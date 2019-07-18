using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace RecommendationEngine.Objects
{
    public class UserAction
    {
        public string Action { get; set; }

        public ObjectId UserID { get; set; }

        public ObjectId ProductID { get; set; }

        public UserAction(string action, ObjectId userid, ObjectId productid)
        {
            Action = action;
            UserID = userid;
            ProductID = productid;
        }

        //public override string ToString()
        //{
        //    return Day + "," + Action + "," + UserID + "," + UserName + "," + ArticleID + "," + ArticleName;
        //}
    }
}
