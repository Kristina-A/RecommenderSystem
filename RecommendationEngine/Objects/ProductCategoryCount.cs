using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace RecommendationEngine.Objects
{
    public class ProductCategoryCount
    {
        public ObjectId ProductID { get; set; }

        public double[] CategoryCounts { get; set; }

        public ProductCategoryCount(ObjectId ProductId, int numCategories)
        {
            ProductID = ProductId;
            CategoryCounts = new double[numCategories];
        }
    }
}
