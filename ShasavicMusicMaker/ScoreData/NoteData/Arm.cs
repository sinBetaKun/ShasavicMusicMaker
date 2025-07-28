using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ShasavicMusicMaker.ScoreData.NoteData
{
    /// <summary>
    /// 腕を表現するためのクラス。
    /// 
    /// </summary>
    internal class Arm
    {
        /// <summary>
        /// 体。底音である場合、ここにはnullが入る。
        /// </summary>
        public Arm? Body { get; private set; }

        /// <summary>
        /// 体始音高 (body-centered pitch)。
        /// この腕が体から何次元に、上下のどちらに伸びているのかを示す。
        /// </summary>
        public BodyCntdPitch Bcp { get; private set; } 

        /// <summary>
        /// この腕を体とする腕の集合。
        /// </summary>
        public List<Arm> Arms { get; private set; }

        /// <summary>
        /// 初期状態では腕を持たない底音を表す。
        /// </summary>
        public Arm()
        {
            Bcp = new(0, true);
            Arms = [];
        }

        /// <summary>
        /// 既存の腕に別の腕から先をコピーできる。
        /// 既存の別の腕と、底音基準の組成式が重複する可能性がある。
        /// </summary>
        /// <param name="origin">コピー元の腕</param>
        /// <param name="parent">体にしたい腕</param>
        private Arm(Arm origin, Arm parent)
        {
            Bcp = new(origin.Bcp);
            Arms = new List<Arm>(origin.Arms.Count);

            foreach (Arm children in origin.Arms)
            {
                Arm arm = new(children, this);
                Arms.Add(arm);
            }
        }

        /// <summary>
        /// 体始音高を変更する。
        /// この腕が底音であるときは無条件でfalseを返し、変更を行わない。
        /// 
        /// secure が true の場合、
        /// 変更後、自身や子要素の腕と組成式が同じ腕が重複する場合、falseを返して変更はしない。
        /// 重複しない場合はtrueを返す。
        /// このソフトは、１つのコードニムに
        /// 同じ組成式を持つ腕を２つ以上持たせることができない。
        /// 
        /// secure が false の場合は、重複が発生した際にマージを行い、
        /// 自身が重複している場合はfalseを返す。
        /// していない場合はtrueを返す。
        /// </summary>
        /// <param name="secure"></param>
        /// <param name="bcp1">変更後の体始音高</param>
        /// <returns>変更したならtrue</returns>
        public bool ChangeBodyCntdPitch_s(BodyCntdPitch bcp1, bool secure)
        {
            if (Body is null)
            {
                return false;
            }

            BaseAnsFormula baf = BaseAnsFormula.CalcBafOfArm(this);
            List<ImmutableArray<int>> newFormulas = [];
            ImmutableArray<Arm> oldArms1 = [this];
            List<Arm> oldArms2 = [];
            ImmutableArray<ImmutableArray<int>> oldFormulas1 = [baf.Formula];
            List<ImmutableArray<int>> oldFormulas2 = [];
            List<Arm> sortedArms1 = new(Arms.Count); // secure = true の時のみ使用。
            List<Arm> sortedArms2 = new(Arms.Count); // secure = true の時のみ使用。
            while (oldArms1.Length > 0)
            {
                for (int i = 0; i < oldFormulas1.Length; i++)
                {
                    Arm oldArm1 = oldArms1[i];
                    ImmutableArray<int> oldFormula1 = oldFormulas1[i];
                    int newScedingA = oldFormula1[bcp1.Dimension] + (bcp1.Scending ? 1 : -1);
                    int newScedingB = oldFormula1[Bcp.Dimension] + (Bcp.Scending ? -1 : 1);
                    ImmutableArray<int> newFormula = baf.Formula
                        .SetItem(bcp1.Dimension, newScedingA)
                        .SetItem(Bcp.Dimension, newScedingB);
                    newFormulas.Add(newFormula);
                    sortedArms1.Add(oldArm1);
                    sortedArms2.Add(oldArm1);

                    foreach (Arm oldArm2 in oldArm1.Arms)
                    {
                        oldArms2.Add(oldArm2);
                        int oldSceding = oldFormula1[oldArm2.Bcp.Dimension] + (oldArm2.Bcp.Scending ? 1 : -1);
                        ImmutableArray<int> oldFormula2 = oldFormula1.SetItem(oldArm2.Bcp.Dimension, oldSceding);
                        oldFormulas2.Add(oldFormula2);
                    }
                }

                oldArms1 = [.. oldArms2];
                oldArms2.Clear();
                oldFormulas1 = [.. oldFormulas2];
                oldFormulas2.Clear();
            }

            // 一度Bodyからこの腕にアクセスできなくする。
            Body.Arms.Remove(this);

            ImmutableArray<Arm> arms1 = [baf.Base];
            List<Arm> arms2 = [];
            ImmutableArray<ImmutableArray<int>> formulas1 = [[.. Enumerable.Repeat(0, BaseAnsFormula.MaxDimension)]];
            List<ImmutableArray<int>> formulas2 = [];

            while (arms1.Length != 0)
            {
                for (int i = 0; i < arms1.Length; i++)
                {
                    Arm arm1 = arms1[i];
                    ImmutableArray<int> formula1 = formulas1[i];

                    foreach (Arm arm2 in arm1.Arms)
                    {
                        BodyCntdPitch bcp2 = arm2.Bcp;
                        int newSceding = formula1[bcp2.Dimension] + (bcp2.Scending ? 1 : -1);
                        ImmutableArray<int> formula2 = formula1.SetItem(bcp2.Dimension, newSceding);

                        foreach (ImmutableArray<int> newFormula in newFormulas)
                        {
                            if (formula2.SequenceEqual(newFormula))
                            {
                                if (secure)
                                {
                                    // Bodyからこの腕にアクセスできるようにする。
                                    Body.Arms.Add(this);
                                    return false;
                                }
                                else
                                {
                                    int index = newFormulas.IndexOf(newFormula);
                                    sortedArms2[index] = arm2;
                                }
                            }
                        }

                        arms2.Add(arm2);
                        formulas2.Add(formula2);
                    }
                }

                arms1 = [.. arms2];
                arms2.Clear();
                formulas1 = [.. formulas2];
                formulas2.Clear();
            }

            if (!secure)
            {
                List<Arm> bodyChangeds1 = [];

                // Body の置き換え
                for (int i = 0; i < sortedArms1.Count; i++)
                {
                    Arm arm1 = sortedArms1[i];
                    Arm arm2 = sortedArms2[i];

                    if (arm1 != arm2)
                    {
                        bodyChangeds1.Remove(arm1);

                        List<Arm> bodyChangeds2 = sortedArms1.FindAll(arm => arm.Body == arm1);
                        bodyChangeds2.ForEach(arm =>
                        {
                            arm.Body = arm2;
                            bodyChangeds1.Add(arm);
                        });

                        arm1.Body = null;
                        arm1.Arms.Clear();
                    }
                }

                // 各BodyのArmsに追加
                bodyChangeds1.ForEach(arm => arm.Body.Arms.Add(arm));

                if (sortedArms2[0] != this)
                {
                    return false;
                }
            }

            Bcp = bcp1;

            // Bodyからこの腕にアクセスできるようにする。
            Body.Arms.Add(this);
            return true;
        }
    }
}
