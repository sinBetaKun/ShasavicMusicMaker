using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        private Arm leaf;

        public ChordonymTest()
        {
            InitializeComponent();
            synth = new();
            chordonym = new(440);
            leaf = chordonym.Arm;
            Viewer.DataContext = chordonym;
        }

        private void Pop_Click(object sender, RoutedEventArgs e)
        {
            if (leaf != chordonym.Arm)
            {
                Arm tmp = leaf;
                leaf = leaf.Body;
                leaf.Arms.Remove(tmp);
                Viewer.UpdateChordonym();
            }
        }

        private void Add_1d_Click(object sender, RoutedEventArgs e)
        {
            (bool, Arm) tmp = leaf.AddArm(new(1, sceding));
            if (tmp.Item1)
            {
                leaf = tmp.Item2;
                Viewer.UpdateChordonym();
            }
        }

        private void Add_2d_Click(object sender, RoutedEventArgs e)
        {
            (bool, Arm) tmp = leaf.AddArm(new(2, sceding));
            if (tmp.Item1)
            {
                leaf = tmp.Item2;
                Viewer.UpdateChordonym();
            }
        }

        private void Add_3d_Click(object sender, RoutedEventArgs e)
        {
            (bool, Arm) tmp = leaf.AddArm(new(3, sceding));
            if (tmp.Item1)
            {
                leaf = tmp.Item2;
                Viewer.UpdateChordonym();
            }
        }

        private void Add_4d_Click(object sender, RoutedEventArgs e)
        {
            (bool, Arm) tmp = leaf.AddArm(new(4, sceding));
            if (tmp.Item1)
            {
                leaf = tmp.Item2;
                Viewer.UpdateChordonym();
            }
        }

        private void Add_5d_Click(object sender, RoutedEventArgs e)
        {
            (bool, Arm) tmp = leaf.AddArm(new(5, sceding));
            if (tmp.Item1)
            {
                leaf = tmp.Item2;
                Viewer.UpdateChordonym();
            }
        }

        private void Add_6d_Click(object sender, RoutedEventArgs e)
        {
            (bool, Arm) tmp = leaf.AddArm(new(6, sceding));
            if (tmp.Item1)
            {
                leaf = tmp.Item2;
                Viewer.UpdateChordonym();
            }
        }

        bool sceding = true;

        private void toggle_Click(object sender, RoutedEventArgs e)
        {
            if (sceding)
            {
                sceding = false;
                toggle.Content = "[down]";
            }
            else
            {
                sceding = true;
                toggle.Content = " [up] ";
            }
        }

        bool isPlaying = false;

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                isPlaying = false;
                playButton.Content = "[play] ";
                popArm.IsEnabled = true;
                add1d.IsEnabled = true;
                add2d.IsEnabled = true;
                add3d.IsEnabled = true;
                add4d.IsEnabled = true;
                add5d.IsEnabled = true;
                add6d.IsEnabled = true;
                synth.AllNoteOff();
            }
            else
            {
                isPlaying = true;
                playButton.Content = "[pause]";
                popArm.IsEnabled = false;
                add1d.IsEnabled = false;
                add2d.IsEnabled = false;
                add3d.IsEnabled = false;
                add4d.IsEnabled = false;
                add5d.IsEnabled = false;
                add6d.IsEnabled = false;

                List<Arm> arms1 = [chordonym.Arm];
                List<Arm> arms2 = [];

                while(arms1.Count > 0)
                {
                    foreach (Arm arm in arms1)
                    {
                        synth.NoteOn(0, 261, BaseAndFormula.CalcBaseAndFomulaOfArm(arm).Formula, 127);
                        arms2.AddRange(arm.Arms);
                    }

                    arms1 = arms2;
                    arms2 = [];
                }
            }
        }
    }
}
