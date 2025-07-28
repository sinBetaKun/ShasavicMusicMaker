using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal class EnvelopeGenerator
    {
        enum State { Idle, Attack, Decay, Sustain, Release, Done }
        State currentState;

        float currentValue;       // 現在の音量（0〜1）
        float releaseTime;        // Releaseにかける時間（秒）
        float releaseRate;        // サンプルごとの減衰量（currentValue / (releaseTime * sampleRate)）

        float sampleRate;

        public void NoteOff()
        {
            if (currentState == State.Attack || currentState == State.Decay || currentState == State.Sustain)
            {
                currentState = State.Release;
                releaseRate = currentValue / (releaseTime * sampleRate);
            }
        }

        public float ProcessNextSample()
        {
            switch (currentState)
            {
                // 省略：Attack, Decay, Sustainの処理...

                case State.Release:
                    currentValue -= releaseRate;
                    if (currentValue <= 0.0f)
                    {
                        currentValue = 0.0f;
                        currentState = State.Done;
                    }
                    break;
            }

            return currentValue;
        }

        public bool IsFinished => currentState == State.Done;
    }
}
