using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SoundFont.SF2Data.RawData;

namespace SinShasavicSynthSF2.SoundFont
{
    public class Sf2Loader
    {
        public static BuiltSF2 GetBuiltSF2(string path)
        {
            SF2RawData rawData = new(path);
            return new(rawData);
        }
    }
}
