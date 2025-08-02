using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using NAudio.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class Instrument
    {
        /// <summary>
        /// 楽器名
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// グローバルゾーン
        /// </summary>
        public InstrumentZone? GrobalZone { get; init; }

        /// <summary>
        /// その他のゾーン
        /// </summary>
        public InstrumentZone[] LocalZones { get; init; }

        public Instrument(uint headerIndex, SF2RawData raw)
        {
            InstHeader header = raw.Pdta.Inst.Headers[headerIndex];
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

            if (zones.Find(zone => zone.SmplHdrs.Length == 0) is InstrumentZone zone)
            {
                GrobalZone = zone;
                zones.Remove(zone);
            }

            LocalZones = [.. zones];
        }

        /// <summary>
        /// key,velの値を範囲に含むゾーンを全て取得する。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="vel">ベロシティ</param>
        /// <returns>該当する全てのゾーン</returns>
        public List<InstrumentZone> GetMatchedZones(byte key, byte vel)
        {
            List<InstrumentZone> matcheds1 = [];
            List<InstrumentZone> matcheds2 = [];
            bool includeGKey = true;
            bool includeGVel = true;

            if (GrobalZone is not null)
            {
                if (GrobalZone.Gens.TryGetValue(GeneratorType.KeyRange, out ushort gValueK))
                {
                    byte maxKeyG = (byte)(gValueK >> 8);
                    byte minKeyG = (byte)(gValueK & 0xFF);
                    includeGKey = (minKeyG <= key) && (key <= maxKeyG);
                }

                if (GrobalZone.Gens.TryGetValue(GeneratorType.VelRange, out ushort gValueV))
                {
                    byte maxVelG = (byte)(gValueV >> 8);
                    byte minVelG = (byte)(gValueV & 0xFF);
                    includeGVel = (minVelG <= vel) && (vel <= maxVelG);
                }
            }

            foreach (InstrumentZone zone in LocalZones)
            {
                if (zone.Gens.TryGetValue(GeneratorType.KeyRange, out ushort value))
                {
                    byte maxKey = (byte)(value >> 8);
                    byte minKey = (byte)(value & 0xFF);

                    if ((minKey <= key) && (key <= maxKey))
                    {
                        matcheds1.Add(zone);
                    }
                }
                else if (includeGKey)
                {
                    matcheds1.Add(zone);
                }
            }

            foreach (InstrumentZone zone in matcheds1)
            {
                if (zone.Gens.TryGetValue(GeneratorType.KeyRange, out ushort value))
                {
                    byte maxVel = (byte)(value >> 8);
                    byte minVel = (byte)(value & 0xFF);

                    if ((minVel <= vel) && (vel <= maxVel))
                    {
                        matcheds2.Add(zone);
                    }
                }
                else if (includeGVel)
                {
                    matcheds2.Add(zone);
                }
            }

            return matcheds2;
        }
    }
}
