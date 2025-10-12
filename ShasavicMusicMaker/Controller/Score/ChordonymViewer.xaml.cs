using ShasavicMusicMaker.Command;
using ShasavicMusicMaker.Command.Event.ChordonymChange;
using ShasavicMusicMaker.DimensionData;
using ShasavicMusicMaker.ScoreData.NoteData;
using SinShasavicSynthSF2.ShasavicObject.Event;
using SinShasavicSynthSF2.SynthEngineCore;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShasavicMusicMaker.Controller.Score
{
    /// <summary>
    /// ChordonymViewer.xaml の相互作用ロジック。
    /// </summary>
    public partial class ChordonymViewer : UserControl
    {
        private readonly List<(double, Arm)> pitchLinePoses;
        private readonly List<(double, ScoreLine)> scoreLinePoses;
        private CommandStucker? commandStucker;

        public FunctionSynth? FuncSynth { get; set; }
        public SF2VoiceManager? SF2VoiceManager { get; set; }

        /// <summary>
        /// コンストラクタの後にSetCommandStuckerも一緒に実行すること。
        /// </summary>
        public ChordonymViewer()
        {
            InitializeComponent();
            pitchLinePoses = [];
            scoreLinePoses = [];
            UpdateScoreLines();
            ContextMenu.PitchLineMuted += MuteArm;
            ContextMenu.PitchLineUnmuted += UnmuteArm;
            ContextMenu.PitchLineRemoved += RemoveArm;
            ContextMenu.ActionFinished += CloseContextMenuPopup;
        }

        internal void SetCommandStucker(CommandStucker commandStucker)
        {
            this.commandStucker = commandStucker;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetViewerToChordonym();
            UpdateChordonym();
            UpdateScoreLines();
        }

        private void SetViewerToChordonym()
        {
            if (DataContext is Chordonym chordonym)
                chordonym.Viewer = this;
        }

        public float OctaveHeight { get; private set; } = 100;

        public void ChangeOctaveHeight(float value)
        {
            OctaveHeight = value;
            UpdateChordonym();
            UpdateScoreLines();
        }

        public int ScoreLineDimension { get; private set; } = 0;

        public void SetScoreLineDimension(int value)
        {
            ScoreLineDimension = value;
            UpdateScoreLines();
        }

        private int selectorTop = 20;
        private int selectorBottom = -20;

        /// <summary>
        /// コードニムの縦方向の当たり判定を指定する。
        /// 原点は底音の音高線。
        /// 単位はピクセルで、上方向を正とする。
        /// </summary>
        /// <param name="top">上端</param>
        /// <param name="bottom">下端</param>
        public void SetSelectorVerticalEdge(int top, int bottom)
        {
            Canvas.SetBottom(PitchLineSelector, bottom);
            PitchLineSelector.Height = top - bottom;
            selectorTop = top;
            selectorBottom = bottom;
            UpdateScoreLines();
        }

        public void UpdateChordonym()
        {
            if (DataContext is Chordonym chordonym)
            {
                SimulateChordonym(chordonym, false);
            }
        }

        private void SimulateChordonym(Chordonym chordonym, bool isProv)
        {
            List<(Arm, BaseAndFormula)>[] dimLineVPositions = chordonym.CalcDimensionLineVPositions();
            List<List<(Arm, BaseAndFormula)>>[] dimLineHVPositions = new List<List<(Arm, BaseAndFormula)>>[DimensionInfo.MaxDimension];

            #region Sort Arm
            for (int dim = 0; dim < DimensionInfo.MaxDimension; dim++)
            {
                if (DimensionInfo.avoidDupl[dim])
                {
                    List<(Arm, BaseAndFormula)> dimLineVPosition = dimLineVPositions[dim];
                    List<List<(Arm, BaseAndFormula)>> dimLineHVPosition = [];
                    List<BaseAndFormula> exclusive = [];

                    foreach ((Arm, BaseAndFormula) aaf in dimLineVPosition)
                    {
                        bool flag = true;
                        float f = aaf.Item2.CalcCoef();

                        for (int i = 0; i < exclusive.Count; i++)
                        {
                            if (exclusive[i].Formula.SequenceEqual(aaf.Item2.Formula))
                            {
                                dimLineHVPosition[i].Add(aaf);
                                BaseAndFormula tmp = new(aaf.Item2.Base, aaf.Item2.Formula);
                                tmp.Formula[dim] += 1;
                                exclusive[i] = tmp;
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                        {
                            for (int i = 0; i < exclusive.Count; i++)
                            {
                                if (exclusive[i].CalcCoef() <= f)
                                {
                                    dimLineHVPosition[i].Add(aaf);
                                    BaseAndFormula tmp = new(aaf.Item2.Base, aaf.Item2.Formula);
                                    tmp.Formula[dim] += 1;
                                    exclusive[i] = tmp;
                                    flag = false;
                                    break;
                                }
                            }

                            if (flag)
                            {
                                dimLineHVPosition.Add([aaf]);
                                BaseAndFormula tmp = new(aaf.Item2.Base, aaf.Item2.Formula);
                                tmp.Formula[dim] += 1;
                                exclusive.Add(tmp);
                            }
                        }

                    }

                    dimLineHVPositions[dim] = dimLineHVPosition;
                }
                else
                {
                    dimLineHVPositions[dim] = [dimLineVPositions[dim]];
                }
            }
            #endregion

            ChordonymCanvas.Children.Clear();
            int lineThickness = (int)Math.Ceiling(0.08 * OctaveHeight);
            int distance = (int)Math.Ceiling(0.12 * OctaveHeight);
            int hOffset = distance;
            Dictionary<int, List<Arm>> armHEdgeDict = [];

            #region 2dLine
            int top_2d = (int)(MathF.Log2(DimensionInfo.Coefs[1]) * OctaveHeight);

            foreach (List<(Arm, BaseAndFormula)> linePoses in dimLineHVPositions[1])
            {
                List<Arm> armHPoses = [];
                armHEdgeDict.Add(hOffset, armHPoses);

                foreach ((Arm arm, BaseAndFormula baf) in linePoses)
                {
                    Rectangle dLine = new()
                    {
                        Width = lineThickness,
                        Height = top_2d,
                        Fill = DimensionInfo.Colors[1]
                    };
                    Canvas.SetLeft(dLine, hOffset);
                    Canvas.SetBottom(dLine, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
                    ChordonymCanvas.Children.Add(dLine);
                    armHPoses.Add(arm);
                }

                hOffset += lineThickness + distance;
            }
            #endregion

            int left1 = (int)(0.3 * OctaveHeight);
            int left2 = left1 + lineThickness;

            #region 6dLine
            int top_6d = (int)(MathF.Log2(DimensionInfo.Coefs[5]) * OctaveHeight);
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

            foreach (List<(Arm, BaseAndFormula)> linePoses in dimLineHVPositions[5])
            {
                List<Arm> armHPoses = [];
                armHEdgeDict.Add(hOffset + left1, armHPoses);

                foreach ((Arm arm, BaseAndFormula baf) in linePoses)
                {
                    Path dLine = new()
                    {
                        Data = geometry_6d,
                        Fill = DimensionInfo.Colors[5]
                    };
                    Canvas.SetLeft(dLine, hOffset);
                    Canvas.SetBottom(dLine, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
                    ChordonymCanvas.Children.Add(dLine);
                    armHPoses.Add(arm);
                }

                hOffset += left2 + distance;
            }
            #endregion

            int pitchLineLeftEdgeDef = hOffset;
            int left3 = (int)(0.6 * OctaveHeight);
            int left4 = left3 + lineThickness;
            List<Arm> armHPoses_4d_5d = [];
            armHEdgeDict.Add(hOffset, armHPoses_4d_5d);

            #region 4dLine
            int top_4d = (int)(MathF.Log2(DimensionInfo.Coefs[3]) * OctaveHeight);
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

            foreach ((Arm arm, BaseAndFormula baf) in dimLineHVPositions[3][0])
            {
                Path dLine = new()
                {
                    Data = geometry_4d,
                    Fill = DimensionInfo.Colors[3]
                };
                Canvas.SetLeft(dLine, hOffset);
                Canvas.SetBottom(dLine, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
                ChordonymCanvas.Children.Add(dLine);
                armHPoses_4d_5d.Add(arm);
            }
            #endregion

            #region 5dLine
            int top_5d = (int)(MathF.Log2(DimensionInfo.Coefs[4]) * OctaveHeight);
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

            foreach ((Arm arm, BaseAndFormula baf) in dimLineHVPositions[4][0])
            {
                Path dLine = new()
                {
                    Data = geometry_5d,
                    Fill = DimensionInfo.Colors[4]
                };
                Canvas.SetLeft(dLine, hOffset);
                Canvas.SetBottom(dLine, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
                ChordonymCanvas.Children.Add(dLine);
                armHPoses_4d_5d.Add(arm);
            }
            #endregion

            int pitchLineRightEdgeDef = hOffset + left4;
            hOffset = pitchLineRightEdgeDef + distance;

            #region 3dLine
            int top_3d = (int)(MathF.Log2(DimensionInfo.Coefs[2]) * OctaveHeight);

            foreach (List<(Arm, BaseAndFormula)> linePoses in dimLineHVPositions[2])
            {
                List<Arm> armHPoses = [];
                armHEdgeDict.Add(hOffset + lineThickness, armHPoses);

                foreach ((Arm arm, BaseAndFormula baf) in linePoses)
                {
                    Rectangle dLine = new()
                    {
                        Width = lineThickness,
                        Height = top_3d,
                        Fill = DimensionInfo.Colors[2]
                    };
                    Canvas.SetLeft(dLine, hOffset);
                    Canvas.SetBottom(dLine, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
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
            foreach (List<(Arm, BaseAndFormula)> linePoses in dimLineHVPositions[0])
            {
                List<Arm> armHPoses = [];
                armHEdgeDict.Add(hOffset + left5 * 2 + distance, armHPoses);

                foreach ((Arm arm, BaseAndFormula baf) in linePoses)
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
                        Canvas.SetBottom(dLine1, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight) + left5);
                        dLine2.Data = geometry_1d_up;
                        Canvas.SetBottom(dLine2, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
                    }
                    else
                    {
                        Canvas.SetBottom(dLine1, (int)(MathF.Log2(baf.CalcCoef()) * OctaveHeight));
                        dLine2.Data = geometry_1d_down;
                        Canvas.SetTop(dLine2, -(int)((MathF.Log2(baf.CalcCoef()) + 1f) * OctaveHeight));
                    }

                    ChordonymCanvas.Children.Add(dLine1);
                    ChordonymCanvas.Children.Add(dLine2);
                    armHPoses.Add(arm);
                }

                hOffset += left5 * 2 + distance;
            }
            #endregion

            #region draw pitch line
            if (!isProv)
            {
                pitchLinePoses.Clear();
                pitchLinePoses.Add((0, chordonym.Arm));
            }

            if (chordonym.Arm.Arms.Count > 0)
            {
                int chordonymRightEgde = pitchLineRightEdgeDef;

                List<Arm> arms1 = [.. chordonym.Arm.Arms];
                List<Arm> arms2 = [];
                Dictionary<Arm, double> armVPosDict = [];

                foreach (List<(Arm, BaseAndFormula)> list in dimLineVPositions)
                {
                    foreach ((Arm arm, BaseAndFormula bas) in list)
                    {
                        float coef2 = bas.CalcCoef();

                        if (arm.Bcp.Scending)
                            coef2 *= DimensionInfo.Coefs[arm.Bcp.Dimension - 1];

                        armVPosDict.Add(arm, MathF.Log2(coef2) * OctaveHeight);
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

                        int vPos = -(int)armVPosDict[arm1];

                        Line line = new()
                        {
                            X1 = leftEdge,
                            X2 = rightEdge,
                            Y1 = vPos,
                            Y2 = vPos,
                            Stroke = new SolidColorBrush(Colors.White),
                            StrokeThickness = (int)Math.Ceiling(0.04 * OctaveHeight),
                        };
                        ChordonymCanvas.Children.Add(line);

                        if (!isProv)
                            pitchLinePoses.Add((-armVPosDict[arm1], arm1));
                    }

                    arms1 = arms2;
                    arms2 = [];
                }

                TonicLine.X2 = chordonymRightEgde;
                SelectedPitchLine.X2 = chordonymRightEgde + 2;
            }
            else
            {
                TonicLine.X2 = pitchLineRightEdgeDef;
                SelectedPitchLine.X2 = pitchLineRightEdgeDef + 2;
            }

            TonicLine.StrokeThickness = (int)Math.Ceiling(0.04 * OctaveHeight);
            SelectedPitchLine.StrokeThickness = (int)Math.Ceiling(0.04 * OctaveHeight) + 2;

            if (!isProv)
                pitchLinePoses.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            PitchLineSelector.Width = TonicLine.X2;
            #endregion
        }

        public void UpdateScoreLines()
        {
            Dictionary<int[], List<(int, Arm)>> formulaDict = [];

            foreach ((double, Arm) tpl in pitchLinePoses)
            {
                int[] formula1 = BaseAndFormula.CalcBaseAndFomulaOfArm(tpl.Item2).Formula;
                int lineLevel = formula1[ScoreLineDimension];
                int[] formula2 = [.. formula1];
                formula2[ScoreLineDimension] = 0;

                if (formulaDict.Keys.FirstOrDefault(formula => formula2.SequenceEqual(formula))
                    is int[] key)
                {
                    formulaDict[key].Add((lineLevel, tpl.Item2));
                }
                else
                {
                    formulaDict.Add(formula2, [(lineLevel, tpl.Item2)]);
                }
            }

            scoreLinePoses.Clear();

            foreach (List<(int, Arm)> value in formulaDict.Values)
            {
                value.Sort((a,b) => a.Item1.CompareTo(b.Item1));
                int minLevel = value[0].Item1;
                int maxLevel = value[0].Item1;

                if ((int)(MathF.Log2(value[0].Item2.CalcCoefFromBase()) * OctaveHeight) > selectorBottom)
                {
                    while ((int)(MathF.Log2(
                        new ScoreLine(value[0].Item2, ScoreLineDimension, minLevel - 1
                        ).CalcCoefFromBase()) * OctaveHeight)
                        > selectorBottom)
                    {
                        minLevel--;
                    }
                }
                else
                {
                    while ((int)(MathF.Log2(
                        new ScoreLine(value[0].Item2, ScoreLineDimension, minLevel
                        ).CalcCoefFromBase()) * OctaveHeight)
                        < selectorBottom)
                    {
                        minLevel++;
                    }
                }

                if ((int)(MathF.Log2(value[0].Item2.CalcCoefFromBase()) * OctaveHeight) > selectorTop)
                {
                    while ((int)(MathF.Log2(
                        new ScoreLine(value[0].Item2, ScoreLineDimension, maxLevel
                        ).CalcCoefFromBase()) * OctaveHeight)
                        > selectorTop)
                    {
                        maxLevel--;
                    }
                }
                else
                {
                    while ((int)(MathF.Log2(
                        new ScoreLine(value[0].Item2, ScoreLineDimension, maxLevel + 1
                        ).CalcCoefFromBase()) * OctaveHeight)
                        < selectorTop)
                    {
                        maxLevel++;
                    }
                }

                if (minLevel < maxLevel)
                {
                    int level = minLevel;

                    for (int i = 0; i < value.Count - 1; i++)
                    {
                        float border = (value[i].Item1 + value[i + 1].Item1) / 2f;

                        while (level < border)
                        {
                            if (value[i].Item1 != level)
                            {
                                ScoreLine scrLine = new(value[i].Item2, ScoreLineDimension, level - value[i].Item1);
                                double vPos = MathF.Log2(scrLine.CalcCoefFromBase()) * OctaveHeight;
                                scoreLinePoses.Add((-vPos, scrLine));
                            }

                            level++;
                        }
                    }

                    while (level <= maxLevel)
                    {
                        if (value[^1].Item1 != level)
                        {
                            ScoreLine scrLine = new(value[^1].Item2, ScoreLineDimension, level - value[^1].Item1);
                            double vPos = MathF.Log2(scrLine.CalcCoefFromBase()) * OctaveHeight;
                            scoreLinePoses.Add((-vPos, scrLine));
                        }

                        level++;
                    }
                }
            }

            scoreLinePoses.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            ScoreLineCanvas.Children.Clear();
            Brush brush = DimensionInfo.Colors[ScoreLineDimension];

            foreach ((double, ScoreLine) tpl in scoreLinePoses)
            {
                Line line = new()
                {
                    X1 = 0,
                    X2 = PitchLineSelector.Width,
                    Y1 = (int)tpl.Item1,
                    Y2 = (int)tpl.Item1,
                    Stroke = brush,
                    Opacity = 0.5,
                    StrokeThickness = (int)Math.Ceiling(0.04 * OctaveHeight),
                };
                ScoreLineCanvas.Children.Add(line);
            }
        }

        private object? provSoundObj; 
        private object? selectedObj;
        private Point? rightButtonStartPint;

        private void PitchSoundUpdate(object sender, MouseEventArgs e)
        {
            if (DataContext is Chordonym chordonym)
            {
                Point pos = e.GetPosition(ChordonymCanvas);
                Arm arm = pitchLinePoses[0].Item2;
                double vPos_Arm = 0;
                double minDis_Arm = double.MaxValue;

                foreach ((double, Arm) linePos in pitchLinePoses)
                {
                    double dis = Math.Abs(linePos.Item1 - pos.Y);

                    if (dis <= minDis_Arm)
                    {
                        arm = linePos.Item2;
                        vPos_Arm = linePos.Item1;
                        minDis_Arm = dis;
                    }
                    else
                    {
                        break;
                    }
                }

                double vPos = vPos_Arm;
                BaseAndFormula bas = BaseAndFormula.CalcBaseAndFomulaOfArm(arm);
                selectedObj = arm;

                if (scoreLinePoses.Count > 0)
                {
                    ScoreLine scoreLine = scoreLinePoses[0].Item2;
                    double vPos_ScrLine = 0;
                    double minDis_ScrLine = double.MaxValue;

                    foreach ((double, ScoreLine) linePos in scoreLinePoses)
                    {
                        double dis = Math.Abs(linePos.Item1 - pos.Y);

                        if (dis <= minDis_ScrLine)
                        {
                            scoreLine = linePos.Item2;
                            vPos_ScrLine = linePos.Item1;
                            minDis_ScrLine = dis;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (minDis_ScrLine < minDis_Arm)
                    {
                        vPos = vPos_ScrLine;
                        bas = BaseAndFormula.CalcBaseAndFomulaOfScoreLine(scoreLine);
                        selectedObj = scoreLine;
                    }
                }


                SelectedPitchLine.Visibility = Visibility.Visible;

                if (Mouse.RightButton == MouseButtonState.Pressed || Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (provSoundObj != selectedObj)
                    {
                        if (selectedObj is ScoreLine sl1)
                        {
                            #region Preview Chordonym
                            BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(sl1.Body);
                            Chordonym chordonym2 = chordonym.Clone();
                            Dictionary<BaseAndFormula, Arm> dict =
                                BaseAndFormula.GetArmsDictOfChordonym(chordonym2.Arm);

                            if (dict.FirstOrDefault(kv => kv.Key.Formula.SequenceEqual(baf.Formula)).Value is Arm arm2)
                            {
                                ScoreLine sl2 = new(arm2, sl1.Dimension, sl1.Sceding);
                                sl2.AddToCordonym();
                                SimulateChordonym(chordonym2, true);
                            }
                            #endregion
                        }

                        SelectedPitchLine.Y1 = SelectedPitchLine.Y2 = vPos;

                        if (SF2VoiceManager is SF2VoiceManager manager2 && manager2.AnySF2sSeted)
                        {
                            SF2VoiceManager.AllNoteOff();
                            NoteOnArg arg = new(0, chordonym.OrgnBaseFreq, bas.Formula, 100);
                            SF2VoiceManager.NoteOn(0, [arg]);
                        } 
                        else if (FuncSynth is not null)
                        {
                            FuncSynth.AllNoteOff();
                            FuncSynth.NoteOn(0, chordonym.OrgnBaseFreq, bas.Formula, 100);
                        }

                        provSoundObj = selectedObj;
                        SelectedPitchLine.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (provSoundObj != null)
                    {
                        SF2VoiceManager?.AllNoteOff();
                        FuncSynth?.AllNoteOff();
                        provSoundObj = null;
                        UpdateChordonym();
                    }

                    SelectedPitchLine.Visibility = Visibility.Hidden;
                }
            }
        }

        private void PitchLineSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed || Mouse.LeftButton == MouseButtonState.Pressed)
            {
                SF2VoiceManager?.AllNoteOff();
                FuncSynth?.AllNoteOff();
                provSoundObj = null;
                selectedObj = null;
                UpdateChordonym();
            }

            SelectedPitchLine.Visibility = Visibility.Hidden;
        }

        private void PitchLineSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PitchSoundUpdate(sender, e);
            if (selectedObj is ScoreLine sl1 && DataContext is Chordonym chordonym && commandStucker is not null)
            {
                BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(sl1.Body);
                Arm arm1 = chordonym.Arm.Clone();
                Dictionary<BaseAndFormula, Arm> dict =
                    BaseAndFormula.GetArmsDictOfChordonym(arm1);

                if (dict.FirstOrDefault(kv => kv.Key.Formula.SequenceEqual(baf.Formula)).Value is Arm arm2)
                {
                    ScoreLine sl2 = new(arm2, sl1.Dimension, sl1.Sceding);
                    sl2.AddToCordonym();
                    ChangeArmOfChordonymCommand command = new(chordonym, chordonym.Arm, arm1);
                    commandStucker.SubscribeCommand(command);
                }
            }
        }

        private DateTime _lastRightClickTime;
        private const int DoubleClickThreshold = 300;
        private Arm? _actionArm;

        private void PitchLineSelector_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (rightButtonStartPint is Point p1 && provSoundObj is Arm arm)
            {
                Point end = e.GetPosition(ChordonymCanvas);
                double dx = end.X - p1.X;
                double dy = end.Y - p1.Y;

                if (Math.Sqrt(dx * dx + dy * dy) < 5)
                {
                    DateTime now = DateTime.Now;
                    double diff = (now - _lastRightClickTime).TotalMilliseconds;
                    _lastRightClickTime = now;

                    if (diff < DoubleClickThreshold)
                    {
                        SelectedPitchLine.Visibility = Visibility.Hidden;

                        Point p2 = e.GetPosition(RootGrid);
                        ContextMenuPopup.HorizontalOffset = p2.X + 10;
                        ContextMenuPopup.VerticalOffset = p2.Y + 10;
                        ActionPitchLine.X1 = SelectedPitchLine.X1;
                        ActionPitchLine.X2 = SelectedPitchLine.X2;
                        ActionPitchLine.Y1 = SelectedPitchLine.Y1;
                        ActionPitchLine.Y2 = SelectedPitchLine.Y2;
                        ActionPitchLine.StrokeThickness = SelectedPitchLine.StrokeThickness;
                        ActionPitchLine.Visibility = Visibility.Visible;

                        ContextMenuPopup.IsOpen = true;
                        _actionArm = arm;
                        ContextMenu.BootPitchLineEditor(arm);
                    }

                }
            }

            if (provSoundObj != null)
            {
                SF2VoiceManager?.AllNoteOff();
                FuncSynth?.AllNoteOff();
                provSoundObj = null;
                UpdateChordonym();
            }

            SelectedPitchLine.Visibility = Visibility.Hidden;
            rightButtonStartPint = null;
        }

        private void PitchLineSelector_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            rightButtonStartPint = e.GetPosition(ChordonymCanvas);
            PitchSoundUpdate(sender, e);
        }

        private void CloseContextMenuPopup(object sender, EventArgs e)
        {
            ContextMenuPopup.IsOpen = false;
        }

        private void ContextMenuPopup_Closed(object sender, EventArgs e)
        {
            ActionPitchLine.Visibility = Visibility.Hidden;
        }

        private void RemoveArm(object sender, EventArgs e)
        {
            if (_actionArm is Arm arm1 && DataContext is Chordonym chordonym && commandStucker is not null)
            {
                if (arm1.Body is not null)
                {
                    BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(arm1);
                    Arm arm2 = chordonym.Arm.Clone();
                    Dictionary<BaseAndFormula, Arm> dict =
                        BaseAndFormula.GetArmsDictOfChordonym(arm2);

                    if (dict.FirstOrDefault(kv => kv.Key.Formula.SequenceEqual(baf.Formula)).Value is Arm arm3 && arm3.Body is not null)
                    {
                        arm3.Body.Arms.Remove(arm3);
                        ChangeArmOfChordonymCommand command = new(chordonym, chordonym.Arm, arm2);
                        commandStucker.SubscribeCommand(command);
                    }
                }
            }
        }

        private void MuteArm(object sender, EventArgs e)
        {
            if (_actionArm is Arm arm1 && DataContext is Chordonym chordonym && commandStucker is not null)
            {
                BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(arm1);
                Arm arm2 = chordonym.Arm.Clone();
                Dictionary<BaseAndFormula, Arm> dict =
                    BaseAndFormula.GetArmsDictOfChordonym(arm2);

                if (dict.FirstOrDefault(kv => kv.Key.Formula.SequenceEqual(baf.Formula)).Value is Arm arm3)
                {
                    arm3.Muted = true;
                    ChangeArmOfChordonymCommand command = new(chordonym, chordonym.Arm, arm2);
                    commandStucker.SubscribeCommand(command);
                }
            }
        }

        private void UnmuteArm(object sender, EventArgs e)
        {
            if (_actionArm is Arm arm1 && DataContext is Chordonym chordonym && commandStucker is not null)
            {
                BaseAndFormula baf = BaseAndFormula.CalcBaseAndFomulaOfArm(arm1);
                Arm arm2 = chordonym.Arm.Clone();
                Dictionary<BaseAndFormula, Arm> dict =
                    BaseAndFormula.GetArmsDictOfChordonym(arm2);

                if (dict.FirstOrDefault(kv => kv.Key.Formula.SequenceEqual(baf.Formula)).Value is Arm arm3)
                {
                    arm3.Muted = false;
                    ChangeArmOfChordonymCommand command = new(chordonym, chordonym.Arm, arm2);
                    commandStucker.SubscribeCommand(command);
                }
            }
        }
    }
}
