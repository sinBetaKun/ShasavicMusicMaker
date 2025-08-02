using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// SoundFontファイル内のすべてのインストルメント(楽器)のヘッダ情報が入ったチャンクです。
    /// <br/>
    /// サンプル(音声波形)のマッピング情報やパラメータ、割り当てサンプル(音声波形)等の情報は
    /// ibagチャンク、imodチャンク、igenチャンクに含まれています。
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#inst%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal class InstChunk
    {
        public InstHeader[] Headers { get; init; }

        static string ID => "inst";

        public InstChunk(BinaryReader reader)
        {

            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            uint size = reader.ReadUInt32();

            if (size % InstHeader.Size != 0)
                throw new InvalidDataException($"Size of {ID} chunk is wrong.");

            Headers = new InstHeader[size / InstHeader.Size];

            for (int i = 0; i < Headers.Length; i++)
            {
                Headers[i] = new(reader);
            }
        }
    }
}
