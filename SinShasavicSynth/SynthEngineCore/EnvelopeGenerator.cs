using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal class EnvelopeGenerator
    {
        public enum EnvelopeState { Idle, Attack, Decay, Sustain, Release, Done }
        public EnvelopeState State { get; private set; } = EnvelopeState.Idle;

        private readonly float attackRate;
        private readonly float decayRate;
        private readonly float sustainLevel;
        private readonly float releaseRate;

        private float level;

        public EnvelopeGenerator(float attackRate, float decayRate, float sustainLevel, float releaseRate)
        {
            this.attackRate = attackRate;
            this.decayRate = decayRate;
            this.sustainLevel = sustainLevel;
            this.releaseRate = releaseRate;
        }

        public EnvelopeGenerator(InstrumentRegion region)
        {
            int sampleRate = (int)region.SmplHdrs[0].SampleRate;
            int attack = region.Gens.TryGetValue(GeneratorType.attackVolEnv, out ushort value_A) ? (short)value_A : -12000;
            int decay = region.Gens.TryGetValue(GeneratorType.decayVolEnv, out ushort value_D) ? (short)value_D : -12000;
            int sustain = region.Gens.TryGetValue(GeneratorType.sustainVolEnv, out ushort value_S) ? (short)value_S : 0;
            int release = region.Gens.TryGetValue(GeneratorType.releaseVolEnv, out ushort value_R) ? (short)value_R : -12000;

            attackRate = TimecentsToRate(attack, sampleRate);
            decayRate = TimecentsToRate(decay, sampleRate);
            sustainLevel = CentibelsToAmplitude(sustain);
            releaseRate = TimecentsToRate(release, sampleRate);
        }

        private static float TimecentsToRate(int timecents, int sampleRate)
        {
            return MathF.Pow(2f, timecents / 1200f) / sampleRate;
        }

        private static float CentibelsToAmplitude(int centibels)
        {
            return MathF.Pow(10f, -centibels / 200f);
        }

        public void NoteOn()
        {
            State = EnvelopeState.Attack;
        }

        public void NoteOff()
        {
            State = EnvelopeState.Release;
        }

        public float Process()
        {
            switch (State)
            {
                case EnvelopeState.Attack:
                    level += attackRate;
                    if (level >= 1.0f)
                    {
                        level = 1.0f;
                        State = EnvelopeState.Decay;
                    }
                    break;
                case EnvelopeState.Decay:
                    level -= decayRate;
                    if (level <= sustainLevel)
                    {
                        level = sustainLevel;
                        State = EnvelopeState.Sustain;
                    }
                    break;
                case EnvelopeState.Sustain:
                    level = sustainLevel;
                    break;
                case EnvelopeState.Release:
                    level -= releaseRate;
                    if (level <= 0.0f)
                    {
                        level = 0.0f;
                        State = EnvelopeState.Done;
                    }
                    break;
                case EnvelopeState.Idle:
                    level = 0.0f;
                    break;
                case EnvelopeState.Done:
                    level = 0.0f;
                    break;
            }
            return level;
        }
    }
}
