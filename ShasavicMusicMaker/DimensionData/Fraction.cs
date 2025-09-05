using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShasavicMusicMaker.DimensionData
{
    internal class Fraction
    {
        public int N { get; init; }

        public int D { get; init; }

        public Fraction(int n, int d)
        {
            N = n;
            D = d;
        }

        public float ToFloat() => (float)N / D;

        public static Fraction CalcBigFraction(IEnumerable<Fraction> ns, IEnumerable<Fraction> ds)
        {
            int n = 1, d = 1;

            foreach (Fraction f in ns)
            {
                n *= f.N;
                d *= f.D;
            }

            foreach (Fraction f in ds)
            {
                n *= f.D;
                d *= f.N;
            }

            Fraction ret = new(n, d);
            return ret;
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new(a.N * b.N, a.D * b.D);
        }

        public static Fraction operator *(Fraction a, int b)
        {
            return new(a.N * b, a.D);
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            return new(a.N * b.D, a.D * b.N);
        }
    }
}
