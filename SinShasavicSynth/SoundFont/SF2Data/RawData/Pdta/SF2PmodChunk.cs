using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// SoundFontファイル内のプリセットレベルにおけるすべてのモジュレータの情報が入ったチャンクです。
    /// <br/>
    /// モジュレータとは「MIDI CCで音量を操作する」「ピッチベンドで音程(ピッチ)を操作する」といった設定情報になります。
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#pmod%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2PmodChunk
    {
        readonly SF2Mod[] mods;

        static string ID => "pmod";

        public SF2PmodChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            uint size = reader.ReadUInt32();

            if (size % SF2Mod.Size != 0)
                throw new InvalidDataException($"Size of {ID} chunk is wrong.");

            mods = new SF2Mod[size / SF2Mod.Size];

            for (int i = 0; i < mods.Length; i++)
            {
                mods[i] = new(reader);
            }
        }
    }
}
