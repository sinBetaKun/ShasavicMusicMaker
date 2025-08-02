using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class InstrumentRegion
    {
        public Dictionary<GeneratorType, ushort> Gens { get; init; }
        public SampleHeader_b[] SmplHdrs { get; init; }

        public InstrumentRegion(InstrumentZone iLZone, InstrumentZone? iGZone, PresetZone pLZone, PresetZone? pGZone)
        {
            SmplHdrs = iLZone.SmplHdrs;

            Gens = new(iLZone.Gens);

            if (iGZone is not null)
            {
                foreach (var kvp in iGZone.Gens)
                {
                    Gens.TryAdd(kvp.Key, kvp.Value);
                }
            }

            foreach (var kvp in pLZone.Gens)
            {
                Gens.TryAdd(kvp.Key, kvp.Value);
            }

            if (pGZone is not null)
            {
                foreach (var kvp in pGZone.Gens)
                {
                    Gens.TryAdd(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
