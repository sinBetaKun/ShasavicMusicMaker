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