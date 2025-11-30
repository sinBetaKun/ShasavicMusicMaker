using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.ShasavicObject.Event;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using System.Collections.Concurrent;
using Timer = System.Timers.Timer;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class SF2VoiceManager : IDisposable
    {
        private readonly WasapiOut _output;
        private readonly MixingSampleProvider _mixer;
        private readonly List<BuiltSF2> _builtSFs = [];
        private readonly List<ShasavicNote> _current = [];
        private readonly Timer _cleanupTimer;
        private readonly BlockingCollection<Action> commandQueue = [];
        private readonly Thread audioThread;

        public bool AnySF2sSeted => _builtSFs.Any();

        public SF2VoiceManager()
        {
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ReadFully = true
            };
            _output = new WasapiOut(AudioClientShareMode.Shared, true, 5);
            _output.Init(_mixer);
            _output.Volume = 0.000001f;
            _output.Play();

            _cleanupTimer = new Timer(100); // 100msごと
            _cleanupTimer.Elapsed += (s, e) => Cleanup();
            _cleanupTimer.Start();

            audioThread = new Thread(AudioThreadLoop)
            {
                IsBackground = true
            };
            audioThread.Start();
        }

        private void AudioThreadLoop()
        {
            foreach (Action cmd in commandQueue.GetConsumingEnumerable())
            {
                cmd();
            }
        }

        public void LoadSF2List(IEnumerable<string> paths)
        {
            foreach (BuiltSF2 builtSF2 in _builtSFs)
                builtSF2.Dispose();

            _builtSFs.Clear();
            
            foreach (string path in paths)
            {
                if (File.Exists(path))
                    _builtSFs.Add(Sf2Loader.GetBuiltSF2(path));
            }
        }

        public void Preload(IEnumerable<GetVoiceArg> args)
        {
            foreach (GetVoiceArg arg in args)
            {
                foreach (BuiltSF2 builtSF in _builtSFs)
                {
                    if (builtSF.CheckPreset(arg.PresetNo, arg.Bank))
                    {
                        ShasavicTone tone = new(arg.BaseFrequency, arg.Formula);
                        float fkey = MathF.Round(69 + 12 * MathF.Log2(tone.ResultFreq / 440.0f));
                        byte key = (byte)(fkey < 0 ? 0 : fkey > 127 ? 127 : fkey);
                        
                        if (builtSF.GetKeyCompleter(arg.PresetNo, arg.Bank, key, arg.Velocity)
                            is KeyCompleter completer)
                        {
                            foreach (InstrumentRegion region in completer.Regions)
                                foreach (SampleHeader_b header in region.SmplHdrs)
                                    builtSF.EnqueuePreload(header);
                        }
                    }
                }
            }
        }

        public void NoteOn(IEnumerable<NoteOnArg> args)
        {
            commandQueue.Add(() => DoNoteOn(args));
        }

        public void NoteOff(IEnumerable<NoteOffArg> args)
        {
            commandQueue.Add(() => DoNoteOff(args));
        }

        private void DoNoteOn(IEnumerable<NoteOnArg> args)
        {
            foreach (BuiltSF2 builtSF in _builtSFs)
            {
                if (builtSF.CheckPreset(0, 0))
                {
                    List<ShasavicNote> notes = [];

                    foreach (NoteOnArg arg in args)
                    {
                        ShasavicTone tone = new(arg.BaseFrequency, arg.Formula);
                        float fkey = MathF.Round(69 + 12 * MathF.Log2(tone.ResultFreq / 440.0f));
                        byte key = (byte)(fkey < 0 ? 0 : fkey > 127 ? 127 : fkey);
                        float pitch = tone.ResultFreq / (440.0f * MathF.Pow(2.0f, (key - 69) / 12.0f));
                        ShasavicNote note = new(_mixer, arg.Channel, tone, builtSF.GetVoices(0, 0, key, arg.Velocity, pitch));
                        notes.Add(note);
                        _current.Add(note);
                    }

                    foreach (ShasavicNote note in notes)
                        note.NoteOn();

                    break;
                }
            }
        }

        private void DoNoteOff(IEnumerable<NoteOffArg> args)
        {
            List<ShasavicNote> notes = [];

            foreach (NoteOffArg arg in args)
            {
                if (_current.FirstOrDefault(note => note.IsApplicable(arg)) is ShasavicNote note)
                {
                    notes.Add(note);
                    _current.Remove(note);
                }
            }

            foreach (ShasavicNote note in notes)
                note.NoteOff();
        }

        public void AllNoteOff()
        {
            foreach (ShasavicNote note in _current)
                note.NoteOff();

            _current.Clear();
        }

        public void Cleanup()
        {
            List<ShasavicNote> toRemove = [];

            foreach (ShasavicNote note in _current)
            {
                note.Cleanup();

                if (note.IsFinished)
                    toRemove.Add(note);
            }

            foreach (ShasavicNote note in toRemove)
                _current.Remove(note);
        }

        public void Dispose()
        {
            foreach (BuiltSF2 builtSF2 in _builtSFs)
                builtSF2.Dispose();
        }
    }
}
