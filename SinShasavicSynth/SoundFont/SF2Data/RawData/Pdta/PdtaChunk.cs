using System.Text;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta
{
    internal class PdtaChunk
    {
        public PhdrChunk Phdr { get; init; }
        public PbagChunk Pbag { get; init; }
        public PmodChunk Pmod { get; init; }
        public PgenChunk Pgen { get; init; }
        public InstChunk Inst { get; init; }
        public IbagChunk Ibag { get; init; }
        public ImodChunk Imod { get; init; }
        public IgenChunk Igen { get; init; }
        public ShdrChunk Shdr { get; init; }

        static string ID => "LIST";

        static string Type => "pdta";

        public PdtaChunk(BinaryReader reader)
        {
            string id = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (id != ID)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            reader.BaseStream.Seek(4, SeekOrigin.Current);

            string type = Encoding.ASCII.GetString(reader.ReadBytes(4));

            if (type != Type)
                throw new InvalidDataException($"{Type} chunk isn't found.");

            Phdr = new PhdrChunk(reader);
            Pbag = new PbagChunk(reader);
            Pmod = new PmodChunk(reader);
            Pgen = new PgenChunk(reader);
            Inst = new InstChunk(reader);
            Ibag = new IbagChunk(reader);
            Imod = new ImodChunk(reader);
            Igen = new IgenChunk(reader);
            Shdr = new ShdrChunk(reader);
        }
    }
}
