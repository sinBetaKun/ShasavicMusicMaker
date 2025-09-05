using ShasavicMusicMaker.DimensionData;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    class Chordonym
    {
        float OrgnBaseFreq;

        public Arm Arm { get; private set; }

        public Chordonym(float freq)
        {
            OrgnBaseFreq = freq;
            Arm = new Arm();
        }

        /// <summary>
        /// 各次元線を和音図に起こしたときに、下の端がどこに来るかを次元別に求めている。
        /// </summary>
        /// <returns></returns>
        public List<(Arm, Fraction)>[] CalcDimensionLineVPositions()
        {
            List<(Arm, Fraction)>[] ret = new List<(Arm, Fraction)>[DimensionInfo.MaxDimension];
            for (int i = 0; i < DimensionInfo.MaxDimension; i++)
                ret[i] = [];

            List<(Arm, Fraction)> aacs1 = [(Arm, Arm.CalcCoefFromBase())];
            List<(Arm, Fraction)> aacs2 = [];

            while (aacs1.Count > 0)
            {
                foreach ((Arm arm1, Fraction coef1) in aacs1)
                {
                    foreach (Arm arm2 in arm1.Arms)
                    {
                        Fraction coef2 = arm2.CalcCoefFromBase();
                        ret[arm2.Bcp.Dimension - 1].Add((arm2, arm2.Bcp.Scending ? coef1 : coef2));
                        aacs2.Add((arm2, coef2));
                    }
                }

                aacs1 = aacs2;
                aacs2 = [];
            }

            foreach (List<(Arm, Fraction)> coefs in ret)
            {
                coefs.Sort((a, b) => a.Item2.ToFloat().CompareTo(b.Item2.ToFloat()));
            }

            return ret;
        }
    }
}
