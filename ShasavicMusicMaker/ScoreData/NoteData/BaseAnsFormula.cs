using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    /// <summary>
    /// 
    /// </summary>
    internal record BaseAnsFormula
    {
        /// <summary>
        /// 現バージョンにおける最大次元数
        /// </summary>
        public static readonly int MaxDimension = 5;

        /// <summary>
        /// 底音
        /// </summary>
        public Arm Base { get; init; }

        /// <summary>
        /// 組成式
        /// </summary>
        public ImmutableArray<int> Formula { get; init; }

        public BaseAnsFormula(Arm _base, IEnumerable<int> formula)
        {
            List<int> tmp = [.. formula.Take(MaxDimension)];
            
            while (tmp.Count < MaxDimension)
                tmp.Add(0);

            Base = _base;
            Formula = [.. tmp];
        }

        /// <summary>
        /// 渡されたArmの底音と組成式を導出する。
        /// </summary>
        /// <param name="arm">底音と組成式を求めたい Arm</param>
        /// <returns>渡された Arm の底音と組成式</returns>
        public static BaseAnsFormula CalcBafOfArm(Arm arm)
        {
            Arm _base = arm;
            int[] formula = new int[MaxDimension];

            while (true)
            {
                formula[_base.Bcp.Dimension] += _base.Bcp.Scending ? 1 : -1;

                if (_base.Body is null)
                    break;

                _base = _base.Body;
            }

            return new(_base, formula);
        }
    }
}
