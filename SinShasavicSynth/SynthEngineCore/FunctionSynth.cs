
using SinShasavicSynthSF2.ShasavicObject.Event;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class FunctionSynth
    {
        private readonly FunctionVoiceManager _voiceManager = new();

        public FunctionSynth()
        {
        }

        public void NoteOn(IEnumerable<NoteOnArg> args)
        {
            _voiceManager.NoteOn(args);
        }

        public void NoteOff(IEnumerable<NoteOffArg> args)
        {
            _voiceManager.NoteOff(args);
        }

        public void AllNoteOff()
        {
            _voiceManager.AllNoteOff();
        }
    }
}
