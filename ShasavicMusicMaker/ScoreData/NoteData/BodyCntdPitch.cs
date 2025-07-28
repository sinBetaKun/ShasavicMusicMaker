using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShasavicMusicMaker.Enum;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    /// <summary>
    /// 体始音高 (body-centered pitch)
    /// 対象の腕とその体との間の音高差を表す。
    /// 上下符は１つまでしか持てない。
    /// </summary>
    internal record BodyCntdPitch
    {
        /// <summary>
        /// 次元符。底音の時のみ 0 で原音を表す。
        /// コンストラクタに無効な次元が渡されたときは -1。
        /// </summary>
        public int Dimension { get; init; }

        /// <summary>
        /// 上下符の向き。trueのときに上向き。
        /// </summary>
        public bool Scending { get; init; }

        /// <summary>
        /// 次元符と上下符の向きを設定する。
        /// </summary>
        /// <param name="dimension">次元符</param>
        /// <param name="scending">上下符の向き（上向きならtrue）</param>
        public BodyCntdPitch(int dimension, bool scending)
        {
            if (dimension < 0 || dimension > BaseAnsFormula.MaxDimension)
            {
                Dimension = -1;
            }
            else
            {
                Dimension = dimension;
                Scending = scending;
            }
        }

        /// <summary>
        /// 複製。
        /// </summary>
        /// <param name="origin">複製元</param>
        public BodyCntdPitch(BodyCntdPitch origin)
        {
            Dimension = origin.Dimension;
            Scending = origin.Scending;
        }
    }
}
