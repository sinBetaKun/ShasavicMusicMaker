using System.Collections.Immutable;
using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta
{
    internal class SmplChunk
    {
        public short[] Samples { get; init; }

        public readonly uint Size;

        static string ID => "smpl";

        public SmplChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            Size = reader.ReadUInt32();
            uint sampleCnt = Size / 2;
            Samples = new short[sampleCnt];

            for (uint i = 0; i < sampleCnt; i++)
            {
                Samples[i] = reader.ReadInt16();
            }
        }
    }
}
