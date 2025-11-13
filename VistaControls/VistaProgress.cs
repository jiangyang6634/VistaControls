using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VistaControls
{
    /// <summary>
    /// VistaProgress - 进度条控件
    /// </summary>
    public class VistaProgress : Control
    {
        private Path? _progressBackground;
        private Path? _progressForeground;
        private Canvas? _progressCanvas;
        private ArcSegment? _backgroundArc;
        private ArcSegment? _foregroundArc;
        private PathFigure? _backgroundFigure;
        private PathFigure? _foregroundFigure;
        private TextBlock? _circleText;
        private TextBlock? _circleTextRight;

        static VistaProgress()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaProgress),
                new FrameworkPropertyMetadata(typeof(VistaProgress)));
        }

        public VistaProgress()
        {
            Loaded += VistaProgress_Loaded;
            SizeChanged += VistaProgress_SizeChanged;
            // 初始化默认颜色
            UpdateComputedColor();
        }

        private void VistaProgress_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateProgress();
        }

        private void VistaProgress_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateProgress();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            _progressCanvas = GetTemplateChild("progressCanvas") as Canvas;
            _progressBackground = GetTemplateChild("progressBackground") as Path;
            _progressForeground = GetTemplateChild("progressForeground") as Path;
            _circleText = GetTemplateChild("circleText") as TextBlock;
            _circleTextRight = GetTemplateChild("circleTextRight") as TextBlock;
            
            if (_progressBackground?.Data is PathGeometry bgGeometry && bgGeometry.Figures.Count > 0)
            {
                _backgroundFigure = bgGeometry.Figures[0] as PathFigure;
                if (_backgroundFigure?.Segments.Count > 0)
                {
                    _backgroundArc = _backgroundFigure.Segments[0] as ArcSegment;
                }
            }
            
            if (_progressForeground?.Data is PathGeometry fgGeometry && fgGeometry.Figures.Count > 0)
            {
                _foregroundFigure = fgGeometry.Figures[0] as PathFigure;
                if (_foregroundFigure?.Segments.Count > 0)
                {
                    _foregroundArc = _foregroundFigure.Segments[0] as ArcSegment;
                }
            }
            
            UpdateProgress();
        }

        #region 依赖属性

        /// <summary>
        /// 百分比（必填，0-100）
        /// </summary>
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(nameof(Percentage), typeof(double), typeof(VistaProgress),
                new FrameworkPropertyMetadata(0.0, OnPercentageChanged, CoercePercentage));

        /// <summary>
        /// 进度条类型：line, circle, dashboard
        /// </summary>
        public static readonly DependencyProperty ProgressTypeProperty =
            DependencyProperty.Register(nameof(ProgressType), typeof(ProgressType), typeof(VistaProgress),
                new PropertyMetadata(ProgressType.Line, OnProgressTypeChanged));

        /// <summary>
        /// 进度条的宽度（单位 px）
        /// </summary>
        public static readonly DependencyProperty StrokeWidthProperty =
            DependencyProperty.Register(nameof(StrokeWidth), typeof(double), typeof(VistaProgress),
                new PropertyMetadata(6.0));

        /// <summary>
        /// 进度条显示文字内置在进度条内（只在 type=line 时可用）
        /// </summary>
        public static readonly DependencyProperty TextInsideProperty =
            DependencyProperty.Register(nameof(TextInside), typeof(bool), typeof(VistaProgress),
                new PropertyMetadata(false));

        /// <summary>
        /// 进度条当前状态：success, exception, warning
        /// </summary>
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(ProgressStatus), typeof(VistaProgress),
                new PropertyMetadata(ProgressStatus.None, OnStatusChanged));

        /// <summary>
        /// 进度条背景色（会覆盖 status 状态颜色）
        /// 可以是字符串、函数或数组
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(object), typeof(VistaProgress),
                new PropertyMetadata(null, OnColorChanged));

        /// <summary>
        /// 环形进度条画布宽度（只在 type 为 circle 或 dashboard 时可用）
        /// </summary>
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(VistaProgress),
                new PropertyMetadata(126.0));

        /// <summary>
        /// 是否显示进度条文字内容
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register(nameof(ShowText), typeof(bool), typeof(VistaProgress),
                new PropertyMetadata(true));

        /// <summary>
        /// circle/dashboard 类型路径两端的形状：butt, round, square
        /// </summary>
        public static readonly DependencyProperty StrokeLinecapProperty =
            DependencyProperty.Register(nameof(StrokeLinecap), typeof(PenLineCap), typeof(VistaProgress),
                new PropertyMetadata(PenLineCap.Round));

        /// <summary>
        /// 指定进度条文字内容的格式化函数
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(Func<double, string>), typeof(VistaProgress),
                new PropertyMetadata(null, OnFormatChanged));

        /// <summary>
        /// 指定进度条底色（支持 hex 格式）
        /// </summary>
        public static readonly DependencyProperty DefineBackColorProperty =
            DependencyProperty.Register(nameof(DefineBackColor), typeof(Brush), typeof(VistaProgress),
                new PropertyMetadata(null));

        /// <summary>
        /// 指定进度条字体颜色（支持 hex 格式）
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(nameof(TextColor), typeof(Brush), typeof(VistaProgress),
                new PropertyMetadata(null));

        #endregion

        #region 属性

        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        public ProgressType ProgressType
        {
            get => (ProgressType)GetValue(ProgressTypeProperty);
            set => SetValue(ProgressTypeProperty, value);
        }

        public double StrokeWidth
        {
            get => (double)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public bool TextInside
        {
            get => (bool)GetValue(TextInsideProperty);
            set => SetValue(TextInsideProperty, value);
        }

        public ProgressStatus Status
        {
            get => (ProgressStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        public object? Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public bool ShowText
        {
            get => (bool)GetValue(ShowTextProperty);
            set => SetValue(ShowTextProperty, value);
        }

        public PenLineCap StrokeLinecap
        {
            get => (PenLineCap)GetValue(StrokeLinecapProperty);
            set => SetValue(StrokeLinecapProperty, value);
        }

        public Func<double, string>? Format
        {
            get => (Func<double, string>?)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        public Brush? DefineBackColor
        {
            get => (Brush?)GetValue(DefineBackColorProperty);
            set => SetValue(DefineBackColorProperty, value);
        }

        public Brush? TextColor
        {
            get => (Brush?)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        #endregion

        #region 内部属性（用于绑定）

        /// <summary>
        /// 格式化后的文字内容
        /// </summary>
        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.Register(nameof(FormattedText), typeof(string), typeof(VistaProgress),
                new PropertyMetadata("0%"));

        public string FormattedText
        {
            get => (string)GetValue(FormattedTextProperty);
            private set => SetValue(FormattedTextProperty, value);
        }

        /// <summary>
        /// 计算后的进度条颜色
        /// </summary>
        public static readonly DependencyProperty ComputedColorProperty =
            DependencyProperty.Register(nameof(ComputedColor), typeof(Brush), typeof(VistaProgress),
                new PropertyMetadata(null));

        public Brush? ComputedColor
        {
            get => (Brush?)GetValue(ComputedColorProperty);
            private set => SetValue(ComputedColorProperty, value);
        }

        #endregion

        #region 私有方法

        private static object CoercePercentage(DependencyObject d, object baseValue)
        {
            if (baseValue is double value)
            {
                return Math.Max(0, Math.Min(100, value));
            }
            return 0.0;
        }

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaProgress progress)
            {
                progress.UpdateProgress();
            }
        }

        private static void OnProgressTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaProgress progress)
            {
                progress.UpdateProgress();
            }
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaProgress progress)
            {
                progress.UpdateProgress();
            }
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaProgress progress)
            {
                progress.UpdateProgress();
            }
        }

        private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaProgress progress)
            {
                progress.UpdateProgress();
            }
        }

        private void UpdateProgress()
        {
            UpdateFormattedText();
            UpdateComputedColor();
            UpdateCircleProgress();
        }

        private void UpdateCircleProgress()
        {
            if (_progressCanvas == null || _backgroundArc == null || _foregroundArc == null ||
                _backgroundFigure == null || _foregroundFigure == null)
            {
                return;
            }

            var canvasSize = Width;
            var centerX = canvasSize / 2;
            var centerY = canvasSize / 2;
            var radius = (canvasSize - StrokeWidth) / 2;

            // 更新文字位置（居中显示在圆环内部，但只在 ShowText 为 true 且没有右侧文字时显示）
            if (_circleText != null && ShowText)
            {
                // 如果显示右侧文字，隐藏内部文字
                if (_circleTextRight != null && _circleTextRight.Visibility == Visibility.Visible)
                {
                    _circleText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _circleText.Visibility = Visibility.Visible;
                    // 使用 Measure 获取文字的实际大小
                    _circleText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    _circleText.Arrange(new Rect(0, 0, _circleText.DesiredSize.Width, _circleText.DesiredSize.Height));
                    
                    Canvas.SetLeft(_circleText, centerX - _circleText.DesiredSize.Width / 2);
                    Canvas.SetTop(_circleText, centerY - _circleText.DesiredSize.Height / 2);
                }
            }

            if (ProgressType == ProgressType.Circle)
            {
                // 环形进度条：从顶部开始，顺时针绘制
                var startAngle = -90; // 从顶部开始
                var sweepAngle = Percentage * 3.6; // 360度 / 100 = 3.6度每百分比
                
                // 背景：完整的圆
                UpdateArcSegment(_backgroundFigure, _backgroundArc, centerX, centerY, radius, -90, 360);
                
                // 前景：根据百分比绘制
                UpdateArcSegment(_foregroundFigure, _foregroundArc, centerX, centerY, radius, startAngle, sweepAngle);
            }
            else if (ProgressType == ProgressType.Dashboard)
            {
                // 仪表盘进度条：从底部左侧开始，顺时针绘制约270度
                var startAngle = -135; // 从底部左侧开始
                var totalAngle = 270; // 总角度270度
                var sweepAngle = Percentage * totalAngle / 100.0;
                
                // 背景：完整的270度弧
                UpdateArcSegment(_backgroundFigure, _backgroundArc, centerX, centerY, radius, startAngle, totalAngle);
                
                // 前景：根据百分比绘制
                UpdateArcSegment(_foregroundFigure, _foregroundArc, centerX, centerY, radius, startAngle, sweepAngle);
            }
        }

        private void UpdateArcSegment(PathFigure figure, ArcSegment arc, double centerX, double centerY, double radius, double startAngle, double sweepAngle)
        {
            if (figure == null || arc == null) return;

            var startAngleRad = startAngle * Math.PI / 180.0;
            var endAngleRad = (startAngle + sweepAngle) * Math.PI / 180.0;

            var startX = centerX + radius * Math.Cos(startAngleRad);
            var startY = centerY + radius * Math.Sin(startAngleRad);
            var endX = centerX + radius * Math.Cos(endAngleRad);
            var endY = centerY + radius * Math.Sin(endAngleRad);

            figure.StartPoint = new Point(startX, startY);
            arc.Point = new Point(endX, endY);
            arc.Size = new Size(radius, radius);
            arc.SweepDirection = SweepDirection.Clockwise;
            arc.IsLargeArc = Math.Abs(sweepAngle) > 180;
        }

        private void UpdateFormattedText()
        {
            if (Format != null)
            {
                FormattedText = Format(Percentage);
            }
            else
            {
                FormattedText = $"{Percentage:F0}%";
            }
            
            // 更新 Tooltip
            ToolTip = FormattedText;
            
            // 文字更新后，如果是环形/仪表盘类型，需要更新文字位置
            if (ProgressType == ProgressType.Circle || ProgressType == ProgressType.Dashboard)
            {
                Dispatcher.BeginInvoke(new Action(() => UpdateCircleProgress()), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void UpdateComputedColor()
        {
            // 如果设置了 Color，优先使用 Color
            if (Color != null)
            {
                ComputedColor = GetColorFromObject(Color, Percentage);
                return;
            }

            // 否则根据 Status 设置颜色
            ComputedColor = Status switch
            {
                ProgressStatus.Success => new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#67C23A")),
                ProgressStatus.Warning => new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E6A23C")),
                ProgressStatus.Exception => new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F56C6C")),
                _ => new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#409EFF"))
            };
        }

        private Brush GetColorFromObject(object colorObj, double percentage)
        {
            // 字符串
            if (colorObj is string colorStr)
            {
                try
                {
                    return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorStr));
                }
                catch
                {
                    return new SolidColorBrush(System.Windows.Media.Colors.Blue);
                }
            }

            // 函数
            if (colorObj is Func<double, string> colorFunc)
            {
                var colorString = colorFunc(percentage);
                try
                {
                    return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString));
                }
                catch
                {
                    return new SolidColorBrush(System.Windows.Media.Colors.Blue);
                }
            }

            // 数组
            if (colorObj is IEnumerable<ProgressColorStop> colorArray)
            {
                foreach (var stop in colorArray)
                {
                    if (percentage <= stop.Percentage)
                    {
                        try
                        {
                            return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(stop.Color));
                        }
                        catch
                        {
                            return new SolidColorBrush(System.Windows.Media.Colors.Blue);
                        }
                    }
                }
                // 如果超过所有阈值，使用最后一个颜色
                var lastStop = Enumerable.LastOrDefault(colorArray);
                if (lastStop != null)
                {
                    try
                    {
                        return new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(lastStop.Color));
                    }
                    catch
                    {
                        return new SolidColorBrush(System.Windows.Media.Colors.Blue);
                    }
                }
            }

            return new SolidColorBrush(System.Windows.Media.Colors.Blue);
        }

        #endregion
    }

    /// <summary>
    /// 进度条类型枚举
    /// </summary>
    public enum ProgressType
    {
        Line,
        Circle,
        Dashboard
    }

    /// <summary>
    /// 进度条状态枚举
    /// </summary>
    public enum ProgressStatus
    {
        None,
        Success,
        Warning,
        Exception
    }

    /// <summary>
    /// 进度条颜色停止点
    /// </summary>
    public class ProgressColorStop
    {
        public string Color { get; set; } = "#409EFF";
        public double Percentage { get; set; } = 0;
    }
}

