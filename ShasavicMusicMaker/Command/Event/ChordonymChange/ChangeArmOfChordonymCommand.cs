using ShasavicMusicMaker.ScoreData.NoteData;

namespace ShasavicMusicMaker.Command.Event.ChordonymChange
{
    internal class ChangeArmOfChordonymCommand : IEditorCommand
    {
        public Chordonym Chordonym { get; init; }

        public Arm Before { get; init; }

        public Arm After { get; init; }

        public ChangeArmOfChordonymCommand(Chordonym chordonym, Arm before, Arm after)
        {
            Chordonym = chordonym;
            Before = before;
            After = after;
        }

        public void Execute()
        {
            Chordonym.ChangeArm(After);
        }

        public void Undo()
        {
            Chordonym.ChangeArm(Before);
        }
    }
}
