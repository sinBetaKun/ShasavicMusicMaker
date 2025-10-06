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
        /// 第N成分に(N+1)次元の倍数が収められている
        /// </summary>
        public static ImmutableArray<float> Coefs { get; private set; } = [
            2f,
            3 / 2f,
            5 / 4f,
            7 / 4f,
            11 / 4f,
            13 / 4f
            ];
    }
}
