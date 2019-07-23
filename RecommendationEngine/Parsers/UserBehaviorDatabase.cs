using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Databases;
using RecommendationEngine.Objects;

namespace RecommendationEngine.Parsers
{
    public class UserBehaviorDatabase
    {
        public List<string> Categories { get; set; }

        public List<Databases.DomainModel.Product> Products { get; set; }

        public List<Databases.DomainModel.User> Users { get; set; }

        public List<UserAction> UserActions { get; set; }

        public UserBehaviorDatabase()
        {
            Categories = new List<string>();
            Products = new List<Databases.DomainModel.Product>();
            Users = new List<Databases.DomainModel.User>();
            UserActions = new List<UserAction>();
        }
    }
}
