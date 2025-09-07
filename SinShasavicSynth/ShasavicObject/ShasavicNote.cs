using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.SynthEngineCore.Voice;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal class ShasavicNote
    {
        private readonly MixingSampleProvider mixer;
        public readonly ShasavicTone tone;
        private readonly VoiceBase[] voices;
        private readonly bool[] voicesFinished;
        public bool IsFinished { get; private set; }

        public ShasavicNote(MixingSampleProvider mixer, ShasavicTone tone, VoiceBase[] voices)
        {
            this.mixer = mixer;
            this.tone = tone;
            this.voices = voices;
            voicesFinished = new bool[voices.Length];

            foreach (VoiceBase voice in voices)
                mixer.AddMixerInput(voice);
        }

        public void NoteOn()
        {
            foreach (VoiceBase voice in voices)
                voice.NoteOn();
        }

        public void NoteOff()
        {
            foreach (VoiceBase voice in voices)
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
                        mixer.RemoveMixerInput(voices[i]);
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
