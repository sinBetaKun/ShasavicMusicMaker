namespace SinShasavicSynthSF2.ShasavicObject
{
    public class GetVoiceArg(ushort presetNo, ushort bank, float baseFreq, int[] fml, byte vel)
    {
        public ushort PresetNo { get; init; } = presetNo;
        public ushort Bank { get; init; } = bank;
        public float BaseFrequency { get; init; } = baseFreq;
        public int[] Formula { get; init; } = fml;
        public byte Velocity { get; init; } = vel;

        public bool IsEqual(GetVoiceArg other)
        {
            return PresetNo == other.PresetNo
                && Bank == other.Bank
                && BaseFrequency == other.BaseFrequency
                && Formula.SequenceEqual(other.Formula)
                && Velocity == other.Velocity;
        }
    }
}
