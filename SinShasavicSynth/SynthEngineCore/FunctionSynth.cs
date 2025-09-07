
namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class FunctionSynth
    {
        private readonly int SampleRate = 44100;
        private readonly float Volume = 0.1f; // 0.0（無音）〜1.0（最大）
        private readonly FunctionVoiceManager voiceManager = new();

        public FunctionSynth()
        {
        }

        public void NoteOn(int ch, float baseFreq, int[] formula, int vel)
        {
            voiceManager.NoteOn(ch, baseFreq, formula, vel);
        }

        public void NoteOff(int ch, float baseFreq, int[] formula)
        {
            voiceManager.NoteOff(ch, baseFreq, formula);
        }

        public void AllNoteOff()
        {
            voiceManager.AllNoteOff();
        }
    }
}
