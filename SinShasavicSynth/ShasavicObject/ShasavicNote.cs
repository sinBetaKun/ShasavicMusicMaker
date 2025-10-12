using NAudio.Mixer;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.SynthEngineCore.Voice;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal class ShasavicNote
    {
        private readonly MixingSampleProvider _mixer;
        public readonly ShasavicTone tone;
        private readonly NoteVoiceBase[] voices;
        private readonly bool[] voicesFinished;
        public bool IsFinished { get; private set; }

        public ShasavicNote(MixingSampleProvider mixer, ShasavicTone tone, IEnumerable<NoteVoiceBase> voices)
        {
            _mixer = mixer;
            this.tone = tone;
            this.voices = [.. voices];
            voicesFinished = new bool[this.voices.Length];

            foreach (NoteVoiceBase voice in voices)
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
            }
        }

        public void NoteOn()
        {
            foreach (NoteVoiceBase voice in voices)
                voice.NoteOn();
        }

        public void NoteOff()
        {
            foreach (NoteVoiceBase voice in voices)
                voice.NoteOff();
        }

        public void Cleanup()
        {
            bool flag = true;

            for (int i = 0; i < voices.Length; i++)
            {
                if (!voicesFinished[i])
                {
                    if (voices[i].IsFinished)
                    {
                        voicesFinished[i] = true;
                        _mixer.RemoveMixerInput(voices[i]);
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }

            IsFinished = flag;
        }
    }
}
