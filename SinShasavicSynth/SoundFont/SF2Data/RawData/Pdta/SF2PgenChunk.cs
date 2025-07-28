using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// SoundFontファイル内のプリセットレベルにおけるすべてのジェネレータの情報が入ったチャンクです。
    /// <br/>
    /// ジェネレータとはマッピングや音量やフィルタのエンベロープ、
    /// 割り当てインストルメント(楽器)等の設定情報になります。
    /// <br/>
    /// <br/>
    /// ゾーン内には変更のあるジェネレータのみが保存されている。
    /// <br/>
    /// 以下のジェネレータは記述通りの順序で並ぶ。
    /// <br/>
    /// 1. keyRangeジェネレータ(genOper=43)（ない場合もある。）
    /// <br/>
    /// 2. velRangeジェネレータ(genOper=44)（ない場合もある。）
    /// <br/>
    /// 3. その他のジェネレーターたち。
    /// <br/>
    /// 4. instrumentジェネレータ(genOper=41)（ない場合、このゾーンはグローバルゾーンになる）
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#pgen%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// https://www.utsbox.com/?p=2390
    /// </summary>
    internal record SF2PgenChunk
    {
        readonly SF2Gen[] gens;

        static string ID => "pgen";

        public SF2PgenChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            uint size = reader.ReadUInt32();

            if (size % SF2Gen.Size != 0)
                throw new InvalidDataException($"Size of {ID} chunk is wrong.");

            gens = new SF2Gen[size / SF2Gen.Size];

            for (int i = 0; i < gens.Length; i++)
            {
                gens[i] = new(reader);
            }
        }
    }
}
