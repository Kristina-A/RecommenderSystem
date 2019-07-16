using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Databases.DomainModel
{
    public class CheckoutDetails
    {
        public User User { get; set; }
        public List<Product> Products { get; set; }
        public List<Notification> Discounts { get; set; }

        public CheckoutDetails()
        {
            Products = new List<Product>();
            Discounts = new List<Notification>();
        }
    }
}
