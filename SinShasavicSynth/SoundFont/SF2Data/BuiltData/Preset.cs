using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData.Pdta;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class Preset
    {
        /// <summary>
        /// プリセットの名前。終端文字(\0)含め20文字の文字列。
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// MIDIプリセット番号。
        /// </summary>
        public ushort Presetno { get; init; }

        /// <summary>
        /// MIDIバンク番号
        /// </summary>
        public ushort Bank { get; init; }

        /// <summary>
        /// グローバルゾーン
        /// </summary>
        public PresetZone? GrobalZone { get; init; }

        /// <summary>
        /// その他のゾーン
        /// </summary>
        public PresetZone[] LocalZones { get; init; }

        public Preset(uint headerIndex, SF2RawData raw)
        {
            PresetHeader header = raw.Pdta.Phdr.Headers[headerIndex];
            Name = header.Name;
            Presetno = header.PresetNo;
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

            LocalZones = [.. zones];
        }


        /// <summary>
        /// key,velの値を範囲に含むゾーンを全て取得する。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="vel">ベロシティ</param>
        /// <returns>該当する全てのゾーン</returns>
        public List<PresetZone> GetMatchedZones(byte key, byte vel)
        {
            List<PresetZone> matcheds1 = [];
            List<PresetZone> matcheds2 = [];
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

            foreach (PresetZone zone in LocalZones)
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

            foreach (PresetZone zone in matcheds1)
            {
                if (zone.Gens.TryGetValue(GeneratorType.VelRange, out ushort value))
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
