using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// SoundFontファイル内のインストルメントレベルにおけるすべてのバッグ情報が入ったチャンクです。
    /// <br/>
    /// バッグはモジュレータとジェネレータをひとまとめにするためのもので、
    /// バッグ・モジュレータ・ジェネレータのかたまりをゾーンと言います。
    /// <br/>
    /// 具体的に「どのパラメータをどうするか？」の情報はigenチャンクとimodチャンクに含まれています。
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#ibag%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2IbagChunk
    {
        readonly SF2Bag[] bags;

        static string ID => "ibag";

        public SF2IbagChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            uint size = reader.ReadUInt32();

            if (size % SF2Bag.Size != 0)
                throw new InvalidDataException($"Size of {ID} chunk is wrong.");

            bags = new SF2Bag[size / SF2Bag.Size];

            for (int i = 0; i < bags.Length; i++)
            {
                bags[i] = new(reader);
            }
        }
    }
}
