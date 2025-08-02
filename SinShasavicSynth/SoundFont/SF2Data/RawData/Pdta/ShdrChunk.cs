using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    /// <summary>
    /// SoundFontファイル内のすべてのサンプル(音声波形)のヘッダ情報が入ったチャンクです。
    /// <br/>
    /// 実際の音声波形データはsmplチャンクの内容になります。
    /// <br/>
    /// 詳細:https://www.utsbox.com/?p=2090#shdr%E3%83%81%E3%83%A3%E3%83%B3%E3%82%AF
    /// </summary>
    internal class ShdrChunk
    {
        public SampleHeader[] Headers { get; init; }

        static string ID => "shdr";

        public ShdrChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            uint size = reader.ReadUInt32();

            if (size % SampleHeader.Size != 0)
                throw new InvalidDataException($"Size of {ID} chunk is wrong.");

            Headers = new SampleHeader[size / SampleHeader.Size];

            for (int i = 0; i < Headers.Length; i++)
            {
                Headers[i] = new(reader);
            }
        }
    }
}
