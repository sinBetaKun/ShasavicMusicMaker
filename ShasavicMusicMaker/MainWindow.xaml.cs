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
using SinShasavicSynthSF2.SoundFont;

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

        private void LoadSF2Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog od = new() { Filter = "Soundfont2(*.sf2)|*.sf2|すべてのファイル(*.*)|*.*" };
            DialogResult result = od.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Sf2Loader loader = new(od.FileName);
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
    }
}