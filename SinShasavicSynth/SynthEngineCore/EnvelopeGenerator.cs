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
        public enum EnvelopeState { Idle, Delay, Attack, Hold, Decay, Sustain, Release, Done }
        public EnvelopeState State { get; private set; } = EnvelopeState.Idle;

        private readonly float delayVolRate;
        private readonly float attackVolRate;
        private readonly float holdVolRate;
        private readonly float decayVolRate;
        private readonly float sustain = 0.0f;
        private readonly float releaseVolRate;

        private float level = 0.0f;
        private float delay = 0.0f;
        private float attack = 0.0f;
        private float hold = 0.0f;
        private float decay = 0.0f;
        private float release = 0.0f;

        // k : カーブの急峻さを決める定数（大きいほど急上昇）
        // k = 2 → ゆっくり滑らかに立ち上がる
        // k = 5 → 自然な耳当たり
        // k = 8 → パンッと素早く鳴る印象
        private const float k_atk = 5;
        private const float k_dcy = 5;
        private const float k_rls = 5;

        public EnvelopeGenerator(
            float attackVolRate, float decayVolRate, float sustain, float releaseVolRate,
            float delayVolRate = 1, float holdVolRate = 1)
        {
            this.delayVolRate = delayVolRate;
            this.attackVolRate = attackVolRate;
            this.holdVolRate = holdVolRate;
            this.decayVolRate = decayVolRate;
            this.sustain = sustain;
            this.releaseVolRate = releaseVolRate;
        }

        public EnvelopeGenerator(InstrumentRegion region)
        {
            int sampleRate = (int)region.SmplHdrs[0].SampleRate;
            int delay = region.Gens.TryGetValue(GeneratorType.delayVolEnv, out ushort value_Dl) ? (short)value_Dl : -12000;
            int attack = region.Gens.TryGetValue(GeneratorType.attackVolEnv, out ushort value_At) ? (short)value_At : -12000;
            int hold = region.Gens.TryGetValue(GeneratorType.holdVolEnv, out ushort value_Hl) ? (short)value_Hl : -12000;
            int decay = region.Gens.TryGetValue(GeneratorType.decayVolEnv, out ushort value_Dc) ? (short)value_Dc : -12000;
            int sustain = region.Gens.TryGetValue(GeneratorType.sustainVolEnv, out ushort value_Ss) ? (short)value_Ss : 0;
            int release = region.Gens.TryGetValue(GeneratorType.releaseVolEnv, out ushort value_Rl) ? (short)value_Rl : -12000;

            delayVolRate = TimecentsToRate(delay, sampleRate);
            attackVolRate = TimecentsToRate(attack, sampleRate);
            holdVolRate = TimecentsToRate(hold, sampleRate);
            decayVolRate = TimecentsToRate(decay, sampleRate);
            this.sustain = CentibelsToAmplitude(sustain);
            releaseVolRate = TimecentsToRate(release, sampleRate);
        }

        private static float TimecentsToRate(int timecents, int sampleRate)
        {
            return 1 / MathF.Pow(2f, timecents / 1200f) / sampleRate;
        }

        private static float CentibelsToAmplitude(int centibels)
        {
            return MathF.Pow(10f, -centibels / 200f);
        }

        public void NoteOn()
        {
            State = EnvelopeState.Delay;
        }

        public void NoteOff()
        {
            State = EnvelopeState.Release;
        }

        public float Process(float speed = 1)
        {
            switch (State)
            {
                case EnvelopeState.Delay:
                    delay += delayVolRate * speed;

                    if (delay >= 1.0f)
                        State = EnvelopeState.Attack;

                    break;

                case EnvelopeState.Attack:
                    attack += attackVolRate * speed;

                    if (attack >= 1.0f)
                    {
                        level = 1.0f;
                        State = EnvelopeState.Hold;
                    }
                    else
                        level = (1.0f - MathF.Exp(-k_atk * attack)) / (1.0f - MathF.Exp(-k_atk));

                    break;

                case EnvelopeState.Hold:
                    hold += holdVolRate * speed;

                    if (hold >= 1.0f)
                        State = EnvelopeState.Decay;

                    break;

                case EnvelopeState.Decay:
                    decay += decayVolRate * speed;

                    if (decay >= 1.0f)
                    {
                        level = sustain;
                        State = EnvelopeState.Sustain;
                    }
                    else
                        level = sustain + (1.0f - sustain) * MathF.Exp(-k_dcy * decay);

                    break;

                case EnvelopeState.Sustain:
                    break;

                case EnvelopeState.Release:
                    release += releaseVolRate * speed;

                    if (release >= 1.0f)
                    {
                        level = 0.0f;
                        State = EnvelopeState.Done;
                    }
                    else
                        level *= MathF.Exp(-k_rls * release);

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
