using NAudio.Wave;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal class SF2Voice : NoteVoiceBase
    {
        private readonly EnvelopeGenerator ampEnvelope;

        private readonly float[] sampleBuffer_L;
        private readonly float[] sampleBuffer_R;
        private readonly int sampleRate;
        private double position_L;
        private double position_R;
        private readonly bool isLooping_L;
        private readonly bool isLooping_R;
        private readonly int loopStart_L;
        private readonly int loopStart_R;
        private readonly int loopEnd_L;
        private readonly int loopEnd_R;
        private readonly float constPitchRatio;
        private readonly float manegerVol;

        public override WaveFormat WaveFormat { get; }

        private bool isFinished_L = false;
        private bool isFinished_R = false;

        public SF2Voice(float mVol, BuiltSF2 builtData, InstrumentRegion region, float pitch = 1.0f, float vol = 1.0f)
        {
            manegerVol = mVol;
            constPitchRatio = DefaultPitchCalculater.Calc(region) * pitch;
            ampEnvelope = new(region);

            switch (region.SmplHdrs.Length)
            {
                case 1:
                    SampleHeader_b header = region.SmplHdrs[0];
                    sampleBuffer_L = sampleBuffer_R = builtData.GetSample(header);
                    uint start = header.Start;
                    uint length = header.End - start;

                    for (int i = 0; i < length; i++)
                    {
                        sampleBuffer_L[i] *= vol;
                        if (sampleBuffer_L[i] < -1) sampleBuffer_L[i] = -1;
                        else if (sampleBuffer_L[i] > 1) sampleBuffer_L[i] = 1;
                    }

                    sampleRate = (int)header.SampleRate;
                    WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

                    loopStart_L = loopStart_R = (int)(header.Loopstart - start);
                    loopEnd_L = loopEnd_R = (int)(header.Loopend - start);
                    isLooping_L = isLooping_R = header.IsLoop;

                    break;

                case 2:
                    SampleHeader_b header_L = region.SmplHdrs[0];
                    SampleHeader_b header_R = region.SmplHdrs[1];
                    sampleBuffer_L = builtData.GetSample(header_L);
                    uint start_L = header_L.Start;
                    uint length_L = header_L.End - start_L;

                    for (int i = 0; i < length_L; i++)
                    {
                        sampleBuffer_L[i] *= vol;
                        if (sampleBuffer_L[i] < -1) sampleBuffer_L[i] = -1;
                        else if (sampleBuffer_L[i] > 1) sampleBuffer_L[i] = 1;
                    }

                    sampleBuffer_R = builtData.GetSample(header_R);
                    uint start_R = header_R.Start;
                    uint length_R = header_R.End - start_R;

                    for (int i = 0; i < length_R; i++)
                    {
                        sampleBuffer_R[i] *= vol;
                        if (sampleBuffer_R[i] < -1) sampleBuffer_R[i] = -1;
                        else if (sampleBuffer_R[i] > 1) sampleBuffer_R[i] = 1;
                    }

                    sampleRate = (int)header_L.SampleRate;
                    WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

                    loopStart_L = (int)(header_L.Loopstart - start_L);
                    loopEnd_L = (int)(header_L.Loopend - start_L);
                    isLooping_L = header_L.IsLoop;

                    loopStart_R = (int)(header_R.Loopstart - start_R);
                    loopEnd_R = (int)(header_R.Loopend - start_R);
                    isLooping_R = header_R.IsLoop;
                    break;

                default:
                    throw new Exception("Unsupported format: count of channels is " + region.SmplHdrs.Length);
            }
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

            while (samplesWritten * 2 < count)
            {
                bool jumped_L = false;

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

                bool jumped_R = false;

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

                float envelopeValue = ampEnvelope.Process(constPitchRatio);

                if (isFinished_L)
                {
                    buffer[offset + samplesWritten * 2] = 0;
                }
                else
                {
                    if (jumped_L)
                    {
                        buffer[offset + samplesWritten * 2] = 0;
                    }
                    else
                    {
                        int i1 = (int)position_L;
                        int i2 = (i1 + 1) % sampleBuffer_L.Length;
                        double frac = position_L - i1;
                        buffer[offset + samplesWritten * 2] = 
                            (float)(envelopeValue * (sampleBuffer_L[i1] * (1 - frac) + sampleBuffer_L[i2] * frac)) * manegerVol;
                    }

                    position_L += constPitchRatio;
                }

                if (isFinished_R)
                {
                    buffer[offset + samplesWritten * 2 + 1] = 0;
                }
                else
                {
                    if (jumped_R)
                    {
                        buffer[offset + samplesWritten * 2 + 1] = 0;
                    }
                    else
                    {
                        int i1 = (int)position_R;
                        int i2 = (i1 + 1) % sampleBuffer_R.Length;
                        double frac = position_R - i1;
                        buffer[offset + samplesWritten * 2 + 1] =
                            (float)(envelopeValue * (sampleBuffer_R[i1] * (1 - frac) + sampleBuffer_R[i2] * frac)) * manegerVol;
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
