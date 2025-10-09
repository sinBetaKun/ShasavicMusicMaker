using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SynthEngineCore;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class BuiltSF2
    {
        private Preset[] presets { get; init; }

        public float[] Samples { get; init; }

        public int SampleRate { get; init; }

        public BuiltSF2(SF2RawData raw)
        {
            WdlResamplingSampleProvider a;
            presets = new Preset[raw.Pdta.Phdr.Headers.Length];

            for (ushort i = 0; i < presets.Length; i++)
            {
                presets[i] = new(i, raw);
            }

            Samples = new float[raw.Sdta.Smpl.Samples.Length];

            for (int i = 0; i < Samples.Length; i++)
            {
                Samples[i] = raw.Sdta.Smpl.Samples[i];
            }

            SampleRate = (int)raw.Pdta.Shdr.Headers[0].SampleRate;
        }

        public List<VoiceBase> GetVoices(ushort presetNo, ushort bank, byte key, byte vel, float pitch = 1.0f)
        {
            List<VoiceBase> voices = [];
            List<InstrumentRegion> regions = GetInstrumentRegions(presetNo, bank, key, vel);

            foreach (InstrumentRegion region in regions)
            {
                StereoVoice voice = new(this, region, pitch);
                voices.Add(voice);
            }

            return voices;
        }

        private List<InstrumentRegion> GetInstrumentRegions(ushort presetNo, ushort bank, byte key, byte vel)
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
                        regions.Add(new(izone, inst.GrobalZone, pzone, preset.GrobalZone, key, vel));
                    }
                }
            }

            return regions;
        }
    }
}
