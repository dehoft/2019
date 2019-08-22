using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelektikos_Projektas
{
    class Gini
    {
        public string Collumn;
        public double GiniIndex;
        public Gini(string collumn, double giniIndex)
        {
            this.Collumn = collumn;
            this.GiniIndex = giniIndex;
        }
        public override string ToString()
        {
            return String.Format("{0} {1:0.000}", Collumn, GiniIndex);
        }
    }
}
