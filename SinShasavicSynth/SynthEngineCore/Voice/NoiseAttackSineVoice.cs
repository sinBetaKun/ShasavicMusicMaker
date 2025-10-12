using NAudio.Wave;
using static SinShasavicSynthSF2.SynthEngineCore.EnvelopeGenerator;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal class NoiseAttackSineVoice : NoteVoiceBase
    {
        private readonly EnvelopeGenerator ampEnvelope;
        private readonly float frequency;
        private readonly int sampleRate;
        private readonly float vel;
        private double phase = 0;
        private readonly double phaseIncrement;
        private int noisePhase;
        private readonly float noiseLength = 0.03f;
        private readonly float cutoffHz = 100;
        private readonly float[] noiseSamples;
        private readonly float waveNoseRate = 0.6f;

        public override WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

        public NoiseAttackSineVoice(float frequency, float vel, int sampleRate = 44100)
        {
            ampEnvelope = new(0.004f, 0.00005f, 0.6f, 0.0001f);
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.vel = vel;

            #region make noise
            noisePhase = (int)(sampleRate * noiseLength);
            noiseSamples = new float[noisePhase * 2];
            float[] preNoise = new float[noisePhase * 2];
            Random rand = new();

            for (int i = 0; i < preNoise.Length; i++)
                preNoise[i] = (float)(rand.NextDouble() * 2.0 - 1.0) * (1 - waveNoseRate);

            float dt = 1f / sampleRate;
            float RC = 1f / (2 * MathF.PI * cutoffHz);
            float alpha = dt / (RC + dt);

            for (int i = 1; i < noiseSamples.Length; i++)
            {
                noiseSamples[i] = noiseSamples[i - 1] + alpha * (preNoise[i] - noiseSamples[i - 1]);
            }

            for (int i = noisePhase - 1; i >= 0; i--)
            {
                float coef = MathF.Pow((float)i / noisePhase, 2);
                noiseSamples[i * 2] *= coef;
                noiseSamples[i * 2 + 1] *= coef;
            }

            noisePhase--;
            #endregion

            phaseIncrement = 2.0 * Math.PI * frequency / WaveFormat.SampleRate;
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
            if (IsFinished)
            {
                return 0;
            }

            for (int i = 0; i < count / 2; i++)
            {
                float envVal = ampEnvelope.Process();

                if (ampEnvelope.State == EnvelopeState.Done)
                {
                    IsFinished = true;
                    return i * 2;
                }

                float sinValue = (float)Math.Sin(phase) * envVal;
                float extraValue = sinValue * waveNoseRate;

                if (noisePhase >= 0)
                {
                    buffer[offset + i * 2] = (extraValue + noiseSamples[2 * noisePhase]) * vel;
                    buffer[offset + i * 2 + 1] = (extraValue + noiseSamples[2 * noisePhase + 1]) * vel;
                    noisePhase--;
                }
                else
                {
                    buffer[offset + i * 2] = buffer[offset + i * 2 + 1] = extraValue;
                }

                phase += phaseIncrement;
                if (phase >= 2.0 * Math.PI)
                    phase -= 2.0 * Math.PI;
            }

            return count;
        }
    }
}
