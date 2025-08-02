using System;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class InstrumentZone
    {
        public Dictionary<GeneratorType, ushort> Gens { get; init; }
        public SampleHeader_b[] SmplHdrs { get; init; }

        public InstrumentZone(IEnumerable<Gen> gens, SF2RawData raw)
        {
            Gens = [];

            foreach (Gen gen in gens)
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
                SampleHeader rawHeader1 = raw.Pdta.Shdr.Headers[sampleID];

                switch (rawHeader1.Type)
                {
                    case 1:
                        SmplHdrs = [new(rawHeader1)];
                        break;
                    case 2:
                        SampleHeader rawHeader2 = raw.Pdta.Shdr.Headers[rawHeader1.SampleLink];
                        SmplHdrs = [new(rawHeader2), new(rawHeader1)];
                        break;
                    case 4:
                        SampleHeader rawHeader3 = raw.Pdta.Shdr.Headers[rawHeader1.SampleLink];
                        SmplHdrs = [new(rawHeader1), new(rawHeader3)];
                        break;
                    default:
                        SmplHdrs = [new(rawHeader1)];
                        break;
                }
            }
            else
            {
                SmplHdrs = [];
            }
        }
    }
}
