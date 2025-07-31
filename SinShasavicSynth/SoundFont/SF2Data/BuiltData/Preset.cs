using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal record Preset
    {
        /// <summary>
        /// プリセットの名前。終端文字(\0)含め20文字の文字列。
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// MIDIプリセット番号。
        /// </summary>
        public readonly ushort Presetno;

        /// <summary>
        /// MIDIバンク番号
        /// </summary>
        public readonly ushort Bank;

        /// <summary>
        /// グローバルゾーン
        /// </summary>
        public readonly PresetZone? GrobalZone;

        /// <summary>
        /// その他のゾーン
        /// </summary>
        public readonly PresetZone[] Zones;

        public Preset(uint headerIndex, SF2RawData raw)
        {
            SF2PresetHeader header = raw.Pdta.Phdr.Headers[headerIndex];
            Name = header.Name;
            Presetno = header.Presetno;
            Bank = header.Bank;
            int index1 = header.BagIndex;
            int index2 = (headerIndex == raw.Pdta.Phdr.Headers.Length - 1)
                ? raw.Pdta.Phdr.Headers.Length
                : raw.Pdta.Phdr.Headers[headerIndex + 1].BagIndex;
            List<PresetZone> zones = [];

            for (int i = index1; i < index2; i++)
            {
                int index3 = raw.Pdta.Pbag.Bags[i].GenIndex;
                if (i == raw.Pdta.Pbag.Bags.Length - 1)
                {
                    zones.Add(new(raw.Pdta.Pgen.Gens[index3..], raw));
                }
                else
                {
                    int index4 = raw.Pdta.Pbag.Bags[i + 1].GenIndex;
                    zones.Add(new(raw.Pdta.Pgen.Gens[index3..index4], raw));
                }
            }

            if (zones.Find(zone => zone.Inst == null) is PresetZone zone)
            {
                GrobalZone = zone;
                zones.Remove(zone);
            }

            Zones = [.. zones];
        }

    }
}
