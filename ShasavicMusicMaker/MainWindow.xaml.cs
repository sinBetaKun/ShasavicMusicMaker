using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ShasavicMusicMaker.Controller.Score;
using SinShasavicSynthSF2.ShasavicObject.Event;
using SinShasavicSynthSF2.SoundFont;
using SinShasavicSynthSF2.SoundFont.SF2Data.BuiltData;
using SinShasavicSynthSF2.SynthEngineCore;
using SinShasavicSynthSF2.SynthEngineCore.Voice;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace ShasavicMusicMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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