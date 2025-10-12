using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData
{
    internal class KeyVelCompleter(InstrumentRegion[] regions, float pitch, float vol)
    {
        public readonly InstrumentRegion[] Regions = regions;
        public readonly float Pitch = pitch;
        public readonly float Vol = vol;
    }
}
