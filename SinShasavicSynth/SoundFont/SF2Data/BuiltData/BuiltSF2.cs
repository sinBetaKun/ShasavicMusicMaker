using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class BuiltSF2
    {
        readonly Preset[] presets;

        readonly float[] samples;

        public BuiltSF2(SF2RawData raw)
        {
            presets = new Preset[raw.Pdta.Phdr.Headers.Length];

            for (ushort i = 0; i < presets.Length; i++)
            {
                presets[i] = new(i, raw);
            }

            samples = new float[raw.Sdta.Smpl.Samples.Length];

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = raw.Sdta.Smpl.Samples[i];
            }
        }

        public List<InstrumentRegion> GetInstrumentRegions(ushort presetNo, ushort bank, byte key, byte vel)
        {
            Preset? preset = presets.FirstOrDefault(
                p => (p.Presetno == presetNo) && (p.Bank == bank));

            if (preset == null)
            {
                return [];
            }

            List<InstrumentRegion> regions = [];
            List<PresetZone> pzones = preset.GetMatchedZones(key, vel);

            foreach (PresetZone pzone in pzones)
            {
                if (pzone.Inst is Instrument inst)
                {
                    List<InstrumentZone> izones = inst.GetMatchedZones(key, vel);

                    foreach (InstrumentZone izone in izones)
                    {
                        regions.Add(new(izone, inst.GrobalZone, pzone, preset.GrobalZone));
                    }
                }
            }

            return regions;
        }
    }
}
