namespace SinShasavicSynthSF2.ShasavicObject.Event
{
    public class NoteOffArg(int ch, float baseFreq, int[] fml)
    {
        public int Channel { get; init; } = ch;

        public float BaseFrequency { get; init; } = baseFreq;


        public int[] Formula { get; init; } = fml;
    }
}
