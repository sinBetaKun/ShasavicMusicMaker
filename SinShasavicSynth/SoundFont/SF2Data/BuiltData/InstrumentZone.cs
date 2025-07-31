using System;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal record InstrumentZone
    {
        public readonly Dictionary<GeneratorType, ushort> Gens;
        public readonly SF2SampleHeader? SmplHdr;

        public InstrumentZone(IEnumerable<SF2Gen> gens, SF2RawData raw)
        {
            Gens = [];

            foreach (SF2Gen gen in gens)
            {
                if (gen.Oper < 256)
                {
                    if (Enum.IsDefined(typeof(GeneratorType), (byte)gen.Oper))
                    {
                        Gens.Add((GeneratorType)gen.Oper, gen.Amount);
                    }
                }
            }

            if (Gens.TryGetValue(GeneratorType.SampleID, out ushort sampleID))
            {
                SmplHdr = raw.Pdta.Shdr.Headers[sampleID];
            }
        }
    }
}
