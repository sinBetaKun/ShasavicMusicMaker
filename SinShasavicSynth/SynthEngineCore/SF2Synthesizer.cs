using NAudio.Mixer;
using NAudio.Wave;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class SF2Synthesizer
    {
        private readonly SF2VoiceManager _voiceManager = new();
        private readonly WaveOutEvent _output;

        public SF2Synthesizer()
        {
            _output = new WaveOutEvent();
            _output.Init(_voiceManager);
            _output.Volume = 0.3f; // 0.0（無音）〜1.0（最大）
            _output.Play();
        }

        public void LoadSoundFontList(IEnumerable<string> paths)
        {
            _voiceManager.LoadSoundFontList(paths);
        }

        public bool NoteOn(int ch, float baseFreq, int[] formula, byte vel)
        {
            return _voiceManager.NoteOn(ch, baseFreq, formula, vel);
        }

        public bool NoteOff(int ch, float baseFreq, int[] formula)
        {
            return _voiceManager.NoteOff(ch, baseFreq, formula);
        }

        public void AllNoteOff()
        {
            _voiceManager.AllNoteOff();
        }
    }
}
