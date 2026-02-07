using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using System.Collections.Concurrent;
using System.IO.MemoryMappedFiles;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class BuiltSF2 : IDisposable
    {
        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewAccessor _accessor;
        private readonly ConcurrentDictionary<uint, float[]> cache = new();
        private readonly BlockingCollection<SampleHeader_b> _preloadQueue = [];
        private readonly CancellationTokenSource _cts = new();
        private readonly Preset[] _presets;

        public int SampleRate { get; init; }

        public BuiltSF2(SF2RawData raw)
        {
            _mmf = MemoryMappedFile.CreateFromFile(raw.Path, FileMode.Open, null);
            _accessor = _mmf.CreateViewAccessor(raw.Sdta.Smpl.SamplePos, raw.Sdta.Smpl.Size, MemoryMappedFileAccess.Read);
            _presets = new Preset[raw.Pdta.Phdr.Headers.Length];

            for (ushort i = 0; i < _presets.Length; i++)
            {
                Preset preset = new(i, raw);
                _presets[i] = preset;
            }

            SampleRate = (int)raw.Pdta.Shdr.Headers[0].SampleRate;
            Task.Run(() => PreloadWorker(_cts.Token));
        }

        public void EnqueuePreload(SampleHeader_b header)
        {
            if (!cache.ContainsKey(header.Start))
                _preloadQueue.Add(header);
        }

        public void ResetCache()
        {
            cache.Clear();
        }

        private float[] ReadSample(uint start, uint count)
        {
            float[] result = new float[count];
            long bytePos = start * 2;

            for (int i = 0; i < count; i++)
            {
                short s = _accessor.ReadInt16(bytePos + i * 2);
                result[i] = s / 32768f;
            }

            return result;
        }

        public float[] GetSample(SampleHeader_b header)
        {
            if (cache.TryGetValue(header.Start, out float[]? data))
                return data;

            data = ReadSample(header.Start, header.End - header.Start);
            cache[header.Start] = data;
            return data;
        }

        private void PreloadWorker(CancellationToken token)
        {
            foreach (SampleHeader_b header in _preloadQueue.GetConsumingEnumerable(token))
            {
                if (cache.ContainsKey(header.Start)) continue;
                float[] data = ReadSample(header.Start, header.End - header.Start);
                cache[header.Start] = data;
            }
        }

        public List<NoteVoiceBase> GetVoices(float mVol, ushort presetNo, ushort bank, byte key, byte vel, float pitch = 1.0f)
        {
            if (GetKeyCompleter(presetNo, bank, key, vel) is not KeyCompleter completer)
                return [];

            List<NoteVoiceBase> voices = [];

            foreach (InstrumentRegion iRegion in completer.Regions)
            {
                SF2Voice voice = new(mVol, this, iRegion, pitch * completer.Pitch);
                voices.Add(voice);
            }

            return voices;
        }

        public KeyCompleter? GetKeyCompleter(ushort presetNo, ushort bank, byte key, byte vel)
        {
            if (_presets.FirstOrDefault(p => p.Presetno == presetNo && p.Bank == bank) is not Preset preset)
                return null;

            KeyCompleter? completer = GetKeyCompleter(preset, key, vel);

            if (completer is not null)
                return completer;

            KeyCompleter? completer_h = null;
            byte dis_h;

            for (dis_h = 1; key + dis_h < 128; dis_h++)
            {
                float pitch2 = MathF.Pow(2, dis_h / 12.0f);
                completer_h = GetKeyCompleter(preset, (byte)(key + dis_h), vel, pitch2);

                if (completer_h is not null)
                    break;
            }

            KeyCompleter? completer_l = null;
            byte dis_l;

            for (dis_l = 1; key - dis_l > -1; dis_l++)
            {
                float pitch2 = MathF.Pow(2, dis_l / 12.0f);
                completer_l = GetKeyCompleter(preset, (byte)(key - dis_l), vel, pitch2);

                if (completer_l is not null)
                    break;
            }

            if (completer_h is null)
            {
                if (completer_l is null)
                    return null;

                return completer_l;
            }
            else
            {
                if (completer_l is null)
                    return completer_h;

                if (dis_h < dis_l)
                    return completer_h;
                else
                    return completer_l;
            }
        }
        
        private KeyCompleter? GetKeyCompleter(Preset preset, byte key, byte vel, float pitch = 1.0f)
        {
            List<InstrumentRegion> iRegions = [];
            List<PresetZone> pzones = preset.GetMatchedZones(key, vel);

            foreach (PresetZone pzone in pzones)
            {
                if (pzone.Inst is Instrument inst)
                {
                    List<InstrumentZone> izones = inst.GetMatchedZones(key, vel);

                    foreach (InstrumentZone izone in izones)
                    {
                        InstrumentRegion iRegion = new(izone, inst.GrobalZone, pzone, preset.GrobalZone, key, vel);
                        iRegions.Add(iRegion);
                    }
                }
            }

            if (iRegions.Count == 0)
                return null;

            return new([.. iRegions], pitch);
        }

        public bool CheckPreset(ushort presetNo, ushort bank)
        {
            return _presets.FirstOrDefault(p => (p.Presetno == presetNo) && (p.Bank == bank)) is not null;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _preloadQueue.CompleteAdding();
            _accessor.Dispose();
            _mmf.Dispose();
        }
    }
}
