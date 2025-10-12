using NAudio.Wave;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal class SineWaveVoice : NoteVoiceBase
    {
        EnvelopeGenerator ampEnvelope;
        private readonly float frequency;
        private int sampleRate;
        private double phase;
        private int sample = 0;
        public override WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

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
                float sineValue = (float)MathF.Sin(2 * MathF.PI * frequency * sample / WaveFormat.SampleRate) * envVal;
                buffer[offset + i * 2] = sineValue;
                buffer[offset + i * 2 + 1] = sineValue;
                sample++;
                sample %= (int)(WaveFormat.SampleRate / frequency);
            }

            return count;
        }
    }
}
