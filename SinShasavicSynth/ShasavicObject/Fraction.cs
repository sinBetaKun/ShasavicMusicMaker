using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal record Fraction
    {
        public int N { get; init; }

        public int D { get; init; }

        public Fraction(int n, int d)
        {
            N = n;
            D = d;
        }
    }
}
