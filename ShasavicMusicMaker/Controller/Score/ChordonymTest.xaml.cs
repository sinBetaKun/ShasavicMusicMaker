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

namespace ShasavicMusicMaker.Controller.Score
{
    /// <summary>
    /// ChordonymTest.xaml の相互作用ロジック
    /// </summary>
    public partial class ChordonymTest : Window
    {
        public ChordonymTest()
        {
            InitializeComponent();
            chordonym = new(440);
            leaf = chordonym.Arm;
            Viewer.DataContext = chordonym;
        }

        private Chordonym chordonym;
        private Arm leaf;

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
    }
}
