using ShasavicMusicMaker.DimensionData;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    /// <summary>
    /// 次元譜を表すクラス。
    /// 基準となる腕と、その腕からどの次元にいくら上下した次元譜なのかを示すフィールドを持つ。
    /// </summary>
    internal class ScoreLine
    {
        /// <summary>
        /// 基準となる腕
        /// </summary>
        public Arm Body { get; init; }

        /// <summary>
        /// 次元符 (0で1d, 1で2d, 2で3d)
        /// </summary>
        public int Dimension { get; init; }

        /// <summary>
        /// 上下符 (上方向を正とする)
        /// </summary>
        public int Sceding { get; init; }

        /// <summary>
        /// 次元譜のデータを作成。
        /// </summary>
        /// <param name="body">基準となる腕</param>
        /// <param name="dim">次元譜 (0で1d, 1で2d, 2で3d)</param>
        /// <param name="scd">上下符 (上方向を正とする)</param>
        public ScoreLine(Arm body, int dim, int scd)
        {
            Body = body;
            Dimension = dim;
            Sceding = scd;
        }

        /// <summary>
        /// 自身を実際にコードニムに腕として追加する。
        /// </summary>
        /// <returns>自身を表す腕</returns>
        /// <exception cref="Exception"></exception>
        public Arm AddToCordonym()
        {
            int scdCount;
            bool scdSign;

            if (Sceding > 0)
            {
                scdCount = Sceding;
                scdSign = true;
            }
            else
            {
                scdCount = -Sceding;
                scdSign = false;
            }

            Arm target = Body;

            for (int i = 0; i < scdCount; i++)
            {
                (bool, Arm) result = target.AddArm(new(Dimension + 1, scdSign));

                if (!result.Item1)
                    throw new Exception("Couldn't add an arm based on a scoreline.");

                target = result.Item2;
            }

            return target;
        }

        /// <summary>
        /// 底音との周波数比を取得するためのメソッド。
        /// </summary>
        /// <returns>底音との周波数比</returns>
        public float CalcCoefFromBase()
        {
            BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfScoreLine(this);
            float n = 1, d = 1;
            for (int dim = 0; dim < baf.Formula.Length; dim++)
            {
                int sceding = baf.Formula[dim];

                if (sceding > 0)
                {
                    n *= MathF.Pow(DimensionInfo.Coefs[dim], sceding);
                }
                else if (sceding < 0)
                {
                    d *= MathF.Pow(DimensionInfo.Coefs[dim], -sceding);
                }
            }

            return n / d;
        }
    }
}
