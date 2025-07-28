using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    internal record SF2PdtaChunk
    {
        readonly SF2PhdrChunk phdr;
        readonly SF2PbagChunk pbag;
        readonly SF2PmodChunk pmod;
        readonly SF2PgenChunk pgen;
        readonly SF2InstChunk inst;
        readonly SF2IbagChunk ibag;
        readonly SF2ImodChunk imod;
        readonly SF2IgenChunk igen;
        readonly SF2ShdrChunk shdr;

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

            phdr = new SF2PhdrChunk(reader);
            pbag = new SF2PbagChunk(reader);
            pmod = new SF2PmodChunk(reader);
            pgen = new SF2PgenChunk(reader);
            inst = new SF2InstChunk(reader);
            ibag = new SF2IbagChunk(reader);
            imod = new SF2ImodChunk(reader);
            igen = new SF2IgenChunk(reader);
            shdr = new SF2ShdrChunk(reader);
        }
    }
}
