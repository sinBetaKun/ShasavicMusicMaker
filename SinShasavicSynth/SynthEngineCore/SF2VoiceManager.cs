using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.ShasavicObject.Event;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using Timer = System.Timers.Timer;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class SF2VoiceManager
    {

        private class Channel
        {
            private readonly List<BuiltSF2> _builtSFs;
            private readonly List<ShasavicNote> _stack = [];
            private readonly MixingSampleProvider _mixer;

            public Channel(MixingSampleProvider mixer, List<BuiltSF2> builtSFs)
            {
                _mixer = mixer;
                _builtSFs = builtSFs;
            }

            public void NoteOn(IEnumerable<NoteOnArg> args)
            {
                foreach (BuiltSF2 builtSF in _builtSFs)
                {
                    if (builtSF.CheckPreset(0,0))
                    {
                        List<NoteVoiceBase> voices = [];

                        foreach (NoteOnArg arg in args)
                        {
                            ShasavicTone tone = new(arg.BaseFrequency, arg.Formula);
                            float fkey = MathF.Round(69 + 12 * MathF.Log2(tone.ResultFreq / 440.0f));
                            byte key = (byte)(fkey < 0 ? 0 : fkey > 127 ? 127 : fkey);
                            float pitch = tone.ResultFreq / (440.0f * MathF.Pow(2.0f, (key - 69) / 12.0f));
                            _stack.Add(new(_mixer, tone, _builtSFs[0].GetVoices(0, 0, key, arg.Velocity, pitch)));
                        }

                        foreach (ShasavicNote note in _stack)
                            note.NoteOn();

                        break;
                    }
                }
            }

            public void NoteOff(IEnumerable<NoteOffArg> args)
            {
                foreach (NoteOffArg arg in args)
                {
                    if (_stack.FirstOrDefault(note => note.tone.IsEqualTone(arg.BaseFrequency, arg.Formula)) is ShasavicNote note)
                    {
                        note.NoteOff();
                        _stack.Remove(note);
                    }
                }
            }

            public void AllNoteOff()
            {
                foreach (ShasavicNote note in _stack)
                    note.NoteOff();

                _stack.Clear();
            }

            public void Cleanup()
            {
                foreach (ShasavicNote note in _stack)
                {
                    note.Cleanup();

                    if (note.IsFinished)
                        _stack.Remove(note);
                }
            }
        }

        private readonly WaveOutEvent _output;
        private readonly MixingSampleProvider _mixer;
        private readonly List<BuiltSF2> _builtSFs = [];
        private readonly Channel[] _channels = new Channel[16];
        private readonly Timer _cleanupTimer;

        public bool AnySF2sSeted => _builtSFs.Any();

        public SF2VoiceManager()
        {
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ReadFully = true
            };
            _output = new WaveOutEvent();
            _output.Init(_mixer);
            _output.Volume = 0.5f;
            _output.Play();

            for (int i = 0; i < 16; i++)
                _channels[i] = new(_mixer, _builtSFs);

            _cleanupTimer = new Timer(100); // 100msごと
            _cleanupTimer.Elapsed += (s, e) => Cleanup();
            _cleanupTimer.Start();
        }

        public void LoadSF2List(IEnumerable<string> paths)
        {
            _builtSFs.Clear();
            foreach (string path in paths)
            {
                _builtSFs.Add(Sf2Loader.GetBuiltSF2(path));
            }
        }

        public void NoteOn(int ch, IEnumerable<NoteOnArg> args)
        {
            _channels[ch].NoteOn(args);
        }

        public void Test2(IEnumerable<NoteOnArg> args)
        {
            _channels[0].NoteOn(args);
        }

        public void AllNoteOff()
        {
            foreach (Channel channel in _channels)
            {
                channel.AllNoteOff();
            }
        }

        public void Cleanup()
        {
            foreach (Channel channel in _channels)
                channel.Cleanup();
        }
    }
}
