using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;

namespace SinShasavicSynthSF2.SynthEngineCore
{
    internal static class DefaultPitchCalculater
    {
        public static float Calc(InstrumentRegion region)
        {
            int o = region.Gens.TryGetValue(GeneratorType.overridingRootKey, out ushort value_o) ? value_o : region.SmplHdrs[0].OriginalKey;
            int c = region.Gens.TryGetValue(GeneratorType.coarseTune, out ushort value_c) ? value_c / 10 : 0;
            int f = region.Gens.TryGetValue(GeneratorType.fineTune, out ushort value_f) ? value_f : 0;

            return MathF.Pow(2.0f, (region.Key - o + c + f / 100.0f) / 12.0f);
        }
    }
}
