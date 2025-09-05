using NAudio.Wave;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal class StereoVoice : VoiceBase
    {
        private readonly EnvelopeGenerator ampEnvelope;

        private readonly float[] sampleBuffer_L;
        private readonly float[] sampleBuffer_R;
        private readonly int sampleRate;
        private float position_L;
        private float position_R;
        private readonly bool isLooping_L;
        private readonly bool isLooping_R;
        private readonly int loopStart_L;
        private readonly int loopStart_R;
        private readonly int loopEnd_L;
        private readonly int loopEnd_R;
        private readonly float constPitchRatio = 1.0f;

        public override WaveFormat WaveFormat { get; }
        public bool IsFinished { get; private set; } = false;
        private bool isFinished_L = false;
        private bool isFinished_R = false;

        public StereoVoice(BuiltSF2 builtData, InstrumentRegion region)
        {
            SampleHeader_b header_L = region.SmplHdrs[0];
            SampleHeader_b header_R = region.SmplHdrs[1];

            ampEnvelope = new(region);

            uint start_L = header_L.Start;
            uint end_L = header_L.End;
            uint length_L = end_L - start_L;
            sampleBuffer_L = new float[length_L];
            Array.Copy(builtData.Samples, start_L, sampleBuffer_L, 0, length_L);

            uint start_R = header_R.Start;
            uint end_R = header_R.End;
            uint length_R = end_R - start_R;
            sampleBuffer_R = new float[length_R];
            Array.Copy(builtData.Samples, start_R, sampleBuffer_R, 0, length_R);

            sampleRate = (int)header_L.SampleRate;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

            loopStart_L = (int)(header_L.Loopstart - start_L);
            loopEnd_L = (int)(header_L.Loopend - start_L);
            isLooping_L = loopStart_L < loopEnd_L;

            loopStart_R = (int)(header_R.Loopstart - start_R);
            loopEnd_R = (int)(header_R.Loopend - start_R);
            isLooping_R = loopStart_R < loopEnd_R;
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
            if (IsFinished) return 0;
            
            int samplesWritten = 0;
            float envelopeValue;
            int index_L, index_R;
            bool jumped_L, jumped_R;

            while (samplesWritten * 2 < count)
            {
                jumped_L = false;

                if (isLooping_L)
                {
                    if (position_L >= loopEnd_L)
                    {
                        position_L = loopStart_L;
                        jumped_L = true;
                    }
                }
                else
                {
                    if (position_L >= sampleBuffer_L.Length)
                    {
                        isFinished_L = true;
                    }
                }

                jumped_R = false;

                if (isLooping_R)
                {
                    if (position_R >= loopEnd_R)
                    {
                        position_R = loopStart_R;
                        jumped_R = true;
                    }
                }
                else
                {
                    if (position_R >= sampleBuffer_R.Length)
                    {
                        isFinished_R = true;
                    }
                }

                if (isFinished_L && isFinished_R)
                {
                    IsFinished = true;
                    break;
                }

                envelopeValue = ampEnvelope.Process();

                if (isFinished_L)
                {
                    buffer[offset + samplesWritten * 2] = 0;
                }
                else
                {
                    index_L = (int)position_L;

                    if (jumped_L)
                    {
                        buffer[offset + samplesWritten * 2] = 0;
                    }
                    else
                    {
                        buffer[offset + samplesWritten * 2] = envelopeValue * sampleBuffer_L[index_L];
                    }

                    position_L += constPitchRatio;
                }

                if (isFinished_R)
                {
                    buffer[offset + samplesWritten * 2 + 1] = 0;
                }
                else
                {
                    index_R = (int)position_R;

                    if (jumped_R)
                    {
                        buffer[offset + samplesWritten * 2 + 1] = 0;
                    }
                    else
                    {
                        buffer[offset + samplesWritten * 2 + 1] = envelopeValue * sampleBuffer_R[index_R];
                    }

                    position_R += constPitchRatio;
                }


                if (ampEnvelope.State == EnvelopeGenerator.EnvelopeState.Done)
                {
                    IsFinished = true;
                    break;
                }

                samplesWritten++;
            }

            return samplesWritten * 2;
        }
    }
}
