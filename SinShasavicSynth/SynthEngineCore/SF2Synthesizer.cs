using NAudio.Mixer;
using NAudio.Wave;
using SinShasavicSynthSF2.ShasavicObject.Event;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class SF2Synthesizer
    {
        private readonly SF2VoiceManager _voiceManager = new();

        public SF2Synthesizer()
        {
        }

        public void LoadSoundFontList(IEnumerable<string> paths)
        {
            _voiceManager.LoadSF2List(paths);
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
