using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShasavicMusicMaker.Controller.Score;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SynthEngineCore;

namespace ShasavicMusicMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Synthesizer synthesizer;

        public MainWindow()
        {
            InitializeComponent();
            synthesizer = new Synthesizer();
        }

        private void LoadSF2Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog od = new() { Filter = "Soundfont2(*.sf2)|*.sf2" };
            DialogResult result = od.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    synthesizer.LoadSoundFont(od.FileName);
                    System.Windows.Forms.MessageBox.Show(
                        "OK",
                        "OK",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(
                        ex.Message + "\n---\n" + ex.StackTrace,
                        "Failure to load sf2",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Stop
                        );
                }
            }
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            synthesizer.Test();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            synthesizer.Stop();
        }

        ChordonymTest? chordonymTest;

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (chordonymTest is null)
            {
                chordonymTest = new ChordonymTest();
                chordonymTest.Closed += (_, _) => chordonymTest = null;
                chordonymTest.Show();
            }
            else
            {
                chordonymTest.Activate();
            }
        }
    }
}