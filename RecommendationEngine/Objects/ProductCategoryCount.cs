using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Objects
{
    public class ProductCategoryCount
    {
        public string ProductID { get; set; }

        public double[] CategoryCounts { get; set; }

        public ProductCategoryCount(string ProductId, int numCategorys)
        {
            ProductID = ProductId;
            CategoryCounts = new double[numCategorys];
        }
    }
}
