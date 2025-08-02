using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;

namespace SinShasavicSynthSF2.SoundFont
{
    public class Sf2Loader
    {
        readonly BuiltSF2 builtData;

        public Sf2Loader(string path)
        {
            SF2RawData rawData = new(path);
            builtData = new(rawData);
        }
    }
}
