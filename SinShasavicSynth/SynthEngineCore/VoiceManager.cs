
using NAudio.Mixer;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using System.Collections.Immutable;
using System.Threading.Channels;
using Timer = System.Timers.Timer;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal class VoiceManager
    {
        private class Channel
        {
            private readonly List<BuiltSF2> _builtSFs;
            private readonly List<ShasavicNote> _stack = [];
            private readonly MixingSampleProvider _mixer;

            public Channel(MixingSampleProvider mixer, List<BuiltSF2> builtSFs)
            {
                this._mixer = mixer;
                this._builtSFs = builtSFs;
            }

            public void NoteOn(float baseFreq, int[] formula, byte vel)
            {
                ShasavicTone tone = new(baseFreq, formula);
                byte key = (byte)Math.Round(69 + 12 * MathF.Log2(tone.ResultFreq / 440.0f));
                float pitch = tone.ResultFreq / 440.0f * MathF.Pow(2.0f, (key - 69) / 12.0f);
                List<VoiceBase> voices;

                foreach (BuiltSF2 builtSF in _builtSFs)
                {
                    voices = builtSF.GetVoices(0, 0, key, vel, pitch);

                    foreach (VoiceBase voice in voices)
                    {
                        if (voice.WaveFormat.SampleRate == _mixer.WaveFormat.SampleRate)
                        {
                            _mixer.AddMixerInput(voice);
                        }
                        else
                        {
                            WdlResamplingSampleProvider provider = new(voice, _mixer.WaveFormat.SampleRate);
                            _mixer.AddMixerInput(provider);
                        }

                        voice.NoteOn();
                    }

                    if (voices.Count > 0)
                    {
                        ShasavicNote note = new(_mixer, tone, voices);
                        _stack.Add(note);
                        note.NoteOn();
                    }
                }
            }

            public void NoteOff(float baseFreq, int[] formula)
            {
                if (_stack.FirstOrDefault(note => note.tone.IsEqualTone(baseFreq, formula)) is ShasavicNote note)
                {
                    note.NoteOff();
                }
            }

            public void AllNoteOff()
            {
                foreach (ShasavicNote note in _stack)
                    note.NoteOff();
            }

            public void Cleanup()
            {
                List<ShasavicNote> finishedNotes = [];

                foreach (ShasavicNote note in _stack)
                {
                    note.Cleanup();

                    if (note.IsFinished)
                        finishedNotes.Add(note);
                }

                foreach (ShasavicNote note in finishedNotes)
                    _stack.Remove(note);
            }
        }

        private const int SampleRate = 44100;
        private string[] _sfPaths = [];
        private readonly List<BuiltSF2> _builtSFs = [];
        private readonly MixingSampleProvider _mixer;
        private readonly WaveOutEvent _output;
        private readonly Channel[] _channels = new Channel[16];
        private readonly Timer _cleanupTimer;

        public VoiceManager()
        {
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 2))
            {
                ReadFully = true
            };
            _output = new WaveOutEvent();
            _output.Init(_mixer);
            _output.Volume = 0.3f; // 0.0（無音）〜1.0（最大）
            _output.Play();

            for (int i = 0; i < 16; i++)
                _channels[i] = new(_mixer, _builtSFs);

            _cleanupTimer = new Timer(100); // 100msごと
            _cleanupTimer.Elapsed += (s, e) => Cleanup();
            _cleanupTimer.Start();
        }

        public void LoadSoundFontList(IEnumerable<string> paths)
        {
            _sfPaths = [.. paths];
            _builtSFs.Clear();

            foreach (Channel ch in _channels)
                ch.Cleanup();

            foreach (string path in _sfPaths)
                _builtSFs.Add(Sf2Loader.GetBuiltSF2(path));
        }

        public void NoteOn(int ch, float baseFreq, int[] formula, byte vel)
        {
            _channels[ch].NoteOn(baseFreq, formula, vel);
        }

        public void NoteOff(int ch, float baseFreq, int[] formula)
        {
            _channels[ch].NoteOff(baseFreq, formula);
        }

        public void AllNoteOff()
        {
            foreach (Channel channel in _channels)
                channel.AllNoteOff();
        }

        public void Cleanup()
        {
            foreach (Channel channel in _channels)
                channel.Cleanup();
        }
    }
}
