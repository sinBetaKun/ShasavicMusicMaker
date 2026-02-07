namespace SinShasavicSynthSF2.ShasavicObject.Event
{
    public class NoteOnArg(int ch, float baseFreq, int[] fml, byte vel) : INoteEventArg
    {
        public int Channel { get; init; } = ch;

        public float BaseFrequency { get; init; } = baseFreq;

        public int[] Formula { get; init; } = fml;

        public byte Velocity { get; init; } = vel;

        public bool IsApplicable(NoteOffArg arg)
        {
            if (Channel == arg.Channel && BaseFrequency == arg.BaseFrequency)
                return Formula.SequenceEqual(arg.Formula);

            return false;
        }
    }
}
