using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class Synthesizer
    {
        private MixingSampleProvider? mixer;
        private WaveOutEvent? output;
        private BuiltSF2? builtData;
        private List<VoiceBase> stack = [];
        private ExtraWaveVoice ex_voice;

        public Synthesizer()
        {
            ex_voice = new(440, 1);
        }

        public void LoadSoundFont(string path)
        {
            builtData = Sf2Loader.GetBuiltSF2(path);
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(builtData.SampleRate, 2))
            {
                ReadFully = true
            };
            output?.Dispose();
            output = new WaveOutEvent();
            output.Init(mixer);
            output.Volume = 0.01f; // 0.0（無音）〜1.0（最大）
            output.Play();
        }

        public void Test()
        {
            if (builtData is null) return;
            if (mixer is null) return;

            List<VoiceBase> voices = builtData.GetVoices(0, 0, 69, 100);

            if (voices.Count == 0)
                return;

            foreach (VoiceBase voice in voices)
            {
                mixer.AddMixerInput(voice);
                voice.NoteOn();
                stack.Add(voice);
            }
        }

        public void Stop()
        {
            foreach (VoiceBase voice in stack)
            {
                voice.NoteOff();
            }
            stack.Clear();
        }

        public void Switch_Extra()
        {
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ReadFully = true
            };
            output?.Dispose();
            output = new WaveOutEvent();
            output.Init(mixer);
            output.Volume = 0.5f; // 0.0（無音）〜1.0（最大）
            output.Play();
        }

        public void Test_Extra()
        {
            if (mixer is null) return;

            mixer.AddMixerInput(ex_voice);
            ex_voice.NoteOn();
            stack.Add(ex_voice);
        }
    }
}
