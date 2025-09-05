using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal enum GeneratorType : byte
    {
        attackVolEnv = 34,
        decayVolEnv = 36,
        sustainVolEnv = 37,
        releaseVolEnv = 38,
        Instrument = 41,
        KeyRange = 43,
        VelRange = 44,
        SampleID = 53,
        sampleModes = 54,
    }
}
