using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace SinShasavicSynthSF2.SynthEngineCore.Voice
{
    internal abstract class VoiceBase : ISampleProvider
    {
        public abstract WaveFormat WaveFormat { get; }

        public abstract void NoteOn();

        public abstract void NoteOff();

        public abstract int Read(float[] buffer, int offset, int count);
    }
}
