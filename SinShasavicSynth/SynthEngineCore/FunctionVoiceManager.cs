using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using Timer = System.Timers.Timer;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal class FunctionVoiceManager
    {
        private enum WaveFunction
        {
            NoiseAttackSine,
            Extra
        }

        private class Channel
        {
            private WaveFunction function = WaveFunction.NoiseAttackSine;
            private readonly List<ShasavicNote> stack = [];
            private readonly MixingSampleProvider mixer;
            private readonly int sampleRate;

            public Channel(MixingSampleProvider mixer, int sampleRate = 44100)
            {
                this.mixer = mixer;
                this.sampleRate = sampleRate;
            }

            public void ChangeWaveFunction(WaveFunction function)
            {
                this.function = function;
            }

            public void NoteOn(float baseFreq, int[] formula, int vel)
            {
                ShasavicTone tone = new(baseFreq, formula);
                NoteVoiceBase voice;

                switch (function)
                {
                    case WaveFunction.NoiseAttackSine:
                        voice = new NoiseAttackSineVoice(tone.ResultFreq, vel / 127f, sampleRate);
                        break;
                    default:
                        voice = new ExtraWaveVoice(tone.ResultFreq, vel / 127f, sampleRate);
                        break;
                }

                ShasavicNote note = new(mixer, tone, [voice]);
                stack.Add(note);
                note.NoteOn();
            }

            public void NoteOff(float baseFreq, int[] formula)
            {
                if (stack.FirstOrDefault(note => note.tone.IsEqualTone(baseFreq, formula)) is ShasavicNote note)
                {
                    note.NoteOff();
                }
            }

            public void AllNoteOff()
            {
                foreach (ShasavicNote note in stack)
                    note.NoteOff();
            }

            public void Cleanup()
            {
                foreach (ShasavicNote note in stack)
                {
                    note.Cleanup();

                    if (note.IsFinished)
                        stack.Remove(note);
                }
            }
        }

        private readonly int SampleRate;
        private readonly MixingSampleProvider mixer;
        private readonly WaveOutEvent output;
        private readonly Channel[] channels = new Channel[16];
        private readonly Timer cleanupTimer;

        public FunctionVoiceManager(float volume = 0.5f, int samplerate = 44100)
        {
            SampleRate = samplerate;
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 2))
            {
                ReadFully = true
            };
            output = new WaveOutEvent();
            output.Init(mixer);
            output.Volume = volume;
            output.Play();

            for (int i = 0; i < 16; i++)
                channels[i] = new(mixer, SampleRate);

            cleanupTimer = new Timer(100); // 100msごと
            cleanupTimer.Elapsed += (s, e) => Cleanup();
            cleanupTimer.Start();
        }

        public void NoteOn(int ch, float baseFreq, int[] formula, int vel)
        {
            channels[ch].NoteOn(baseFreq, formula, vel);
        }

        public void NoteOff(int ch, float baseFreq, int[] formula)
        {
            channels[ch].NoteOff(baseFreq, formula);
        }

        public void AllNoteOff()
        {
            foreach (Channel channel in channels)
                channel.AllNoteOff();
        }

        public void Cleanup()
        {
            foreach (Channel channel in channels)
                channel.Cleanup();
        }
    }
}
