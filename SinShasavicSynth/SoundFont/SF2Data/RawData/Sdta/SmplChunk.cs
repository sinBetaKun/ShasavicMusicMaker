using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta
{
    internal class SmplChunk
    {
        /// <summary>
        /// サンプル波形データの開始位置
        /// </summary>
        public long SamplePos { get; init; }

        public uint Size { get; init; }

        static string ID => "smpl";

        public SmplChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            Size = reader.ReadUInt32();
            SamplePos = reader.BaseStream.Position;
            reader.BaseStream.Seek(Size, SeekOrigin.Current);
        }
    }
}
