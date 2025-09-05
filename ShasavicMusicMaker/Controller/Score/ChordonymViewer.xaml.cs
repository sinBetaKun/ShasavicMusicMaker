using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShasavicMusicMaker.DimensionData;
using ShasavicMusicMaker.ScoreData.NoteData;

namespace ShasavicMusicMaker.Controller.Score
{
    /// <summary>
    /// ChordonymViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class ChordonymViewer : UserControl
    {
        public ChordonymViewer()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateChordonym();
        }

        private float OctaveHeight => 100;

        public void UpdateChordonym()
        {
            if (DataContext is Chordonym chordonym)
            {
                List<(Arm, Fraction)>[] dimLineVPositions = chordonym.CalcDimensionLineVPositions();
                List<List<(Arm, Fraction)>>[] dimLineHVPositions = new List<List<(Arm, Fraction)>>[DimensionInfo.MaxDimension];

                for (int dim = 0; dim < DimensionInfo.MaxDimension; dim++)
                {
                    if (DimensionInfo.avoidDupl[dim])
                    {
                        List<(Arm, Fraction)> dimLineVPosition = dimLineVPositions[dim];
                        List<List<(Arm, Fraction)>> dimLineHVPosition = [];
                        List<float> exclusive = [];

                        foreach ((Arm, Fraction) aac in dimLineVPosition)
                        {
                            bool flag = true;
                            float f = aac.Item2.ToFloat();

                            for (int i = 0; i < exclusive.Count; i++)
                            {
                                if (exclusive[i] <= f)
                                {
                                    dimLineHVPosition[i].Add(aac);
                                    exclusive[i] = (aac.Item2 * DimensionInfo.Coefs[dim]).ToFloat();
                                    flag = false;
                                    break;
                                }
                            }

                            if (flag)
                            {
                                dimLineHVPosition.Add([aac]);
                                exclusive.Add((aac.Item2 * DimensionInfo.Coefs[dim]).ToFloat());
                            }
                        }

                        dimLineHVPositions[dim] = dimLineHVPosition;
                    }
                    else
                    {
                        dimLineHVPositions[dim] = [dimLineVPositions[dim]];
                    }
                }

                ChordonymCanvas.Children.Clear();
                const int lineThickness = 8;
                const int distance = 12;
                int hOffset = 0;
                Dictionary<int, List<Arm>> armHEdgeDict = [];

                int left1 = (int)(0.3 * OctaveHeight);
                int left2 = left1 + lineThickness;

                #region 6dLine
                int top_6d = (int)(MathF.Log2(DimensionInfo.Coefs[5].ToFloat()) * OctaveHeight);
                PathFigure figure_6d = new()
                {
                    StartPoint = new(left1, 0),
                    IsClosed = true
                };
                figure_6d.Segments.Add(new QuadraticBezierSegment(
                    new(0, top_6d / 2),
                    new(left1, top_6d),
                    true));
                figure_6d.Segments.Add(new LineSegment(
                    new(left2, top_6d),
                    true));
                figure_6d.Segments.Add(new QuadraticBezierSegment(
                    new(lineThickness, top_6d / 2),
                    new(left2, 0), true
                    ));
                PathGeometry geometry_6d = new();
                geometry_6d.Figures.Add(figure_6d);

                foreach (List<(Arm, Fraction)> linePoses in dimLineHVPositions[5])
                {
                    List<Arm> armHPoses = [];
                    armHEdgeDict.Add(hOffset + left1, armHPoses);

                    foreach ((Arm arm, Fraction coef) in linePoses)
                    {
                        Path dLine = new()
                        {
                            Data = geometry_6d,
                            Fill = DimensionInfo.Colors[5]
                        };
                        Canvas.SetLeft(dLine, hOffset);
                        Canvas.SetBottom(dLine, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                        ChordonymCanvas.Children.Add(dLine);
                        armHPoses.Add(arm);
                    }

                    hOffset += left2 + distance;
                }
                #endregion

                #region 2dLine
                int top_2d = (int)(MathF.Log2(DimensionInfo.Coefs[1].ToFloat()) * OctaveHeight);

                foreach (List<(Arm, Fraction)> linePoses in dimLineHVPositions[1])
                {
                    List<Arm> armHPoses = [];
                    armHEdgeDict.Add(hOffset, armHPoses);

                    foreach ((Arm arm, Fraction coef) in linePoses)
                    {
                        Rectangle dLine = new()
                        {
                            Width = lineThickness,
                            Height = top_2d,
                            Fill = DimensionInfo.Colors[1]
                        };
                        Canvas.SetLeft(dLine, hOffset);
                        Canvas.SetBottom(dLine, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                        ChordonymCanvas.Children.Add(dLine);
                        armHPoses.Add(arm);
                    }

                    hOffset += lineThickness + distance;
                }
                #endregion

                int pitchLineLeftEdgeDef = hOffset;
                int left3 = (int)(0.6 * OctaveHeight);
                int left4 = left3 + lineThickness;
                List<Arm> armHPoses_4d_5d = [];
                armHEdgeDict.Add(hOffset, armHPoses_4d_5d);

                #region 4dLine
                int top_4d = (int)(MathF.Log2(DimensionInfo.Coefs[3].ToFloat()) * OctaveHeight);
                PathFigure figure_4d = new()
                {
                    StartPoint = new(0, 0),
                    IsClosed = true
                };
                figure_4d.Segments.Add(new LineSegment(
                    new(left3, -top_4d),
                    true));
                figure_4d.Segments.Add(new LineSegment(
                    new(left4, -top_4d),
                    true));
                figure_4d.Segments.Add(new LineSegment(
                    new(lineThickness, 0),
                    true));
                PathGeometry geometry_4d = new();
                geometry_4d.Figures.Add(figure_4d);

                foreach ((Arm arm, Fraction coef) in dimLineHVPositions[3][0])
                {
                    Path dLine = new()
                    {
                        Data = geometry_4d,
                        Fill = DimensionInfo.Colors[3]
                    };
                    Canvas.SetLeft(dLine, hOffset);
                    Canvas.SetBottom(dLine, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                    ChordonymCanvas.Children.Add(dLine);
                    armHPoses_4d_5d.Add(arm);
                }
                #endregion

                #region 5dLine
                int top_5d = (int)(MathF.Log2(DimensionInfo.Coefs[4].ToFloat()) * OctaveHeight);
                PathFigure figure_5d = new()
                {
                    StartPoint = new(left3, 0),
                    IsClosed = true
                };
                figure_5d.Segments.Add(new LineSegment(
                    new(0, -top_5d),
                    true));
                figure_5d.Segments.Add(new LineSegment(
                    new(lineThickness, -top_5d),
                    true));
                figure_5d.Segments.Add(new LineSegment(
                    new(left4, 0),
                    true));
                PathGeometry geometry_5d = new();
                geometry_5d.Figures.Add(figure_5d);

                foreach ((Arm arm, Fraction coef) in dimLineHVPositions[4][0])
                {
                    Path dLine = new()
                    {
                        Data = geometry_5d,
                        Fill = DimensionInfo.Colors[4]
                    };
                    Canvas.SetLeft(dLine, hOffset);
                    Canvas.SetBottom(dLine, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                    ChordonymCanvas.Children.Add(dLine);
                    armHPoses_4d_5d.Add(arm);
                }
                #endregion

                int pitchLineRightEdgeDef = hOffset + left4;
                hOffset = pitchLineRightEdgeDef + distance;

                #region 3dLine
                int top_3d = (int)(MathF.Log2(DimensionInfo.Coefs[2].ToFloat()) * OctaveHeight);

                foreach (List<(Arm, Fraction)> linePoses in dimLineHVPositions[2])
                {
                    List<Arm> armHPoses = [];
                    armHEdgeDict.Add(hOffset + lineThickness, armHPoses);

                    foreach ((Arm arm, Fraction coef) in linePoses)
                    {
                        Rectangle dLine = new()
                        {
                            Width = lineThickness,
                            Height = top_3d,
                            Fill = DimensionInfo.Colors[2]
                        };
                        Canvas.SetLeft(dLine, hOffset);
                        Canvas.SetBottom(dLine, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                        ChordonymCanvas.Children.Add(dLine);
                        armHPoses.Add(arm);
                    }

                    hOffset += lineThickness + distance;
                }
                #endregion

                #region 1dLine
                int top_1d = (int)OctaveHeight;
                int left5 = (int)(0.1f * OctaveHeight);
                int thickness_1d = (int)(0.5f * lineThickness);
                PathFigure figure_1d_up = new()
                {
                    StartPoint = new(0, 0)
                };
                figure_1d_up.Segments.Add(new LineSegment(
                    new(left5, -left5),
                    true));
                figure_1d_up.Segments.Add(new LineSegment(
                    new(left5 * 2, 0),
                    true));
                PathGeometry geometry_1d_up = new();
                geometry_1d_up.Figures.Add(figure_1d_up);
                PathFigure figure_1d_down = new()
                {
                    StartPoint = new(0, 0)
                };
                figure_1d_down.Segments.Add(new LineSegment(
                    new(left5, left5),
                    true));
                figure_1d_down.Segments.Add(new LineSegment(
                    new(left5 * 2, 0),
                    true));
                PathGeometry geometry_1d_down = new();
                geometry_1d_down.Figures.Add(figure_1d_down);
                foreach (List<(Arm, Fraction)> linePoses in dimLineHVPositions[0])
                {
                    List<Arm> armHPoses = [];
                    armHEdgeDict.Add(hOffset + left5 * 2 + distance, armHPoses);

                    foreach ((Arm arm, Fraction coef) in linePoses)
                    {
                        Rectangle dLine1 = new()
                        {
                            Width = thickness_1d,
                            Height = top_1d - left5,
                            Fill = DimensionInfo.Colors[0]
                        };
                        Canvas.SetLeft(dLine1, hOffset + left5 - thickness_1d / 2);
                        Path dLine2 = new()
                        {
                            Stroke = new SolidColorBrush(Colors.White),
                            StrokeThickness = thickness_1d,
                        };
                        Canvas.SetLeft(dLine2, hOffset);

                        if (arm.Bcp.Scending)
                        {
                            Canvas.SetBottom(dLine1, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight) + left5);
                            dLine2.Data = geometry_1d_up;
                            Canvas.SetBottom(dLine2, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                        }
                        else
                        {
                            Canvas.SetBottom(dLine1, (int)(MathF.Log2(coef.ToFloat()) * OctaveHeight));
                            dLine2.Data = geometry_1d_down;
                            Canvas.SetTop(dLine2, -(int)((MathF.Log2(coef.ToFloat()) + 1f) * OctaveHeight));
                        }

                        ChordonymCanvas.Children.Add(dLine1);
                        ChordonymCanvas.Children.Add(dLine2);
                        armHPoses.Add(arm);
                    }

                    hOffset += left5 * 2 + distance;
                }
                #endregion

                #region draw pitch line
                if (chordonym.Arm.Arms.Count > 0)
                {
                    int chordonymRightEgde = pitchLineRightEdgeDef;

                    List<Arm> arms1 = [.. chordonym.Arm.Arms];
                    List<Arm> arms2 = [];
                    Dictionary<Arm, int> armVPosDict = [];

                    foreach (List<(Arm, Fraction)> list in dimLineVPositions)
                    {
                        foreach ((Arm arm, Fraction coef) in list)
                        {
                            Fraction coef2 = arm.Bcp.Scending
                                ? coef * DimensionInfo.Coefs[arm.Bcp.Dimension - 1]
                                : coef;
                            armVPosDict.Add(arm, (int)(MathF.Log2(coef2.ToFloat()) * OctaveHeight));
                        }
                    }

                    while (arms1.Count > 0)
                    {
                        foreach (Arm arm1 in arms1)
                        {
                            int leftEdge = pitchLineLeftEdgeDef;
                            int rightEdge = pitchLineRightEdgeDef;
                            int bodyHEdge = armHEdgeDict.First(kv => kv.Value.Contains(arm1)).Key;

                            if (bodyHEdge < leftEdge)
                                leftEdge = bodyHEdge;
                            else if (bodyHEdge > rightEdge)
                                rightEdge = bodyHEdge;

                            foreach (Arm arm2 in arm1.Arms)
                            {
                                int hEdge = armHEdgeDict.First(kv => kv.Value.Contains(arm2)).Key;

                                if (hEdge < leftEdge)
                                    leftEdge = hEdge;
                                else if (hEdge > rightEdge)
                                    rightEdge = hEdge;

                                arms2.Add(arm2);
                            }

                            if (rightEdge > chordonymRightEgde)
                                chordonymRightEgde = rightEdge;

                            int vPos = armVPosDict[arm1];

                            Line line = new()
                            {
                                X1 = 0,
                                X2 = rightEdge - leftEdge,
                                Y1 = -vPos,
                                Y2 = -vPos,
                                Stroke = new SolidColorBrush(Colors.White),
                                StrokeThickness = 4,
                            };
                            Canvas.SetLeft(line, leftEdge);
                            ChordonymCanvas.Children.Add(line);
                        }

                        arms1 = arms2;
                        arms2 = [];
                    }

                    TonicLine.X2 = chordonymRightEgde;
                }
                else
                {
                    TonicLine.X2 = pitchLineRightEdgeDef;
                }
                #endregion
            }
        }
    }
}
