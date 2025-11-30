using NAudio.Mixer;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.ShasavicObject.Event;
using SinShasavicSynthSF2.SynthEngineCore.Voice;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal class ShasavicNote
    {
        private readonly MixingSampleProvider _mixer;
        private readonly int _ch;
        private readonly ShasavicTone _tone;
        private readonly NoteVoiceBase[] _voices;
        private readonly bool[] _voicesFinished;
        public bool IsFinished { get; private set; }

        public ShasavicNote(MixingSampleProvider mixer, int ch, ShasavicTone tone, IEnumerable<NoteVoiceBase> voices)
        {
            _mixer = mixer;
            _ch = ch;
            _tone = tone;
            _voices = [.. voices];
            _voicesFinished = new bool[_voices.Length];

            foreach (NoteVoiceBase voice in voices)
            {
                if (voice.WaveFormat.SampleRate == _mixer.WaveFormat.SampleRate)
                {
                    _mixer.AddMixerInput(voice);
                }
                else
                {
                    WdlResamplingSampleProvider provider = new(voice, _mixer.WaveFormat.SampleRate);
                    _mixer.AddMixerInput(provider);
                }
            }
        }

        public void NoteOn()
        {
            foreach (NoteVoiceBase voice in _voices)
                voice.NoteOn();
        }

        public void NoteOff()
        {
            foreach (NoteVoiceBase voice in _voices)
                voice.NoteOff();
        }

        public void Cleanup()
        {
            bool flag = true;

            for (int i = 0; i < _voices.Length; i++)
            {
                if (!_voicesFinished[i])
                {
                    if (_voices[i].IsFinished)
                    {
                        _voicesFinished[i] = true;
                        _mixer.RemoveMixerInput(_voices[i]);
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }

            IsFinished = flag;
        }

        public bool IsApplicable(INoteEventArg arg)
            => _ch == arg.Channel && _tone.IsEqualTone(arg.BaseFrequency, arg.Formula);
    }
}
