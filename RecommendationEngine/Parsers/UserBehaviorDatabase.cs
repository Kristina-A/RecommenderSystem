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
        public List<Databases.DomainModel.Category> Categories { get; set; }

        public List<Databases.DomainModel.Product> Products { get; set; }

        public List<Databases.DomainModel.User> Users { get; set; }

        public List<UserAction> UserActions { get; set; }
    }
}
