using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.ShasavicObject
{
    /// <summary>
    /// 鳴らすノーツの音階（音高線）を指定するためのクラス。
    /// 初期化時に、音高線の周波数が計算される。
    /// </summary>
    public record ShasavicTone
    {
        /// <summary>
        /// 底音の周波数
        /// </summary>
        public float BaseFreq { get; init; }

        /// <summary>
        /// 組成式
        /// </summary>
        public ImmutableArray<int> Formula { get; init; }

        /// <summary>
        /// 音高線の周波数
        /// </summary>
        public float ResultFreq { get; init; }

        /// <summary>
        /// 鳴らすノートの音高線の、底音と組成式を指定する。
        /// 同時に、音高線の周波数(ResultFreq)が計算される。
        /// </summary>
        /// <param name="baseFreq">底音の周波数</param>
        /// <param name="formula">組成式</param>
        public ShasavicTone(float baseFreq, IEnumerable<int> formula)
        {
            List<int> tmp = [.. formula.Take(DimensionInfo.MaxDimension)];

            while (tmp.Count < DimensionInfo.MaxDimension)
                tmp.Add(0);

            BaseFreq = baseFreq;
            Formula = [.. tmp];

            uint n = 1, d = 1;

            for (int i = 0; i < DimensionInfo.MaxDimension; i++)
            {
                if (Formula[i] < 0)
                {
                    for (int j = 0; j > Formula[i]; j--)
                    {
                        n *= DimensionInfo.Coefs[i].D;
                        d *= DimensionInfo.Coefs[i].N;
                    }
                }
                else
                {
                    for (int j = 0; j < Formula[i]; j++)
                    {
                        n *= DimensionInfo.Coefs[i].N;
                        d *= DimensionInfo.Coefs[i].D;
                    }
                }
            }

            ResultFreq = (float)n / d;
        }
    }
}
