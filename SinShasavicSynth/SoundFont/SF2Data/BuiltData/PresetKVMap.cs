using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class PresetKVMap
    {
        /// <summary>
        /// MIDIプリセット番号。
        /// </summary>
        public ushort PresetNo { get; init; }

        /// <summary>
        /// MIDIバンク番号
        /// </summary>
        public ushort Bank { get; init; }

        private readonly (byte, (byte, InstrumentRegion[])[])[] map;

        public PresetKVMap(Preset preset)
        {
            PresetNo = preset.Presetno;
            Bank = preset.Bank;


            List<(byte, (byte, InstrumentRegion[])[])> regions = [];

            for (byte key = 0; key < 128; key++)
            {
                List<(byte, InstrumentRegion[])> keyMatched = [];

                for (byte vel = 1; vel < 128; vel++)
                {
                    List<InstrumentRegion> velMatched = [];
                    List<PresetZone> pzones = preset.GetMatchedZones(key, vel);

                    foreach (PresetZone pzone in pzones)
                    {
                        if (pzone.Inst is Instrument inst)
                        {
                            List<InstrumentZone> izones = inst.GetMatchedZones(key, vel);

                            foreach (InstrumentZone izone in izones)
                            {
                                velMatched.Add(new(izone, inst.GrobalZone, pzone, preset.GrobalZone, key, vel));
                            }
                        }
                    }

                    if (velMatched.Count > 0)
                    {
                        keyMatched.Add((vel, [.. velMatched]));
                    }
                }

                if (keyMatched.Count > 0)
                {
                    keyMatched.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
                    regions.Add((key, [.. keyMatched]));
                }
            }

            regions.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));
            map = [.. regions];
        }

        public KeyVelCompleter GetKVCompleter(byte key, byte vel)
        {
            if (map.FirstOrDefault(kt => kt.Item1 == key).Item2 is (byte, InstrumentRegion[])[] vts)
            {
                if (vts.FirstOrDefault(vt => vt.Item1 == vel).Item2 is InstrumentRegion[] regions1)
                {
                    return new(regions1, 1.0f, 1.0f);
                }
                else
                {
                    (byte, InstrumentRegion[]) vt = vts.First();

                    for (byte i = 1; i < vts.Length; i++)
                    {
                        if (vel < vts[i].Item1)
                        {
                            if (vts[i].Item1 - vel < vel - vt.Item1)
                                return new(vts[i].Item2, 0, 1f * vts[i].Item1 / vel);
                            else
                                break;
                        }
                        else
                            vt = vts[i];
                    }

                    return new(vt.Item2, 1.0f, 1f * vel / vt.Item1);
                }
            }
            else
            {
                (byte, (byte, InstrumentRegion[])[]) pt = map.First();

                for (byte i = 1; i < map.Length; i++)
                {
                    if (key < map[i].Item1)
                    {
                        if (map[i].Item1 - key < key - pt.Item1)
                            pt = map[i];

                        break;
                    }
                    else
                        pt = map[i];
                }

                float pitch = MathF.Pow(2, (key - pt.Item1) / 12.0f);

                if (pt.Item2.FirstOrDefault(vt => vt.Item1 == vel).Item2 is InstrumentRegion[] regions2)
                {
                    return new(regions2, pitch, 1.0f);
                }
                else
                {
                    (byte, InstrumentRegion[]) vt = pt.Item2.First();

                    for (byte i = 1; i < pt.Item2.Length; i++)
                    {
                        if (vel < pt.Item2[i].Item1)
                        {
                            if (pt.Item2[i].Item1 - vel < vel - vt.Item1)
                                return new(pt.Item2[i].Item2, 0, 1f * pt.Item2[i].Item1 / vel);
                            else
                                break;
                        }
                        else
                            vt = pt.Item2[i];
                    }

                    return new(vt.Item2, pitch, 1f * vel / vt.Item1);
                }
            }
        }
    }
}
