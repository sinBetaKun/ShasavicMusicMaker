using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using SinShasavicSynthSF2.Audio;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.SoundFont;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal class Voice : ISampleProvider
    {
        private readonly float[] sampleBuffer;
        private readonly int sampleRate;
        private int position;
        private bool isLooping;
        private int loopStart;
        private int loopEnd;

        public WaveFormat WaveFormat { get; }

        public bool IsFinished { get; private set; } = false;

        public Voice(short[] pcmData, int sampleRate, int loopStart = -1, int loopEnd = -1)
        {
            this.sampleRate = sampleRate;
            this.WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);

            // short[] → float[] に変換（-1.0～1.0）
            this.sampleBuffer = new float[pcmData.Length];
            for (int i = 0; i < pcmData.Length; i++)
            {
                this.sampleBuffer[i] = pcmData[i] / 32768f;
            }

            // ループ設定
            this.isLooping = (loopStart >= 0 && loopEnd > loopStart);
            this.loopStart = loopStart;
            this.loopEnd = loopEnd;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (IsFinished) return 0;

            int samplesWritten = 0;
            while (samplesWritten < count)
            {
                if (position >= sampleBuffer.Length)
                {
                    if (isLooping)
                    {
                        position = loopStart;
                    }
                    else
                    {
                        IsFinished = true;
                        break;
                    }
                }

                buffer[offset + samplesWritten] = sampleBuffer[position];
                samplesWritten++;
                position++;
            }

            return samplesWritten;
        }

        // releaseTimeMs は SF2の releaseVolEnv の値（ミリ秒）
        float CalculateReleaseRate(float releaseTimeMs)
        {
            if (releaseTimeMs <= 0) return 0f;

            // 1フレームあたりの減衰率を計算（指数関数的減衰）
            float releaseSeconds = releaseTimeMs / 1000f;
            float releaseRate = MathF.Exp(-1f / (releaseSeconds * sampleRate));
            return releaseRate;
        }
    }
}
