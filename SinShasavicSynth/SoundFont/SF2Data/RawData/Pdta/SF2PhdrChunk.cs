using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// SoundFontファイル内のすべてのプリセットのヘッダ情報が入ったチャンクです。
    /// <br/>
    /// インストルメント(楽器)のマッピング情報やパラメータ、割り当てインストルメント(楽器)等の情報は
    /// pbagチャンク、pmodチャンク、pgenチャンクに含まれています。
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#phdr%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal record SF2PhdrChunk
    {
        public readonly SF2PresetHeader[] Headers;

        static string ID => "phdr";

        public SF2PhdrChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            uint size = reader.ReadUInt32();

            if (size % SF2PresetHeader.Size != 0 || size / SF2PresetHeader.Size < 2)
                throw new InvalidDataException($"Size of {ID} chunk is wrong.");

            Headers = new SF2PresetHeader[size / SF2PresetHeader.Size];

            for (int i = 0; i < Headers.Length; i++)
            {
                Headers[i] = new(reader);
            }
        }
    }
}
