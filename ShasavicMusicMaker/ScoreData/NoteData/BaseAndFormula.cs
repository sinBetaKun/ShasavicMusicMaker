using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShasavicMusicMaker.DimensionData;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    /// <summary>
    /// 
    /// </summary>
    internal class BaseAndFormula
    {
        /// <summary>
        /// 底音
        /// </summary>
        public Arm Base { get; init; }

        /// <summary>
        /// 組成式
        /// </summary>
        public int[] Formula { get; init; }

        public BaseAndFormula(Arm _base, IEnumerable<int> formula)
        {
            // 組成式から、現バージョンで対応している最大の次元までの上下符の数を取得
            List<int> tmp = [.. formula.Take(DimensionInfo.MaxDimension)];
            
            // もし最大次元数まで取得できなければ、最大次元数まで0を付け足す。
            while (tmp.Count < DimensionInfo.MaxDimension)
                tmp.Add(0);

            Base = _base;
            Formula = [.. tmp];
        }

        /// <summary>
        /// 組成式から、対象とする音高線の
        /// </summary>
        /// <returns></returns>
        public float CalcCoef()
        {
            float n = 1;
            float d = 1;

            for (int i = 0; i < DimensionInfo.MaxDimension; i++)
            {
                if (Formula[i] > 0)
                {
                    n *= MathF.Pow(DimensionInfo.Coefs[i], Formula[i]);
                }
                else
                {
                    d *= MathF.Pow(DimensionInfo.Coefs[i], -Formula[i]);
                }
            }

            return n / d;
        }

        /// <summary>
        /// 渡されたArmの底音と組成式を導出する。
        /// </summary>
        /// <param name="arm">底音と組成式を求めたい Arm</param>
        /// <returns>渡された Arm の底音と組成式</returns>
        public static BaseAndFormula CalcBaseAndFomulaOfArm(Arm arm)
        {
            Arm _base = arm;
            int[] formula = new int[DimensionInfo.MaxDimension];

            while (_base.Bcp.Dimension > 0)
            {
                formula[_base.Bcp.Dimension - 1] += _base.Bcp.Scending ? 1 : -1;

                if (_base.Body is null)
                    throw new Exception("failure to calc base and fomula");

                _base = _base.Body;
            }

            return new(_base, formula);
        }

        /// <summary>
        /// 渡されたScoreLineの底音と組成式を導出する。
        /// </summary>
        /// <param name="scrLine">底音と組成式を求めたい ScoreLine</param>
        /// <returns>渡された ScoreLine の底音と組成式</returns>
        public static BaseAndFormula CalcBaseAndFomulaOfScoreLine(ScoreLine scrLine)
        {
            BaseAndFormula bas = CalcBaseAndFomulaOfArm(scrLine.Body);
            bas.Formula[scrLine.Dimension] += scrLine.Sceding;
            return bas;
        }

        /// <summary>
        /// その腕が持つ子要素の腕を末端まで全て組成式とともに取得する。
        /// </summary>
        /// <param name="arm">調べたい腕</param>
        /// <returns>全子要素の組成式と腕本体</returns>
        public static Dictionary<BaseAndFormula, Arm> GetArmsDictOfChordonym(Arm arm)
        {
            Arm baseArm = CalcBaseAndFomulaOfArm(arm).Base;
            Arm[] arms1 = [baseArm];
            List<Arm> arms2 = [];
            Dictionary<BaseAndFormula, Arm> ret = [];

            while (arms1.Length > 0)
            {
                foreach (Arm arm2 in arms1)
                {
                    BaseAndFormula baf = CalcBaseAndFomulaOfArm(arm2);
                    ret.Add(baf, arm2);
                    arms2.AddRange(arm2.Arms);
                }

                arms1 = [.. arms2];
                arms2.Clear();
            }

            return ret;
        }
    }
}
