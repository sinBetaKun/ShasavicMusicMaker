namespace SinShasavicSynthSF2.ShasavicObject.Event
{
    public class NoteOffArg : INoteEventArg
    {
        public int Channel { get; init; }

        public float BaseFrequency { get; init; }

        public int[] Formula { get; init; }

        public NoteOffArg(int ch, float baseFreq, int[] fml)
        {
            Channel = ch;
            BaseFrequency = baseFreq;
            Formula = fml;
        }

        public NoteOffArg(NoteOnArg noteOnArg)
        {
            Channel = noteOnArg.Channel;
            BaseFrequency = noteOnArg.BaseFrequency;
            Formula = [.. noteOnArg.Formula];
        }
    }
}
