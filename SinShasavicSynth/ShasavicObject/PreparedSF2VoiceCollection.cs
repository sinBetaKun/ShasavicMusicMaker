using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SinShasavicSynthSF2.ShasavicObject
{
    internal class PreparedSF2VoiceCollection
    {
        public GetVoiceArg GetVoiceArg { get; init; }

        public ShasavicTone Tone { get; init; }

        public int Count { get; private set; }

        private readonly MixingSampleProvider _mixer;
        private readonly List<BuiltSF2> _builtSFs;
        private readonly byte _key;
        private readonly float _pitch;
        private readonly List<PreparedVoice> noteVoices = [];
        private readonly BlockingCollection<Action> commandQueue = [];
        private readonly Thread prepareThread;

        public PreparedSF2VoiceCollection(MixingSampleProvider mixer, List<BuiltSF2> builtSFs, GetVoiceArg getVoiceArg, int count)
        {
            GetVoiceArg = getVoiceArg;
            Count = count;

            _mixer = mixer;
            _builtSFs = builtSFs;
            Tone = new(getVoiceArg.BaseFrequency, getVoiceArg.Formula);
            float fkey = MathF.Round(69 + 12 * MathF.Log2(Tone.ResultFreq / 440.0f));
            _key = (byte)(fkey < 0 ? 0 : fkey > 127 ? 127 : fkey);
            _pitch = Tone.ResultFreq / (440.0f * MathF.Pow(2.0f, (_key - 69) / 12.0f));
            prepareThread = new Thread(PrepareThreadLoop)
            {
                IsBackground = true
            };
            prepareThread.Start();
            PrepareVoices();
        }

        private void PrepareThreadLoop()
        {
            foreach (Action cmd in commandQueue.GetConsumingEnumerable())
            {
                cmd();
            }
        }

        private void PrepareVoices()
        {
            commandQueue.Add(() => DoPrepareVoices());
        }

        private void DoPrepareVoices()
        {
            int needCount = Count - noteVoices.Count;

            if (needCount <= 0)
                return;

            List<PreparedVoice> list = [];

            for (int i = 0; i < needCount; i++)
            {

            }

            lock (noteVoices)
            {

            }
        }

        public List<PreparedVoice> GetVoices(int count)
        {
            lock (noteVoices)
            {
                int count2 = count < noteVoices.Count ? count : noteVoices.Count;
                List<PreparedVoice> ret = noteVoices[0..count2];
                PrepareVoices();
                return ret;
            }
        }
    }
}
