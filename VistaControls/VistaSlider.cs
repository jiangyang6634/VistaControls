using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace VistaControls
{
    /// <summary>
    /// VistaSlider - 滑块控件
    /// </summary>
    public class VistaSlider : Slider
    {
        private Popup? _tooltipPopup;
        private TextBlock? _tooltipText;
        private VistaInputNumber? _inputNumber;
        private ItemsControl? _stopsControl;
        private bool _isDragging;

        static VistaSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaSlider),
                new FrameworkPropertyMetadata(typeof(VistaSlider)));
        }

        public VistaSlider()
        {
            Minimum = 0;
            Maximum = 100;
            Value = 0;
            TickFrequency = 1;
            IsSnapToTickEnabled = false;
            ValueChanged += VistaSlider_ValueChanged;
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
            AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            _tooltipPopup = GetTemplateChild("PART_TooltipPopup") as Popup;
            _tooltipText = GetTemplateChild("PART_TooltipText") as TextBlock;
            _inputNumber = GetTemplateChild("PART_InputNumber") as VistaInputNumber;
            _stopsControl = GetTemplateChild("PART_Stops") as ItemsControl;
            
            if (_inputNumber != null)
            {
                _inputNumber.Value = Value;
                _inputNumber.Min = Minimum;
                _inputNumber.Max = Maximum;
                _inputNumber.Step = TickFrequency > 0 ? TickFrequency : 1;
                _inputNumber.Changed += InputNumber_Changed;
            }
            
            UpdateTooltip();
            UpdateStops();
            
            // 监听属性变化
            DependencyPropertyDescriptor.FromProperty(ShowStopsProperty, typeof(VistaSlider))
                .AddValueChanged(this, (s, e) => UpdateStops());
            DependencyPropertyDescriptor.FromProperty(TickFrequencyProperty, typeof(VistaSlider))
                .AddValueChanged(this, (s, e) => UpdateStops());
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(VistaSlider))
                .AddValueChanged(this, (s, e) => UpdateStops());
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(VistaSlider))
                .AddValueChanged(this, (s, e) => UpdateStops());
        }

        #region 依赖属性

        /// <summary>
        /// 是否显示输入框
        /// </summary>
        public static readonly DependencyProperty ShowInputProperty =
            DependencyProperty.Register(nameof(ShowInput), typeof(bool), typeof(VistaSlider),
                new PropertyMetadata(false));

        /// <summary>
        /// 在显示输入框的情况下，是否显示输入框的控制按钮
        /// </summary>
        public static readonly DependencyProperty ShowInputControlsProperty =
            DependencyProperty.Register(nameof(ShowInputControls), typeof(bool), typeof(VistaSlider),
                new PropertyMetadata(true));

        /// <summary>
        /// 输入框的尺寸
        /// </summary>
        public static readonly DependencyProperty InputSizeProperty =
            DependencyProperty.Register(nameof(InputSize), typeof(InputSize), typeof(VistaSlider),
                new PropertyMetadata(InputSize.Small));

        /// <summary>
        /// 是否显示间断点
        /// </summary>
        public static readonly DependencyProperty ShowStopsProperty =
            DependencyProperty.Register(nameof(ShowStops), typeof(bool), typeof(VistaSlider),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否显示 tooltip
        /// </summary>
        public static readonly DependencyProperty ShowTooltipProperty =
            DependencyProperty.Register(nameof(ShowTooltip), typeof(bool), typeof(VistaSlider),
                new PropertyMetadata(true));

        /// <summary>
        /// 格式化 tooltip message 的委托
        /// </summary>
        public static readonly DependencyProperty FormatTooltipProperty =
            DependencyProperty.Register(nameof(FormatTooltip), typeof(Func<double, string>), typeof(VistaSlider),
                new PropertyMetadata(null));

        /// <summary>
        /// 是否为范围选择
        /// </summary>
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register(nameof(Range), typeof(bool), typeof(VistaSlider),
                new PropertyMetadata(false, OnRangeChanged));

        /// <summary>
        /// 范围选择的值（两个元素的数组）
        /// </summary>
        public static readonly DependencyProperty RangeValueProperty =
            DependencyProperty.Register(nameof(RangeValue), typeof(double[]), typeof(VistaSlider),
                new FrameworkPropertyMetadata(new double[] { 0, 100 }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRangeValueChanged));

        /// <summary>
        /// 是否竖向模式
        /// </summary>
        public static readonly DependencyProperty VerticalProperty =
            DependencyProperty.Register(nameof(Vertical), typeof(bool), typeof(VistaSlider),
                new PropertyMetadata(false, OnVerticalChanged));

        /// <summary>
        /// Slider 高度，竖向模式时必填
        /// </summary>
        public static readonly DependencyProperty SliderHeightProperty =
            DependencyProperty.Register(nameof(SliderHeight), typeof(double), typeof(VistaSlider),
                new PropertyMetadata(200.0));

        /// <summary>
        /// 标记字典，key 为数值，value 为标记文本或样式对象
        /// </summary>
        public static readonly DependencyProperty MarksProperty =
            DependencyProperty.Register(nameof(Marks), typeof(Dictionary<double, object>), typeof(VistaSlider),
                new PropertyMetadata(null));

        /// <summary>
        /// 输入时的去抖延迟，毫秒，仅在show-input等于true时有效
        /// </summary>
        public static readonly DependencyProperty DebounceProperty =
            DependencyProperty.Register(nameof(Debounce), typeof(int), typeof(VistaSlider),
                new PropertyMetadata(300));

        /// <summary>
        /// tooltip 的自定义类名
        /// </summary>
        public static readonly DependencyProperty TooltipClassProperty =
            DependencyProperty.Register(nameof(TooltipClass), typeof(string), typeof(VistaSlider),
                new PropertyMetadata(string.Empty));

        #endregion

        #region 属性

        /// <summary>
        /// 是否显示输入框
        /// </summary>
        public bool ShowInput
        {
            get => (bool)GetValue(ShowInputProperty);
            set => SetValue(ShowInputProperty, value);
        }

        /// <summary>
        /// 在显示输入框的情况下，是否显示输入框的控制按钮
        /// </summary>
        public bool ShowInputControls
        {
            get => (bool)GetValue(ShowInputControlsProperty);
            set => SetValue(ShowInputControlsProperty, value);
        }

        /// <summary>
        /// 输入框的尺寸
        /// </summary>
        public InputSize InputSize
        {
            get => (InputSize)GetValue(InputSizeProperty);
            set => SetValue(InputSizeProperty, value);
        }

        /// <summary>
        /// 是否显示间断点
        /// </summary>
        public bool ShowStops
        {
            get => (bool)GetValue(ShowStopsProperty);
            set => SetValue(ShowStopsProperty, value);
        }

        /// <summary>
        /// 是否显示 tooltip
        /// </summary>
        public bool ShowTooltip
        {
            get => (bool)GetValue(ShowTooltipProperty);
            set => SetValue(ShowTooltipProperty, value);
        }

        /// <summary>
        /// 格式化 tooltip message 的委托
        /// </summary>
        public Func<double, string>? FormatTooltip
        {
            get => (Func<double, string>?)GetValue(FormatTooltipProperty);
            set => SetValue(FormatTooltipProperty, value);
        }

        /// <summary>
        /// 是否为范围选择
        /// </summary>
        public bool Range
        {
            get => (bool)GetValue(RangeProperty);
            set => SetValue(RangeProperty, value);
        }

        /// <summary>
        /// 范围选择的值（两个元素的数组）
        /// </summary>
        public double[] RangeValue
        {
            get => (double[]?)GetValue(RangeValueProperty) ?? new double[] { 0, 100 };
            set => SetValue(RangeValueProperty, value);
        }

        /// <summary>
        /// 是否竖向模式
        /// </summary>
        public bool Vertical
        {
            get => (bool)GetValue(VerticalProperty);
            set => SetValue(VerticalProperty, value);
        }

        /// <summary>
        /// Slider 高度，竖向模式时必填
        /// </summary>
        public double SliderHeight
        {
            get => (double)GetValue(SliderHeightProperty);
            set => SetValue(SliderHeightProperty, value);
        }

        /// <summary>
        /// 标记字典，key 为数值，value 为标记文本或样式对象
        /// </summary>
        public Dictionary<double, object>? Marks
        {
            get => (Dictionary<double, object>?)GetValue(MarksProperty);
            set => SetValue(MarksProperty, value);
        }

        /// <summary>
        /// 输入时的去抖延迟，毫秒，仅在show-input等于true时有效
        /// </summary>
        public int Debounce
        {
            get => (int)GetValue(DebounceProperty);
            set => SetValue(DebounceProperty, value);
        }

        /// <summary>
        /// tooltip 的自定义类名
        /// </summary>
        public string TooltipClass
        {
            get => (string)GetValue(TooltipClassProperty);
            set => SetValue(TooltipClassProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 值改变时触发（使用鼠标拖曳时，只在松开鼠标后触发）
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double>? Change;

        /// <summary>
        /// 数据改变时触发（使用鼠标拖曳时，活动过程实时触发）
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double>? Input;

        #endregion

        #region 私有方法

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSlider slider)
            {
                // 范围选择模式需要特殊处理
                slider.UpdateRangeMode();
            }
        }

        private static void OnRangeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSlider slider && e.NewValue is double[] rangeValue && rangeValue.Length >= 2)
            {
                // 更新单个值以反映范围选择
                slider.Value = rangeValue[0];
            }
        }

        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSlider slider)
            {
                slider.Orientation = slider.Vertical ? Orientation.Vertical : Orientation.Horizontal;
            }
        }

        private void UpdateRangeMode()
        {
            // 范围选择模式的实现需要自定义模板
        }

        private void VistaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateTooltip();
            
            // 触发 Input 事件（实时）
            Input?.Invoke(this, e);
            
            // 如果不是拖拽中，触发 Change 事件
            if (!_isDragging)
            {
                Change?.Invoke(this, e);
            }
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
            if (_tooltipPopup != null && ShowTooltip)
            {
                _tooltipPopup.IsOpen = true;
                UpdateTooltipPosition();
            }
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            if (_tooltipPopup != null)
            {
                _tooltipPopup.IsOpen = false;
            }
            
            // 拖拽结束时触发 Change 事件
            var oldValue = Value;
            Change?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, Value, ValueChangedEvent));
        }

        private void UpdateTooltip()
        {
            if (_tooltipText == null) return;
            
            double displayValue = Value;
            string text;
            
            if (FormatTooltip != null)
            {
                text = FormatTooltip(displayValue);
            }
            else
            {
                text = displayValue.ToString("F0");
            }
            
            _tooltipText.Text = text;
            UpdateTooltipPosition();
        }

        private void UpdateTooltipPosition()
        {
            if (_tooltipPopup == null) return;
            
            // 计算 tooltip 位置（跟随滑块）
            var track = GetTemplateChild("PART_Track") as Track;
            if (track?.Thumb != null)
            {
                _tooltipPopup.PlacementTarget = track.Thumb;
            }
        }

        private void InputNumber_Changed(object sender, ValueChangedEventArgs e)
        {
            if (_inputNumber != null)
            {
                Value = _inputNumber.Value;
            }
        }

        private void UpdateStops()
        {
            if (_stopsControl == null || !ShowStops || TickFrequency <= 0) return;
            
            var stops = new ObservableCollection<StopItem>();
            double current = Minimum;
            while (current <= Maximum)
            {
                stops.Add(new StopItem { Value = current });
                current += TickFrequency;
            }
            
            _stopsControl.ItemsSource = stops;
        }

        #endregion
    }

    /// <summary>
    /// 间断点数据项
    /// </summary>
    internal class StopItem
    {
        public double Value { get; set; }
    }
}

