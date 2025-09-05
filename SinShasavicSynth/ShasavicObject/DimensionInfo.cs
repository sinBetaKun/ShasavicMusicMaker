using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal class DimensionInfo
    {
        /// <summary>
        /// このシンセで扱える最大の次元。
        /// または、次元の数。
        /// </summary>
        public static readonly int MaxDimension = 6;

        /// <summary>
        /// 各次元の倍率
        /// </summary>
        public static readonly ImmutableArray<Fraction> Coefs = 
            [
            new(2,1),
            new(3,2),
            new(5,4),
            new(7,4),
            new(11,4),
            new(13,4),
            ];
    }
}
