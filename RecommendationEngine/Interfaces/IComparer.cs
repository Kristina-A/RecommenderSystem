using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommendationEngine.Interfaces
{
    public interface IComparer
    {
        double CompareVectors(double[] vectorOne, double[] vectorTwo);//uzima dva vektora, ili korisnici ili proizvodi i vraca slicnost
    }
}
