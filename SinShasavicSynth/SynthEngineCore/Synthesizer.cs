namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class Synthesizer
    {
        private readonly VoiceManager _voiceManager = new();

        public Synthesizer()
        {
        }

        public void LoadSoundFontList(IEnumerable<string> paths)
        {
            _voiceManager.LoadSoundFontList(paths);
        }

        public void NoteOn(int ch, float baseFreq, int[] formula, byte vel)
        {
            _voiceManager.NoteOn(ch, baseFreq, formula, vel);
        }

        public void NoteOff(int ch, float baseFreq, int[] formula)
        {
            _voiceManager.NoteOff(ch, baseFreq, formula);
        }

        public void AllNoteOff()
        {
            _voiceManager.AllNoteOff();
        }
    }
}
