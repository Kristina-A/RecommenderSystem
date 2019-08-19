using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecommendationEngine.Interfaces;

namespace RecommendationEngine
{
    public class CosineComparer : IComparer
    {
        public double CompareVectors(double[] vectorOne, double[] vectorTwo)
        {
            double sumProduct = 0.0;
            double sumOneSquared = 0.0;
            double sumTwoSquared = 0.0;

            for (int i = 0; i < vectorOne.Length; i++)
            {
                sumProduct += vectorOne[i] * vectorTwo[i];
                sumOneSquared += Math.Pow(vectorOne[i], 2);
                sumTwoSquared += Math.Pow(vectorTwo[i], 2);
            }

            double intensities = Math.Sqrt(sumOneSquared) * Math.Sqrt(sumTwoSquared);

            if (sumProduct == 0 && intensities == 0)
                return 0;
            else
                return sumProduct / (Math.Sqrt(sumOneSquared) * Math.Sqrt(sumTwoSquared));
        }
    }
}
