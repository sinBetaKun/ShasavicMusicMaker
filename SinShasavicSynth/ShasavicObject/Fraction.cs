using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal record Fraction
    {
        public uint N { get; init; }

        public uint D { get; init; }

        public Fraction(uint n, uint d)
        {
            N = n; D = d;
        }
    }
}
