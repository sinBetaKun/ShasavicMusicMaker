namespace SinShasavicSynthSF2.ShasavicObject.Event
{
    internal interface INoteEventArg
    {
        public int Channel { get; init; }

        public float BaseFrequency { get; init; }

        public int[] Formula { get; init; }
    }
}
