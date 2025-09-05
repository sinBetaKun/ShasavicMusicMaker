using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NAudio.Wave;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal class SineWaveVoice : VoiceBase
    {
        EnvelopeGenerator ampEnvelope;
        private readonly float frequency;
        private int sampleRate;
        private double phase;
        private int sample = 0;
        public override WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);

        public SineWaveVoice(float frequency, int sampleRate = 44100)
        {
            ampEnvelope = new(0.1f, 0.1f, 1, 0.1f);
            this.frequency = frequency;
            this.sampleRate = sampleRate;
        }

        public override void NoteOn()
        {
            ampEnvelope.NoteOn();
        }

        public override void NoteOff()
        {
            ampEnvelope.NoteOff();
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            for (int i = 0; i < count / 2; i++)
            {
                float envVal = ampEnvelope.Process();
                buffer[offset + i * 2] = (float)Math.Sin((2 * Math.PI * frequency * sample) / WaveFormat.SampleRate) * envVal;
                buffer[offset + i * 2 + 1] = (float)Math.Sin((2 * Math.PI * frequency * sample) / WaveFormat.SampleRate) * envVal;
                sample++;
                sample %= (int)(WaveFormat.SampleRate / (2 * Math.PI * frequency * sample));
            }

            return count;
        }
    }
}
