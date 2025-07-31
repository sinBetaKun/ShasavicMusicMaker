using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal record Instrument
    {
        /// <summary>
        /// 楽器名
        /// </summary>
        public readonly string Name;
        
        /// <summary>
        /// グローバルゾーン
        /// </summary>
        public readonly InstrumentZone? GrobalZone;
        
        /// <summary>
        /// その他のゾーン
        /// </summary>
        public readonly InstrumentZone[] Zones;

        public Instrument(uint headerIndex, SF2RawData raw)
        {
            SF2InstHeader header = raw.Pdta.Inst.Headers[headerIndex];
            Name = header.Name;
            int index1 = header.BagIndex;
            int index2 = (headerIndex == raw.Pdta.Inst.Headers.Length - 1)
                ? raw.Pdta.Inst.Headers.Length
                : raw.Pdta.Inst.Headers[headerIndex + 1].BagIndex;
            List<InstrumentZone> zones = [];

            for (int i = index1; i < index2; i++)
            {
                int index3 = raw.Pdta.Ibag.Bags[i].GenIndex;
                if (i == raw.Pdta.Ibag.Bags.Length - 1)
                {
                    zones.Add(new(raw.Pdta.Igen.Gens[index3..], raw));
                }
                else
                {
                    int index4 = raw.Pdta.Ibag.Bags[i + 1].GenIndex;
                    zones.Add(new(raw.Pdta.Igen.Gens[index3..index4], raw));
                }
            }

            if (zones.Find(zone => zone.SmplHdr == null) is InstrumentZone zone)
            {
                GrobalZone = zone;
                zones.Remove(zone);
            }

            Zones = [.. zones];
        }
    }
}
