using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta
{
    internal class SdtaChunk
    {
        public SmplChunk Smpl { get; init; }
        private readonly Sm24Chunk? _sm24;

        static string ID => "LIST";
        static string Type => "sdta";

        public SdtaChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            uint size = reader.ReadUInt32();

            string type = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (type != Type)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            Smpl = new SmplChunk(reader);

            if (size - 12 != Smpl.Size)
            {
                _sm24 = new Sm24Chunk(reader);
            }
        }
    }
}
