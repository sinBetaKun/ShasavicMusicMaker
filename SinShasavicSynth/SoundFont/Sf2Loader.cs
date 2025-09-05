using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;
using SinShasavicSynthSF2.SynthEngineCore.Voice;

namespace SinShasavicSynthSF2.SoundFont
{
    internal class Sf2Loader
    {
        public static BuiltSF2 GetBuiltSF2(string path)
        {
            SF2RawData rawData = new(path);
            return new(rawData);
        }
    }
}
