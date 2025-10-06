using System.Windows;
using ShasavicMusicMaker.Command;
using ShasavicMusicMaker.ScoreData.NoteData;
using SinShasavicSynthSF2.SynthEngineCore;

namespace ShasavicMusicMaker.Controller.Score
{
    /// <summary>
    /// ChordonymTest.xaml の相互作用ロジック
    /// </summary>
    public partial class ChordonymTest : Window
    {
        private readonly FunctionSynth synth;
        private readonly Chordonym chordonym;
        private readonly CommandStucker commandStucker;

        public ChordonymTest()
        {
            InitializeComponent();
            synth = new();
            chordonym = new(512);
            commandStucker = new();
            commandStucker.CommandSubscribed += UpdateUndoRedoButton;
            Viewer.SetCommandStucker(commandStucker);
            Viewer.DataContext = chordonym;
            Viewer.SetSelectorVerticalEdge(1000, -1000);
        }

        private void Add_1d_Click(object sender, RoutedEventArgs e)
        {
            Viewer.SetScoreLineDimension(0);
        }

        private void Add_2d_Click(object sender, RoutedEventArgs e)
        {
            Viewer.SetScoreLineDimension(1);
        }

        private void Add_3d_Click(object sender, RoutedEventArgs e)
        {
            Viewer.SetScoreLineDimension(2);
        }

        private void Add_4d_Click(object sender, RoutedEventArgs e)
        {
            Viewer.SetScoreLineDimension(3);
        }

        private void Add_5d_Click(object sender, RoutedEventArgs e)
        {
            Viewer.SetScoreLineDimension(4);
        }

        private void Add_6d_Click(object sender, RoutedEventArgs e)
        {
            Viewer.SetScoreLineDimension(5);
        }

        bool sceding = true;

        bool isPlaying = false;

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                isPlaying = false;
                playButton.Content = "[play] ";
                add1d.IsEnabled = true;
                add2d.IsEnabled = true;
                add3d.IsEnabled = true;
                add4d.IsEnabled = true;
                add5d.IsEnabled = true;
                add6d.IsEnabled = true;
                UpdateUndoRedoButton(sender, e);
                synth.AllNoteOff();
            }
            else
            {
                isPlaying = true;
                playButton.Content = "[pause]";
                add1d.IsEnabled = false;
                add2d.IsEnabled = false;
                add3d.IsEnabled = false;
                add4d.IsEnabled = false;
                add5d.IsEnabled = false;
                add6d.IsEnabled = false;
                Redo.IsEnabled = false;
                Undo.IsEnabled = false;

                List<Arm> arms1 = [chordonym.Arm];
                List<Arm> arms2 = [];

                while(arms1.Count > 0)
                {
                    foreach (Arm arm in arms1)
                    {
                        synth.NoteOn(0, chordonym.OrgnBaseFreq, BaseAndFormula.CalcBaseAndFomulaOfArm(arm).Formula, 127);
                        arms2.AddRange(arm.Arms);
                    }

                    arms1 = arms2;
                    arms2 = [];
                }
            }
        }

        private void UpdateUndoRedoButton(object sender, EventArgs e)
        {
            Undo.IsEnabled = commandStucker.CanUndo;
            Redo.IsEnabled = commandStucker.CanRedo;
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            commandStucker.Undo();
            UpdateUndoRedoButton(sender, e);
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            commandStucker.Redo();
            UpdateUndoRedoButton(sender, e);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Viewer.ChangeOctaveHeight(Viewer.OctaveHeight * 1.5f);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Viewer.ChangeOctaveHeight(Viewer.OctaveHeight / 1.5f);
        }
    }
}
