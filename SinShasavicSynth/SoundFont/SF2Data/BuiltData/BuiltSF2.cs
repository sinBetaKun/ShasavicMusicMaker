using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    public class BuiltSF2
    {
        //private Preset[] Presets { get; init; }

        /// <summary>
        /// ぼ く が か ん が え た
        /// KeyとVelの一番近い音を探すための
        /// さ い き ょ う の マ ッ プ
        /// </summary>

        private readonly PresetKVMap[] maps;

        public float[] Samples { get; init; }

        public int SampleRate { get; init; }

        internal BuiltSF2(SF2RawData raw)
        {
            //Presets = new Preset[raw.Pdta.Phdr.Headers.Length];
            maps = new PresetKVMap[raw.Pdta.Phdr.Headers.Length];

            //for (ushort i = 0; i < Presets.Length; i++)
            //{
            //    Presets[i] = new(i, raw);
            //}

            for (ushort i = 0; i < maps.Length; i++)
            {
                Preset preset = new(i, raw);
                maps[i] = new(preset);
            }

            Samples = new float[raw.Sdta.Smpl.Samples.Length];

            for (int i = 0; i < Samples.Length; i++)
            {
                Samples[i] = raw.Sdta.Smpl.Samples[i] / 32768.0f;
            }

            SampleRate = (int)raw.Pdta.Shdr.Headers[0].SampleRate;
        }

        public List<NoteVoiceBase> GetVoices(ushort presetNo, ushort bank, byte key, byte vel, float pitch = 1.0f)
        {
            List<NoteVoiceBase> voices = [];

            if (GetKVCompleter(presetNo, bank, key, vel) is KeyVelCompleter completer)
            {
                foreach (InstrumentRegion region in completer.Regions)
                {
                    StereoVoice voice = new(this, region, pitch * completer.Pitch, completer.Vol);
                    voices.Add(voice);
                }

                return voices;
            }

            return [];
        }

        public bool CheckPreset(ushort presetNo, ushort bank)
        {
            return maps.FirstOrDefault(m => (m.PresetNo == presetNo) && (m.Bank == bank)) is not null;
        }

        private KeyVelCompleter? GetKVCompleter(ushort presetNo, ushort bank, byte key, byte vel)
        {
            PresetKVMap? map = maps.FirstOrDefault(
                    m => (m.PresetNo == presetNo) && (m.Bank == bank));

            if (map == null)
            {
                return null;
            }

            return map.GetKVCompleter(key, vel);
        }

        //public List<NoteVoiceBase> GetVoices(ushort presetNo, ushort bank, byte key, byte vel, float pitch = 1.0f)
        //{
        //    List<NoteVoiceBase> voices = [];
        //    List<InstrumentRegion> regions = GetInstrumentRegions(presetNo, bank, key, vel);

        //    foreach (InstrumentRegion region in regions)
        //    {
        //        StereoVoice voice = new(this, region, pitch);
        //        voices.Add(voice);
        //    }

        //    return voices;
        //}

        //private List<InstrumentRegion> GetInstrumentRegions(ushort presetNo, ushort bank, byte key, byte vel)
        //{
        //    Preset? preset = Presets.FirstOrDefault(
        //        p => (p.Presetno == presetNo) && (p.Bank == bank));

        //    if (preset == null)
        //    {
        //        return [];
        //    }

        //    return GetInstrumentRegions(preset, key, vel);
        //}
    }
}
