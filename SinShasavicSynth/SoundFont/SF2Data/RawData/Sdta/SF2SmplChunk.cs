using System.Collections.Immutable;
using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta
{
    internal record SF2SmplChunk
    {
        readonly short[] samples;

        public readonly uint Size;

        static string ID => "smpl";

        public SF2SmplChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{ID} chunk isn't found.");

            Size = reader.ReadUInt32();
            uint sampleCnt = Size / 2;
            samples = new short[sampleCnt];

            for (uint i = 0; i < sampleCnt; i++)
            {
                samples[i] = reader.ReadInt16();
            }
        }
    }
}
