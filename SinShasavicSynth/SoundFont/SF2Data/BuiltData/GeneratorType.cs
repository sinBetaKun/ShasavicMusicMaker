using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal enum GeneratorType : byte
    {
        delayVolEnv = 33,
        attackVolEnv = 34,
        holdVolEnv = 35,
        decayVolEnv = 36,
        sustainVolEnv = 37,
        releaseVolEnv = 38,
        Instrument = 41,
        KeyRange = 43,
        VelRange = 44,
        coarseTune = 51,
        fineTune = 52,
        SampleID = 53,
        sampleModes = 54,
        scaleTuning = 56,
        overridingRootKey = 58,
    }
}
