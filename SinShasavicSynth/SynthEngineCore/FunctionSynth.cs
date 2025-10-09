
namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class FunctionSynth
    {
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
