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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShasavicMusicMaker.Controller.MainWindow
{
    /// <summary>
    /// ScoreCanvas.xaml の相互作用ロジック
    /// </summary>
    public partial class ScoreCanvas : UserControl
    {
        public ScoreCanvas()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateHorizontalScrollBar();
            UpdateScoreScrollBar();
            UpdateBackgrounds_Score();
            UpdatePositionLines_Score();
            UpdatePositionLines_Graph();
            UpdateBackgrounds_Graph();
        }

        public void UpdateHorizontalScrollBar()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                double contentWidth = ScorePanel.ActualWidth;
                double viewportWidth = ScoreViewport.ActualWidth;

                double scrollableWidth = contentWidth - viewportWidth;
                HorizontalScrollBar.Maximum = Math.Max(0, scrollableWidth);
                HorizontalScrollBar.ViewportSize = viewportWidth;
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void UpdateScoreScrollBar()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                double contentHeight = ScorePanel.ActualHeight;
                double viewportHeight = ScoreViewport.ActualHeight;

                double scrollableHeight = contentHeight - viewportHeight;
                ScoreScrollBar.Maximum = Math.Max(0, scrollableHeight);
                ScoreScrollBar.ViewportSize = viewportHeight;
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void UpdateBackgrounds_Score()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                double viewportWidth = ScoreViewport.ActualWidth;
                double viewportHeight = ScoreViewport.ActualHeight;
                ScaleBackground.Width = viewportWidth;
                ScoreCanvasBackground.Width = viewportWidth;
                ScoreCanvasBackground.Height = viewportHeight;
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void UpdatePositionLines_Score()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                double viewportHeight = ScoreViewport.ActualHeight;
                PreviewPositionLine.Y2 = viewportHeight;
                PlaybackPositionLine_Score.Y2 = viewportHeight;
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void UpdateBackgrounds_Graph()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                double viewportWidth = GraphViewport.ActualWidth;
                double viewportHeight = GraphViewport.ActualHeight;
                GraphCanvasBackground.Width = viewportWidth;
                GraphCanvasBackground.Height = viewportHeight;
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public void UpdatePositionLines_Graph()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                double viewportHeight = GraphViewport.ActualHeight;
                PlaybackPositionLine_Graph.Y2 = viewportHeight;
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double offset = e.NewValue;
            Canvas.SetLeft(ScorePanel, -offset);
            Canvas.SetLeft(GraphGrid, -offset);
        }

        private void ScoreScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double offset = e.NewValue;
            Canvas.SetTop(ScorePanel, -offset);
        }

        private void ScoreViewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateHorizontalScrollBar();
            UpdateScoreScrollBar();
            UpdateBackgrounds_Score();
            UpdatePositionLines_Score();
        }

        // 中身を変更するメソッドの例
        public void SetContent(UIElement content)
        {
            ScorePanel.Children.Clear();
            ScorePanel.Children.Add(content);

            // 内容が変わったあと、サイズが変わるのでスクロール範囲更新
            UpdateHorizontalScrollBar();
        }

        private bool isRightButtonHeld_OnScore = false;

        private void ScoreCanvasBackground_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRightButtonHeld_OnScore = true;
            ScoreCanvasBackground.CaptureMouse();
            PreviewPositionLine.Visibility = Visibility.Visible;
            Point position = e.GetPosition(ScoreCanvasBackground);
            PreviewPositionLine.X1 = PreviewPositionLine.X2 = position.X;
            PreviewPositionLine.Y2 = ScoreViewport.ActualHeight;
        }

        private void ScoreCanvasBackground_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            isRightButtonHeld_OnScore = false;
            ScoreCanvasBackground.ReleaseMouseCapture();
            PreviewPositionLine.Visibility = Visibility.Hidden;
        }

        private void ScoreCanvasBackground_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightButtonHeld_OnScore)
            {
                Point position = e.GetPosition(ScoreCanvasBackground);
                PreviewPositionLine.X1 = PreviewPositionLine.X2 = position.X;
            }
        }

        private void ScoreCanvasBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(ScoreCanvasBackground);
            PlaybackPositionLine_Score.X1 = PlaybackPositionLine_Score.X2 = position.X;
            PlaybackPositionLine_Graph.X1 = PlaybackPositionLine_Graph.X2 = position.X;
        }

        private void GraphCanvasBackground_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(GraphCanvasBackground);
            PlaybackPositionLine_Score.X1 = PlaybackPositionLine_Score.X2 = position.X;
            PlaybackPositionLine_Graph.X1 = PlaybackPositionLine_Graph.X2 = position.X;
        }

        private void GraphViewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePositionLines_Graph();
            UpdateBackgrounds_Graph();
        }
    }
}
