using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Sdta
{
    internal record SF2SdtaChunk
    {
        readonly SF2SmplChunk smpl;
        readonly SF2Sm24Chunk? sm24;

        static string ID => "LIST";
        static string Type => "sdta";

        public SF2SdtaChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            uint size = reader.ReadUInt32();

            string type = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (type != Type)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            smpl = new SF2SmplChunk(reader);

            if (size - 12 != smpl.Size)
            {
                sm24 = new(reader);
            }
        }
    }
}
