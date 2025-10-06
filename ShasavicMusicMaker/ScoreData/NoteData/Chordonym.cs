using ShasavicMusicMaker.Controller.Score;
using ShasavicMusicMaker.DimensionData;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    class Chordonym
    {
        public float OrgnBaseFreq { get; private set; }

        public Arm Arm { get; private set; }

        public ChordonymViewer? Viewer { get; set; }

        /// <summary>
        /// 底音の周波数を指定して新たにコードニムを作る
        /// </summary>
        /// <param name="freq">底音の周波数</param>
        public Chordonym(float freq)
        {
            OrgnBaseFreq = freq;
            Arm = new Arm();
        }

        /// <summary>
        /// 自身を複製する。
        /// コードニムの形が変わったときに、コマンドに変化前と変化後の状態を記録するために使う。
        /// </summary>
        /// <returns>複製</returns>
        public Chordonym Clone()
        {
            Chordonym clone = new(OrgnBaseFreq)
            {
                Arm = Arm.Clone()
            };

            return clone;
        }

        /// 各次元線を和音図に起こしたときに、下の端がどこに来るかを次元別に求めている。
        /// </summary>
        /// <returns></returns>
        public List<(Arm, BaseAndFormula)>[] CalcDimensionLineVPositions()
        {
            List<(Arm, BaseAndFormula)>[] ret = new List<(Arm, BaseAndFormula)>[DimensionInfo.MaxDimension];
            for (int i = 0; i < DimensionInfo.MaxDimension; i++)
                ret[i] = [];

            List<(Arm, BaseAndFormula)> aacs1 = [(Arm, BaseAndFormula.CalcBaseAndFomulaOfArm(Arm))];
            List<(Arm, BaseAndFormula)> aacs2 = [];

            while (aacs1.Count > 0)
            {
                foreach ((Arm arm1, BaseAndFormula baf1) in aacs1)
                {
                    foreach (Arm arm2 in arm1.Arms)
                    {
                        BaseAndFormula baf2 = BaseAndFormula.CalcBaseAndFomulaOfArm(arm2);
                        ret[arm2.Bcp.Dimension - 1].Add((arm2, arm2.Bcp.Scending ? baf1 : baf2));
                        aacs2.Add((arm2, baf2));
                    }
                }

                aacs1 = aacs2;
                aacs2 = [];
            }

            foreach (List<(Arm, BaseAndFormula)> coefs in ret)
            {
                coefs.Sort((a, b) => a.Item2.CalcCoef().CompareTo(b.Item2.CalcCoef()));
            }

            return ret;
        }

        /// <summary>
        /// 腕を交換するメソッド。
        /// コマンド意外から呼び出すことは推奨しない。
        /// </summary>
        /// <param name="arm">変更後の腕</param>
        public void ChangeArm(Arm arm)
        {
            Arm = arm;
            
            if (Viewer is not null)
            {
                Viewer.UpdateChordonym();
                Viewer.UpdateScoreLines();
            }
        }
    }
}
