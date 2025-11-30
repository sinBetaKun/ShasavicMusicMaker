using SinShasavicSynthSF2.SynthEngineCore.Voice;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal class PreparedVoice(NoteVoiceBase[] voices, ShasavicTone tone)
    {
        public NoteVoiceBase[] Voices { get; init; } = voices;
        public ShasavicTone Tone { get; init; } = tone;
    }
}
