using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShasavicMusicMaker.Enum;

namespace ShasavicMusicMaker.Controller.MainWindow
{
    /// <summary>
    /// EditModeSwitch.xaml の相互作用ロジック
    /// </summary>
    public partial class EditModeSwitch : UserControl
    {
        public EditModeSwitch()
        {
            InitializeComponent();
        }

        public EditMode SelectedMode
        {
            get { return (EditMode)GetValue(SelectedModeProperty); }
            set { SetValue(SelectedModeProperty, value); }
        }

        public static readonly DependencyProperty SelectedModeProperty =
            DependencyProperty.Register("SelectedMode", typeof(EditMode), typeof(EditModeSwitch), new PropertyMetadata(EditMode.Add));



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton_Click(sender);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton_Click(sender);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton_Click(sender);
        }

        private void ToggleButton_Click(object sender)
        {
            var clicked = sender as ToggleButton;

            // すべてオフにしてから、自分だけオンに
            foreach (var child in (panel as Panel).Children)
            {
                if (child is ToggleButton btn)
                {
                    btn.IsChecked = btn == clicked;
                }
            }
        }
    }
}
