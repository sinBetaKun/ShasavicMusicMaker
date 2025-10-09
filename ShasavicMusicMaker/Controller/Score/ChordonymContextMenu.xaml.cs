using ShasavicMusicMaker.DimensionData;
using ShasavicMusicMaker.ScoreData.NoteData;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;

namespace ShasavicMusicMaker.Controller.Score
{
    /// <summary>
    /// ChordonymContextMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class ChordonymContextMenu : UserControl
    {
        static readonly ResourceDictionary mainStyle = new()
        {
            Source = new Uri("pack://application:,,,/Styles/MainStyle.xaml")
        };
        static readonly ResourceDictionary dimSymbol = new()
        {
            Source = new Uri("pack://application:,,,/Symbol/DimensionSymbol.xaml")
        };

        public ChordonymContextMenu()
        {
            InitializeComponent();
        }

        private bool _pitchLineIsMuted;
        private const string _mutePitchLineText = "Mute this pitch line";
        private const string _unmutePitchLineText = "Unmute this pitch line";
        private bool _enableRemoveArm;

        public event EventHandler? ActionFinished;
        public event EventHandler? PitchLineMuted;
        public event EventHandler? PitchLineUnmuted;
        public event EventHandler? PitchLineRemoved;

        internal void BootPitchLineEditor(Arm arm)
        {
            MakeFormula(arm);

            if (arm.Muted)
            {
                MutePitchLineText.Text = _unmutePitchLineText;
                _pitchLineIsMuted = true;
            }
            else
            {
                MutePitchLineText.Text = _mutePitchLineText;
                _pitchLineIsMuted = false;
            }

            if (arm.Body is null)
            {
                _enableRemoveArm = false;
                RemovePitchLine.Style = (Style)mainStyle["InvalidContextMenuItem1"];
                RemovePitchLineText.Opacity = 0.5;
            }
            else
            {
                _enableRemoveArm = true;
                RemovePitchLine.Style = (Style)mainStyle["ContextMenuItem1"];
                RemovePitchLineText.Opacity = 1;
            }
        }

        private void MakeFormula(Arm arm)
        {
            FormulaPanel.Children.Clear();
            Canvas[][] symbols = [
                [(Canvas)dimSymbol["m1d"], (Canvas)dimSymbol["m1d_up"], (Canvas)dimSymbol["m1d_down"]],
                [(Canvas)dimSymbol["m2d"], (Canvas)dimSymbol["m2d_up"], (Canvas)dimSymbol["m2d_down"]],
                [(Canvas)dimSymbol["m3d"], (Canvas)dimSymbol["m3d_up"], (Canvas)dimSymbol["m3d_down"]],
                [(Canvas)dimSymbol["m4d"], (Canvas)dimSymbol["m4d_up"], (Canvas)dimSymbol["m4d_down"]],
                [(Canvas)dimSymbol["m5d"], (Canvas)dimSymbol["m5d_up"], (Canvas)dimSymbol["m5d_down"]],
                [(Canvas)dimSymbol["m6d"], (Canvas)dimSymbol["m6d_up"], (Canvas)dimSymbol["m6d_down"]],
                ];
            BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(arm);
            bool tone = true;

            for (int i = 0; i < DimensionInfo.MaxDimension; i++)
            {
                if (baf.Formula[i] > 0)
                {
                    tone = false;
                    FormulaPanel.Children.Add(new Rectangle() { Width = 2 });
                    FormulaPanel.Children.Add(symbols[i][0]);
                    FormulaPanel.Children.Add(new Rectangle() { Width = 2 });
                    string xaml = XamlWriter.Save(symbols[i][1]);

                    for (int j = 0; j < baf.Formula[i]; j++)
                        FormulaPanel.Children.Add((UIElement)XamlReader.Parse(xaml));
                }
                else if (baf.Formula[i] < 0)
                {
                    tone = false;
                    FormulaPanel.Children.Add(new Rectangle() { Width = 2 });
                    FormulaPanel.Children.Add(symbols[i][0]);
                    FormulaPanel.Children.Add(new Rectangle() { Width = 2 });
                    string xaml = XamlWriter.Save(symbols[i][2]);

                    for (int j = 0; j > baf.Formula[i]; j--)
                        FormulaPanel.Children.Add((UIElement)XamlReader.Parse(xaml));
                }
            }

            if (tone)
            {
                FormulaPanel.Children.Add((Canvas)dimSymbol["m0d"]);
            }
        }

        private void MenuItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border item)
            {
                if (!MenuItemIsEnable(item))
                    return;

                item.Style = (Style)mainStyle["SelectedContextMenuItem1"];
            }
        }

        private void MenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border item)
            {
                if (!MenuItemIsEnable(item))
                    return;

                item.Style = (Style)mainStyle["ContextMenuItem1"];
            }
        }

        private void MutePitchLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_pitchLineIsMuted)
            {
                PitchLineUnmuted.Invoke(sender, e);
            }
            else
            {
                PitchLineMuted.Invoke(sender, e);
            }

            ActionFinished.Invoke(sender, e);
        }

        private void RemovePitchLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_enableRemoveArm)
            {
                return;
            }

            PitchLineRemoved.Invoke(sender, e);
            ActionFinished.Invoke(sender, e);
        }

        private bool MenuItemIsEnable(Border item)
        {
            if (item == RemovePitchLine)
            {
                if (!_enableRemoveArm)
                {
                    return false;
                }
            }

            return true;
        }


    }
}
