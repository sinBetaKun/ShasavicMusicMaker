using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    internal record SF2PdtaChunk
    {
        public readonly SF2PhdrChunk Phdr;
        public readonly SF2PbagChunk Pbag;
        public readonly SF2PmodChunk Pmod;
        public readonly SF2PgenChunk Pgen;
        public readonly SF2InstChunk Inst;
        public readonly SF2IbagChunk Ibag;
        public readonly SF2ImodChunk Imod;
        public readonly SF2IgenChunk Igen;
        public readonly SF2ShdrChunk Shdr;

        static string ID => "LIST";

        static string Type => "pdta";

        public SF2PdtaChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            reader.BaseStream.Seek(4, SeekOrigin.Current);

            string type = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (type != Type)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            Phdr = new SF2PhdrChunk(reader);
            Pbag = new SF2PbagChunk(reader);
            Pmod = new SF2PmodChunk(reader);
            Pgen = new SF2PgenChunk(reader);
            Inst = new SF2InstChunk(reader);
            Ibag = new SF2IbagChunk(reader);
            Imod = new SF2ImodChunk(reader);
            Igen = new SF2IgenChunk(reader);
            Shdr = new SF2ShdrChunk(reader);
        }
    }
}
