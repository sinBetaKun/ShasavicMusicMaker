using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using ShasavicMusicMaker.DimensionData;

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

        public bool Muted { get; set; } = false;

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
        /// <param name="body">体にしたい腕</param>
        private Arm(Arm origin, Arm body)
        {
            Body = body;
            Bcp = new(origin.Bcp);
            Arms = new List<Arm>(origin.Arms.Count);

            foreach (Arm children in origin.Arms)
            {
                Arm arm = new(children, this);
                Arms.Add(arm);
            }
        }

        /// <summary>
        /// 自身を複製する。
        /// 腕の末端まで新たなArmとして複製する。
        /// 底音以外のArmでこのメソッドは使わないように。
        /// </summary>
        /// <returns>複製</returns>
        public Arm Clone()
        {
            Arm clone = new()
            {
                Bcp = new(Bcp)
            };

            foreach (Arm arm in Arms)
            {
                clone.Arms.Add(arm.Clone(body: clone));
            }

            return clone;
        }

        /// <summary>
        /// 自身を複製する。
        /// 腕の末端まで新たなArmとして複製する。
        /// 底音以外の腕でこのメソッドを使うように。
        /// </summary>
        /// <param name="body">親</param>
        /// <returns>複製</returns>
        private Arm Clone(Arm body)
        {
            Arm clone = new()
            {
                Body = body,
                Bcp = new(Bcp)
            };

            foreach (Arm arm in Arms)
            {
                clone.Arms.Add(arm.Clone(body: clone));
            }

            return clone;
        }

        /// <summary>
        /// 新たに腕を追加する。
        /// 組成式が別の腕と一致する場合、追加せずに(false, 重複先)を返す。
        /// 重複が無ければ(true, 新しく追加した腕)を返す。
        /// </summary>
        /// <param name="pitch"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (bool, Arm) AddArm(BodyCntdPitch pitch)
        {
            if (pitch.Dimension < 1)
                throw new Exception("can't add invalid dimension arm.");

            // 底音と組成式を取得
            BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(this);

            baf.Formula[pitch.Dimension - 1] += pitch.Scending ? 1 : -1;

            // コードニム上の全ての腕とその組成式を取得
            Dictionary<BaseAndFormula, Arm> allArms_base = BaseAndFormula.GetArmsDictOfChordonym(baf.Base);

            foreach (BaseAndFormula baf2 in allArms_base.Keys)
            {
                // 腕の組成式が一致した場合
                if (baf2.Formula.SequenceEqual(baf.Formula))
                {
                    // 組成式の一致する腕を返す。
                    return (false, allArms_base[baf2]);
                }
            }

            Arm ret = new()
            {
                Body = this,
                Bcp = pitch,
            };

            Arms.Add(ret);

            return (true, ret);
        }

        /// <summary>
        /// 体始音高を変更する。
        /// この腕が底音であるときはエラーを吐く。
        /// <br/>
        /// secure が true の場合、
        /// 変更後、自身や子要素の腕と組成式が同じ腕が重複する場合、変更はしない。
        /// <br/>
        /// secure が false の場合は、重複が発生した際にマージを行う。
        /// </summary>
        /// <param name="pitch">変更後の体始音高</param>
        /// <param name="secure">セキュアモード</param>
        /// <returns>重複する腕と重複先の腕の辞書</returns>
        public Dictionary<Arm, Arm> ChangeBodyCntdPitch_s(BodyCntdPitch pitch, bool secure)
        {
            if (Body is null)
                throw new Exception("can't use body for this method");

            // 底音と組成式を取得
            BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(this);

            // 一度Bodyからこの腕にアクセスできなくする。
            Body.Arms.Remove(this);

            // この腕を除いた状態における、コードニム上の全ての腕とその組成式を取得
            Dictionary<BaseAndFormula, Arm> allArms_base = BaseAndFormula.GetArmsDictOfChordonym(baf.Base);

            // この腕上の子要素の全ての腕とその組成式を取得（自身も含む）
            Dictionary<BaseAndFormula, Arm> allArms_this = BaseAndFormula.GetArmsDictOfChordonym(this);

            // 体始音高を変更すると重複先が発声する腕をキーとし、重複先の腕をバリューとする辞書
            Dictionary<Arm, Arm> warningArms = [];

            // 組成式の一致する腕を辞書に登録
            foreach (var item in allArms_this)
            {
                BaseAndFormula ibaf = item.Key;
                Arm iarm = item.Value;

                int[] newFormula = [.. ibaf.Formula];
                newFormula[pitch.Dimension] += pitch.Scending ? 1 : -1;

                foreach (BaseAndFormula ibaf2 in allArms_base.Keys)
                {
                    if (ibaf2.Formula.SequenceEqual(newFormula))
                        // 組成式の一致する腕を辞書に登録
                        warningArms.Add(iarm, allArms_base[ibaf2]);
                }
            }

            // Bodyからこの腕に再びアクセスできるようにする。
            Body.Arms.Add(this);

            if (secure)
            {
                if (warningArms.Count == 0)
                    // 重複がなければ体始音高を変更
                    Bcp = pitch;
            }
            else
            {
                // 重複する腕を体から取り除く
                foreach (Arm iarm in warningArms.Keys)
                {
                    if (iarm.Body is null)
                        throw new Exception("invalid arm was found.");

                    iarm.Body.Arms.Remove(iarm);
                }

                // 重複先に、重複元の腕を移動
                foreach (var item in warningArms)
                {
                    item.Value.Arms.AddRange(item.Key.Arms);
                }

                // 体始音高を変更
                Bcp = pitch;
            }

            return warningArms;
        }

        /// <summary>
        /// 底音との周波数比を取得するためのメソッド。
        /// </summary>
        /// <returns>底音との周波数比</returns>
        public float CalcCoefFromBase()
        {
            BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(this);
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

        /// <summary>
        /// 体との接続を切る。もともと切ってある場合はfalseを返す。
        /// </summary>
        /// <returns>体との接続を切ったならtrue。すでに切ってあるならfalse</returns>
        public bool DisconnectBody()
        {
            if (Body is null) return false;

            Body.Arms.Remove(this);
            Body = null;
            return true;
        }
    }
}
