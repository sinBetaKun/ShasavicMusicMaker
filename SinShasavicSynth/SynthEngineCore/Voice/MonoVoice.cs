using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NAudio.Wave;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal class MonoVoice : VoiceBase
    {
        private readonly EnvelopeGenerator ampEnvelope;

        private readonly float[] sampleBuffer;
        private readonly int sampleRate;
        private float position;
        private readonly bool isLooping;
        private readonly int loopStart;
        private readonly int loopEnd;
        private float pitchRatio = 1.0f;

        public override WaveFormat WaveFormat { get; }
        public bool IsFinished { get; private set; } = false;

        public MonoVoice(BuiltSF2 builtData, InstrumentRegion region)
        {
            SampleHeader_b header = region.SmplHdrs[0];
            ampEnvelope = new(region);

            uint start = header.Start;
            uint end = header.End;
            uint length = end - start;
            sampleBuffer = new float[length];
            Array.Copy(builtData.Samples, start, sampleBuffer, 0, length);
            sampleRate = (int)header.SampleRate;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);

            // ループ設定
            loopStart = (int)(header.Loopstart - start);
            loopEnd = (int)(header.Loopend - start);
            isLooping = loopStart < loopEnd;
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
            bool jumped;

            while (samplesWritten * 2 < count)
            {
                if (isLooping)
                {
                    if (position >= loopEnd)
                    {
                        position = loopStart;
                        jumped = true;
                    }
                }
                else
                {
                    if (position >= sampleBuffer.Length)
                    {
                        IsFinished = true;
                        break;
                    }
                }

                float envelopeValue = ampEnvelope.Process();
                int index = (int)position;
                buffer[offset + samplesWritten * 2] = sampleBuffer[index] * envelopeValue;
                buffer[offset + samplesWritten * 2 + 1] = sampleBuffer[index] * envelopeValue;

                if (ampEnvelope.State == EnvelopeGenerator.EnvelopeState.Done)
                {
                    IsFinished = true;
                    break;
                }

                samplesWritten++;
                position += pitchRatio;
            }

            return samplesWritten;
        }
    }
}
