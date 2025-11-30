using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.ShasavicObject;
using SinShasavicSynthSF2.ShasavicObject.Event;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using System.Collections.Concurrent;
using Timer = System.Timers.Timer;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    public class FunctionVoiceManager
    {
        private enum WaveFunction
        {
            NoiseAttackSine,
            Extra
        }

        private readonly int SampleRate;
        private readonly MixingSampleProvider _mixer;
        private readonly WasapiOut output;
        private WaveFunction[] _functions = new WaveFunction[16];
        private readonly List<ShasavicNote> _current = [];
        private readonly Timer cleanupTimer;
        private readonly BlockingCollection<Action> commandQueue = [];
        private readonly Thread audioThread;

        public FunctionVoiceManager(float volume = 0.5f, int samplerate = 44100)
        {
            SampleRate = samplerate;
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 2))
            {
                ReadFully = true
            };
            output = new WasapiOut(AudioClientShareMode.Shared, true, 5);
            output.Init(_mixer);
            output.Volume = volume;
            output.Play();

            for (int i = 0; i < 16; i++)
                _functions[i] = WaveFunction.NoiseAttackSine;

            cleanupTimer = new Timer(100); // 100msごと
            cleanupTimer.Elapsed += (s, e) => Cleanup();
            cleanupTimer.Start();

            audioThread = new Thread(AudioThreadLoop)
            {
                IsBackground = true
            };
            audioThread.Start();
        }

        private void AudioThreadLoop()
        {
            foreach (Action cmd in commandQueue.GetConsumingEnumerable())
            {
                cmd();
            }
        }

        public void NoteOn(IEnumerable<NoteOnArg> args)
        {
            commandQueue.Add(() => DoNoteOn(args));
        }

        public void NoteOff(IEnumerable<NoteOffArg> args)
        {
            commandQueue.Add(() => DoNoteOff(args));
        }

        private void DoNoteOn(IEnumerable<NoteOnArg> args)
        {
            List<ShasavicNote> notes = [];

            foreach (NoteOnArg arg in args)
            {
                ShasavicTone tone = new(arg.BaseFrequency, arg.Formula);
                NoteVoiceBase voice = _functions[arg.Channel] switch
                {
                    WaveFunction.NoiseAttackSine => new NoiseAttackSineVoice(tone.ResultFreq, arg.Velocity / 127f, SampleRate),
                    _ => new ExtraWaveVoice(tone.ResultFreq, arg.Velocity / 127f, SampleRate),
                };
                ShasavicNote note = new(_mixer, arg.Channel, tone, [voice]);
                notes.Add(note);
                _current.Add(note);
            }

            foreach (ShasavicNote note in notes)
                note.NoteOn();
        }

        private void DoNoteOff(IEnumerable<NoteOffArg> args)
        {
            List<ShasavicNote> notes = [];

            foreach (NoteOffArg arg in args)
            {
                if (_current.FirstOrDefault(note => note.IsApplicable(arg)) is ShasavicNote note)
                {
                    notes.Add(note);
                    _current.Remove(note);
                }
            }

            foreach (ShasavicNote note in notes)
                note.NoteOff();
        }

        public void AllNoteOff()
        {
            foreach (ShasavicNote note in _current)
                note.NoteOff();

            _current.Clear();
        }

        public void Cleanup()
        {
            foreach (ShasavicNote note in _current)
            {
                note.Cleanup();

                if (note.IsFinished)
                    _current.Remove(note);
            }
        }
    }
}
