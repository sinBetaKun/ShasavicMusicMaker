namespace SinShasavicSynthSF2.ShasavicObject.Event
{
    public class NoteOnArg(int ch, float baseFreq, int[] fml, byte vel)
    {
        public int Channel { get; init; } = ch;

        public float BaseFrequency { get; init; } = baseFreq;

        public int[] Formula { get; init; } = fml;

        public byte Velocity { get; init; } = vel;
    }
}
