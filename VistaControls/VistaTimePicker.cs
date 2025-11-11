using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaTimePicker - 任意时间点选择器
    /// 可以选择任意时间，支持 selectableRange 限制可选时间范围
    /// </summary>
    public class VistaTimePicker : Control
    {
        private Popup? _popup;
        private Border? _displayArea;
        private Button? _clearButton;
        private TextBlock? _displayText;
        private TextBlock? _placeholderText;
        private TextBlock? _startPlaceholderText;
        private TextBlock? _endPlaceholderText;
        private TextBlock? _startDisplayText;
        private TextBlock? _endDisplayText;
        private TextBlock? _rangeSeparator;
        private StackPanel? _rangePanel;

        // 时间选择面板
        private StackPanel? _hourPanel;
        private StackPanel? _minutePanel;
        private StackPanel? _secondPanel;
        private ScrollViewer? _hourScrollViewer;
        private ScrollViewer? _minuteScrollViewer;
        private ScrollViewer? _secondScrollViewer;

        // 当前选择的时间（非范围模式）
        private DateTime? _selectedTime;
        // 当前选择的时间范围（范围模式）
        private DateTime[]? _selectedTimeRange;

        static VistaTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTimePicker),
                new FrameworkPropertyMetadata(typeof(VistaTimePicker)));
        }

        public VistaTimePicker()
        {
            Loaded += VistaTimePicker_Loaded;
        }

        private void VistaTimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: 时间范围选择功能暂时注释掉，待后续实现
            // if (IsRange && ValueRange == null)
            // {
            //     ValueRange = new DateTime[2];
            // }
            // else 
            if (Value == null && DefaultValue != null)
            {
                Value = DefaultValue;
            }

            UpdateDisplay();
            UpdateTimePanels();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild("PART_Popup") as Popup;
            _displayArea = GetTemplateChild("PART_Display") as Border;
            _clearButton = GetTemplateChild("PART_Clear") as Button;
            _displayText = GetTemplateChild("displayText") as TextBlock;
            _placeholderText = GetTemplateChild("placeholder") as TextBlock;
            _startPlaceholderText = GetTemplateChild("startPlaceholder") as TextBlock;
            _endPlaceholderText = GetTemplateChild("endPlaceholder") as TextBlock;
            _startDisplayText = GetTemplateChild("startDisplayText") as TextBlock;
            _endDisplayText = GetTemplateChild("endDisplayText") as TextBlock;
            _rangeSeparator = GetTemplateChild("rangeSeparator") as TextBlock;
            _rangePanel = GetTemplateChild("rangePanel") as StackPanel;

            _hourPanel = GetTemplateChild("hourPanel") as StackPanel;
            _minutePanel = GetTemplateChild("minutePanel") as StackPanel;
            _secondPanel = GetTemplateChild("secondPanel") as StackPanel;
            _hourScrollViewer = GetTemplateChild("hourScrollViewer") as ScrollViewer;
            _minuteScrollViewer = GetTemplateChild("minuteScrollViewer") as ScrollViewer;
            _secondScrollViewer = GetTemplateChild("secondScrollViewer") as ScrollViewer;

            if (_displayArea != null)
            {
                _displayArea.MouseLeftButtonDown += (_, __) => TogglePopup();
            }

            if (_clearButton != null)
            {
                _clearButton.Click += (_, __) => ClearSelection();
            }

            // 全局点击关闭
            if (Application.Current?.MainWindow != null)
            {
                Application.Current.MainWindow.PreviewMouseDown -= GlobalPreviewMouseDown;
                Application.Current.MainWindow.PreviewMouseDown += GlobalPreviewMouseDown;
            }

            UpdateDisplay();
            UpdateTimePanels();
        }

        #region 依赖属性

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(DateTime?), typeof(VistaTimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty ValueRangeProperty =
            DependencyProperty.Register(nameof(ValueRange), typeof(DateTime[]), typeof(VistaTimePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueRangeChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaTimePicker),
                new PropertyMetadata("选择时间"));

        public static readonly DependencyProperty StartPlaceholderProperty =
            DependencyProperty.Register(nameof(StartPlaceholder), typeof(string), typeof(VistaTimePicker),
                new PropertyMetadata("开始时间"));

        public static readonly DependencyProperty EndPlaceholderProperty =
            DependencyProperty.Register(nameof(EndPlaceholder), typeof(string), typeof(VistaTimePicker),
                new PropertyMetadata("结束时间"));

        public static readonly DependencyProperty ReadonlyProperty =
            DependencyProperty.Register(nameof(Readonly), typeof(bool), typeof(VistaTimePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(VistaTimePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(VistaTimePicker),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ClearableProperty =
            DependencyProperty.Register(nameof(Clearable), typeof(bool), typeof(VistaTimePicker),
                new PropertyMetadata(true));

        public static readonly DependencyProperty InputSizeProperty =
            DependencyProperty.Register(nameof(InputSize), typeof(InputSize), typeof(VistaTimePicker),
                new PropertyMetadata(InputSize.Default));

        public static readonly DependencyProperty IsRangeProperty =
            DependencyProperty.Register(nameof(IsRange), typeof(bool), typeof(VistaTimePicker),
                new PropertyMetadata(false, OnIsRangeChanged));

        public static readonly DependencyProperty ArrowControlProperty =
            DependencyProperty.Register(nameof(ArrowControl), typeof(bool), typeof(VistaTimePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty RangeSeparatorProperty =
            DependencyProperty.Register(nameof(RangeSeparator), typeof(string), typeof(VistaTimePicker),
                new PropertyMetadata("至"));

        public static readonly DependencyProperty PickerOptionsProperty =
            DependencyProperty.Register(nameof(PickerOptions), typeof(TimePickerOptions), typeof(VistaTimePicker),
                new PropertyMetadata(new TimePickerOptions()));

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(DateTime?), typeof(VistaTimePicker),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register(nameof(ValueFormat), typeof(string), typeof(VistaTimePicker),
                new PropertyMetadata(null));

        #endregion

        #region 属性

        public DateTime? Value
        {
            get => (DateTime?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public DateTime[]? ValueRange
        {
            get => (DateTime[]?)GetValue(ValueRangeProperty);
            set => SetValue(ValueRangeProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string StartPlaceholder
        {
            get => (string)GetValue(StartPlaceholderProperty);
            set => SetValue(StartPlaceholderProperty, value);
        }

        public string EndPlaceholder
        {
            get => (string)GetValue(EndPlaceholderProperty);
            set => SetValue(EndPlaceholderProperty, value);
        }

        public bool Readonly
        {
            get => (bool)GetValue(ReadonlyProperty);
            set => SetValue(ReadonlyProperty, value);
        }

        public bool IsDisabled
        {
            get => (bool)GetValue(IsDisabledProperty);
            set => SetValue(IsDisabledProperty, value);
        }

        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }

        public bool Clearable
        {
            get => (bool)GetValue(ClearableProperty);
            set => SetValue(ClearableProperty, value);
        }

        public InputSize InputSize
        {
            get => (InputSize)GetValue(InputSizeProperty);
            set => SetValue(InputSizeProperty, value);
        }

        public bool IsRange
        {
            get => (bool)GetValue(IsRangeProperty);
            set => SetValue(IsRangeProperty, value);
        }

        public bool ArrowControl
        {
            get => (bool)GetValue(ArrowControlProperty);
            set => SetValue(ArrowControlProperty, value);
        }

        public string RangeSeparator
        {
            get => (string)GetValue(RangeSeparatorProperty);
            set => SetValue(RangeSeparatorProperty, value);
        }

        public TimePickerOptions PickerOptions
        {
            get => (TimePickerOptions)GetValue(PickerOptionsProperty);
            set => SetValue(PickerOptionsProperty, value);
        }

        public DateTime? DefaultValue
        {
            get => (DateTime?)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        public string? ValueFormat
        {
            get => (string?)GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler<object?>? Change;
        public event EventHandler? Blur;
        public event EventHandler? OnFocus;

        #endregion

        #region 私有字段和方法

        // TODO: 时间范围选择功能暂时注释掉，待后续实现
        // private bool _isSelectingStart = true; // 范围模式下，当前是否在选择开始时间

        // 临时选择的时间（用于在弹出框中显示，但还未确认）
        private DateTime? _tempSelectedTime = null;

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTimePicker picker)
            {
                picker._selectedTime = picker.Value;
                // 如果弹出框未打开，同步临时时间
                if (picker._popup?.IsOpen != true)
                {
                    picker._tempSelectedTime = picker.Value;
                }
                picker.UpdateDisplay();
                picker.UpdateTimePanels();
            }
        }

        private static void OnValueRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTimePicker picker)
            {
                picker._selectedTimeRange = picker.ValueRange;
                picker.UpdateDisplay();
                picker.UpdateTimePanels();
            }
        }

        private static void OnIsRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTimePicker picker)
            {
                picker.UpdateDisplay();
            }
        }

        private void UpdateTimePanels()
        {
            if (_hourPanel == null || _minutePanel == null || _secondPanel == null) return;

            // 清空现有按钮（必须在UI线程）
            _hourPanel.Children.Clear();
            _minutePanel.Children.Clear();
            _secondPanel.Children.Clear();

            // 获取当前时间（在UI线程获取）
            // TODO: 时间范围选择功能暂时注释掉，待后续实现
            var currentTime = _selectedTime ?? DefaultValue;
            var baseTime = _tempSelectedTime ?? currentTime ?? DateTime.Now;
            var today = DateTime.Today;
            
            DateTime time;
            try
            {
                time = new DateTime(today.Year, today.Month, today.Day, 
                    baseTime.Hour, baseTime.Minute, baseTime.Second);
            }
            catch
            {
                time = DateTime.Now;
            }

            var targetTime = time; // 保存目标时间用于后续使用

            // 异步生成按钮列表，使用 async/await 模式，不阻塞UI线程
            _ = UpdateTimePanelsAsync(targetTime);
        }

        private async Task UpdateTimePanelsAsync(DateTime targetTime)
        {
            // 在后台线程生成按钮数据
            var (hourButtons, minuteButtons, secondButtons) = await Task.Run(() =>
            {
                var hourButtons = new List<(string Text, int Hour, bool IsSelected)>();
                var minuteButtons = new List<(string Text, int Minute, bool IsSelected)>();
                var secondButtons = new List<(string Text, int Second, bool IsSelected)>();

                // 生成小时选项 (0-23)
                for (int h = 0; h < 24; h++)
                {
                    hourButtons.Add((h.ToString("D2"), h, h == targetTime.Hour));
                }

                // 生成分钟选项 (0-59)
                for (int m = 0; m < 60; m++)
                {
                    minuteButtons.Add((m.ToString("D2"), m, m == targetTime.Minute));
                }

                // 生成秒选项 (0-59)
                for (int s = 0; s < 60; s++)
                {
                    secondButtons.Add((s.ToString("D2"), s, s == targetTime.Second));
                }

                return (hourButtons, minuteButtons, secondButtons);
            });

            // 回到UI线程更新UI（await 会自动回到调用线程，即UI线程）
            // 但为了确保，我们显式使用 Dispatcher
            await Dispatcher.InvokeAsync(() =>
            {
                // 检查控件是否还存在（防止在异步过程中控件被销毁）
                if (_hourPanel == null || _minutePanel == null || _secondPanel == null)
                    return;

                // 清空现有按钮
                _hourPanel.Children.Clear();
                _minutePanel.Children.Clear();
                _secondPanel.Children.Clear();

                // 添加小时按钮
                foreach (var (text, hour, isSelected) in hourButtons)
                {
                    var hourValue = hour; // 避免闭包捕获问题
                    var hourButton = CreateTimeButton(text, isSelected, () => SelectHour(hourValue));
                    _hourPanel.Children.Add(hourButton);
                }

                // 添加分钟按钮
                foreach (var (text, minute, isSelected) in minuteButtons)
                {
                    var minuteValue = minute; // 避免闭包捕获问题
                    var minuteButton = CreateTimeButton(text, isSelected, () => SelectMinute(minuteValue));
                    _minutePanel.Children.Add(minuteButton);
                }

                // 添加秒按钮
                foreach (var (text, second, isSelected) in secondButtons)
                {
                    var secondValue = second; // 避免闭包捕获问题
                    var secondButton = CreateTimeButton(text, isSelected, () => SelectSecond(secondValue));
                    _secondPanel.Children.Add(secondButton);
                }

                // 滚动到当前选择的时间
                ScrollToTime(targetTime);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private Button CreateTimeButton(string text, bool isSelected, Action onClick)
        {
            var button = new Button
            {
                Content = text,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 2, 0, 2),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = isSelected ? 
                    new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xEC, 0xF5, 0xFF)) :
                    System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0), // 移除边框
                Tag = text // 存储文本用于后续更新选中状态
            };

            button.Click += (s, e) => onClick();

            return button;
        }

        private void SelectHour(int hour)
        {
            // 点击小时：只更新临时时间，不更新Value，不关闭弹出框
            UpdateTempTime(hour, null, null);
        }

        private void SelectMinute(int minute)
        {
            // 点击分钟：只更新临时时间，不更新Value，不关闭弹出框
            UpdateTempTime(null, minute, null);
        }

        private void SelectSecond(int second)
        {
            // 点击秒：确认选择，更新Value，关闭弹出框
            ConfirmTime(null, null, second);
        }

        /// <summary>
        /// 更新临时时间（点击小时/分钟时调用，不更新Value，不关闭弹出框）
        /// </summary>
        private void UpdateTempTime(int? hour, int? minute, int? second)
        {
            // 使用临时时间或当前值作为基准
            var baseTime = _tempSelectedTime ?? _selectedTime ?? DateTime.Now;
            var today = DateTime.Today;
            
            var h = hour ?? baseTime.Hour;
            var m = minute ?? baseTime.Minute;
            var s = second ?? baseTime.Second;

            // 验证时间值的有效性
            if (h < 0 || h > 23) h = baseTime.Hour;
            if (m < 0 || m > 59) m = baseTime.Minute;
            if (s < 0 || s > 59) s = baseTime.Second;

            DateTime newTime;
            try
            {
                newTime = new DateTime(today.Year, today.Month, today.Day, h, m, s);
            }
            catch
            {
                newTime = DateTime.Now;
            }

            // 更新临时时间
            _tempSelectedTime = newTime;

            // 只更新面板显示，不更新Value，不关闭弹出框
            UpdateTimePanels();
            
            // 更新按钮的选中状态
            UpdateButtonSelection();
        }

        /// <summary>
        /// 更新按钮的选中状态
        /// </summary>
        private void UpdateButtonSelection()
        {
            if (_tempSelectedTime == null) return;
            
            var time = _tempSelectedTime.Value;

            // 更新小时按钮
            if (_hourPanel != null)
            {
                foreach (var child in _hourPanel.Children)
                {
                    if (child is Button btn && btn.Tag is string text)
                    {
                        var hour = int.Parse(text);
                        btn.Background = hour == time.Hour ?
                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xEC, 0xF5, 0xFF)) :
                            System.Windows.Media.Brushes.Transparent;
                    }
                }
            }

            // 更新分钟按钮
            if (_minutePanel != null)
            {
                foreach (var child in _minutePanel.Children)
                {
                    if (child is Button btn && btn.Tag is string text)
                    {
                        var minute = int.Parse(text);
                        btn.Background = minute == time.Minute ?
                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xEC, 0xF5, 0xFF)) :
                            System.Windows.Media.Brushes.Transparent;
                    }
                }
            }

            // 更新秒按钮
            if (_secondPanel != null)
            {
                foreach (var child in _secondPanel.Children)
                {
                    if (child is Button btn && btn.Tag is string text)
                    {
                        var second = int.Parse(text);
                        btn.Background = second == time.Second ?
                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xEC, 0xF5, 0xFF)) :
                            System.Windows.Media.Brushes.Transparent;
                    }
                }
            }
        }

        /// <summary>
        /// 确认时间（点击秒时调用，更新Value，关闭弹出框）
        /// </summary>
        private void ConfirmTime(int? hour, int? minute, int? second)
        {
            // 使用临时时间或当前值作为基准
            var baseTime = _tempSelectedTime ?? _selectedTime ?? DateTime.Now;
            var today = DateTime.Today;
            
            var h = hour ?? baseTime.Hour;
            var m = minute ?? baseTime.Minute;
            var s = second ?? baseTime.Second;

            // 验证时间值的有效性
            if (h < 0 || h > 23) h = baseTime.Hour;
            if (m < 0 || m > 59) m = baseTime.Minute;
            if (s < 0 || s > 59) s = baseTime.Second;

            DateTime newTime;
            try
            {
                newTime = new DateTime(today.Year, today.Month, today.Day, h, m, s);
            }
            catch
            {
                newTime = DateTime.Now;
            }

            // 检查是否在可选范围内（只有在设置了SelectableRange时才检查）
            // 如果不在范围内，自动调整到范围内的有效时间
            if (PickerOptions?.SelectableRange != null && !IsTimeInSelectableRange(newTime))
            {
                // 尝试调整到范围内的有效时间
                var adjustedTime = AdjustTimeToSelectableRange(newTime);
                if (adjustedTime.HasValue)
                {
                    newTime = adjustedTime.Value;
                }
                else
                {
                    // 如果无法调整，使用范围内的第一个有效时间
                    var firstValidTime = GetFirstValidTimeInRange();
                    if (firstValidTime.HasValue)
                    {
                        newTime = firstValidTime.Value;
                    }
                }
            }

            // 确认选择，更新Value
            _selectedTime = newTime;
            _tempSelectedTime = null; // 清空临时时间
            Value = newTime;
            Change?.Invoke(this, Value);

            // 更新显示和面板
            UpdateDisplay();
            UpdateTimePanels();
            
            // 选择时间后关闭弹出框
            if (_popup != null)
            {
                _popup.IsOpen = false;
            }
        }

        private bool IsTimeInSelectableRange(DateTime time)
        {
            if (PickerOptions?.SelectableRange == null) return true;

            var timeOfDay = time.TimeOfDay;

            if (PickerOptions.SelectableRange is string rangeStr)
            {
                return IsTimeInRangeString(timeOfDay, rangeStr);
            }
            else if (PickerOptions.SelectableRange is IEnumerable ranges)
            {
                foreach (var range in ranges)
                {
                    if (range is string r && IsTimeInRangeString(timeOfDay, r))
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

        private bool IsTimeInRangeString(TimeSpan time, string rangeStr)
        {
            try
            {
                var parts = rangeStr.Split('-');
                if (parts.Length == 2)
                {
                    var start = ParseTimeSpan(parts[0].Trim());
                    var end = ParseTimeSpan(parts[1].Trim());
                    return time >= start && time <= end;
                }
            }
            catch { }

            return true;
        }

        private TimeSpan ParseTimeSpan(string timeStr)
        {
            var parts = timeStr.Split(':');
            if (parts.Length >= 2)
            {
                var hours = int.Parse(parts[0]);
                var minutes = int.Parse(parts[1]);
                var seconds = parts.Length >= 3 ? int.Parse(parts[2]) : 0;
                return new TimeSpan(hours, minutes, seconds);
            }
            throw new ArgumentException("Invalid time format");
        }

        /// <summary>
        /// 调整时间到可选范围内
        /// </summary>
        private DateTime? AdjustTimeToSelectableRange(DateTime time)
        {
            if (PickerOptions?.SelectableRange == null) return time;

            var timeOfDay = time.TimeOfDay;
            var today = DateTime.Today;

            if (PickerOptions.SelectableRange is string rangeStr)
            {
                try
                {
                    var parts = rangeStr.Split('-');
                    if (parts.Length == 2)
                    {
                        var start = ParseTimeSpan(parts[0].Trim());
                        var end = ParseTimeSpan(parts[1].Trim());

                        // 如果时间在范围内，直接返回
                        if (timeOfDay >= start && timeOfDay <= end)
                        {
                            return time;
                        }

                        // 如果时间小于开始时间，调整到开始时间
                        if (timeOfDay < start)
                        {
                            return new DateTime(today.Year, today.Month, today.Day, start.Hours, start.Minutes, start.Seconds);
                        }

                        // 如果时间大于结束时间，调整到结束时间
                        if (timeOfDay > end)
                        {
                            return new DateTime(today.Year, today.Month, today.Day, end.Hours, end.Minutes, end.Seconds);
                        }
                    }
                }
                catch { }
            }
            else if (PickerOptions.SelectableRange is IEnumerable ranges)
            {
                // 对于多个范围，找到包含该时间的范围或最接近的范围
                foreach (var range in ranges)
                {
                    if (range is string r)
                    {
                        try
                        {
                            var parts = r.Split('-');
                            if (parts.Length == 2)
                            {
                                var start = ParseTimeSpan(parts[0].Trim());
                                var end = ParseTimeSpan(parts[1].Trim());

                                if (timeOfDay >= start && timeOfDay <= end)
                                {
                                    return time;
                                }
                            }
                        }
                        catch { }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取范围内的第一个有效时间
        /// </summary>
        private DateTime? GetFirstValidTimeInRange()
        {
            if (PickerOptions?.SelectableRange == null) return null;

            var today = DateTime.Today;

            if (PickerOptions.SelectableRange is string rangeStr)
            {
                try
                {
                    var parts = rangeStr.Split('-');
                    if (parts.Length == 2)
                    {
                        var start = ParseTimeSpan(parts[0].Trim());
                        return new DateTime(today.Year, today.Month, today.Day, start.Hours, start.Minutes, start.Seconds);
                    }
                }
                catch { }
            }
            else if (PickerOptions.SelectableRange is IEnumerable ranges)
            {
                foreach (var range in ranges)
                {
                    if (range is string r)
                    {
                        try
                        {
                            var parts = r.Split('-');
                            if (parts.Length == 2)
                            {
                                var start = ParseTimeSpan(parts[0].Trim());
                                return new DateTime(today.Year, today.Month, today.Day, start.Hours, start.Minutes, start.Seconds);
                            }
                        }
                        catch { }
                    }
                }
            }

            return null;
        }

        private void ScrollToTime(DateTime time)
        {
            if (_hourScrollViewer != null)
            {
                var hourButton = _hourPanel?.Children.OfType<Button>()
                    .FirstOrDefault(b => b.Content?.ToString() == time.Hour.ToString("D2"));
                if (hourButton != null)
                {
                    hourButton.BringIntoView();
                }
            }

            if (_minuteScrollViewer != null)
            {
                var minuteButton = _minutePanel?.Children.OfType<Button>()
                    .FirstOrDefault(b => b.Content?.ToString() == time.Minute.ToString("D2"));
                if (minuteButton != null)
                {
                    minuteButton.BringIntoView();
                }
            }

            if (_secondScrollViewer != null)
            {
                var secondButton = _secondPanel?.Children.OfType<Button>()
                    .FirstOrDefault(b => b.Content?.ToString() == time.Second.ToString("D2"));
                if (secondButton != null)
                {
                    secondButton.BringIntoView();
                }
            }
        }

        private void TogglePopup()
        {
            if (IsDisabled || Readonly) return;
            if (_popup == null) return;
            _popup.IsOpen = !_popup.IsOpen;
            if (_popup.IsOpen)
            {
                // 打开弹出框时，初始化临时时间为当前值或当前时间
                _tempSelectedTime = _selectedTime ?? DateTime.Now;
                UpdateTimePanels();
                OnFocus?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                // 关闭时清空临时时间，触发 Blur 事件
                _tempSelectedTime = null;
                Blur?.Invoke(this, EventArgs.Empty);
            }
        }

        private void GlobalPreviewMouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (_popup == null || !_popup.IsOpen) return;
            var source = e.OriginalSource as DependencyObject;
            if (IsDescendantOf(source, _displayArea)) return;
            if (IsDescendantOf(source, _popup.Child)) return;
            _popup.IsOpen = false;
            Blur?.Invoke(this, EventArgs.Empty);
        }

        private void ClearSelection()
        {
            if (IsDisabled) return;
            // TODO: 时间范围选择功能暂时注释掉，待后续实现
            // if (IsRange)
            // {
            //     ValueRange = null;
            //     _selectedTimeRange = null;
            //     Change?.Invoke(this, null);
            // }
            // else
            // {
                Value = null;
                _selectedTime = null;
                Change?.Invoke(this, null);
            // }
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_displayArea == null) return;

            if (_clearButton != null)
            {
                // TODO: 时间范围选择功能暂时注释掉，待后续实现
                // bool hasValue = false;
                // if (IsRange)
                // {
                //     hasValue = ValueRange != null && ValueRange.Length == 2 && ValueRange[0] != default && ValueRange[1] != default;
                // }
                // else
                // {
                    var hasValue = Value != null;
                // }
                _clearButton.Visibility = (Clearable && hasValue && !IsDisabled)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            // TODO: 时间范围选择功能暂时注释掉，待后续实现
            // if (IsRange)
            // {
            //     // 范围模式
            //     if (_rangePanel != null) _rangePanel.Visibility = Visibility.Visible;
            //     if (_displayText != null) _displayText.Visibility = Visibility.Collapsed;
            //     if (_placeholderText != null) _placeholderText.Visibility = Visibility.Collapsed;
            //
            //     if (_rangeSeparator != null)
            //     {
            //         _rangeSeparator.Text = RangeSeparator;
            //     }
            //
            //     bool hasStart = ValueRange != null && ValueRange.Length > 0 && ValueRange[0] != default;
            //     bool hasEnd = ValueRange != null && ValueRange.Length > 1 && ValueRange[1] != default;
            //
            //     if (hasStart)
            //     {
            //         if (_startPlaceholderText != null) _startPlaceholderText.Visibility = Visibility.Collapsed;
            //         if (_startDisplayText != null)
            //         {
            //             _startDisplayText.Text = FormatTime(ValueRange![0]);
            //             _startDisplayText.Visibility = Visibility.Visible;
            //         }
            //     }
            //     else
            //     {
            //         if (_startPlaceholderText != null) _startPlaceholderText.Visibility = Visibility.Visible;
            //         if (_startDisplayText != null) _startDisplayText.Visibility = Visibility.Collapsed;
            //     }
            //
            //     if (hasEnd)
            //     {
            //         if (_endPlaceholderText != null) _endPlaceholderText.Visibility = Visibility.Collapsed;
            //         if (_endDisplayText != null)
            //         {
            //             _endDisplayText.Text = FormatTime(ValueRange![1]);
            //             _endDisplayText.Visibility = Visibility.Visible;
            //         }
            //     }
            //     else
            //     {
            //         if (_endPlaceholderText != null) _endPlaceholderText.Visibility = Visibility.Visible;
            //         if (_endDisplayText != null) _endDisplayText.Visibility = Visibility.Collapsed;
            //     }
            // }
            // else
            // {
                // 单时间模式
                if (_rangePanel != null) _rangePanel.Visibility = Visibility.Collapsed;
                if (_startDisplayText != null) _startDisplayText.Visibility = Visibility.Collapsed;
                if (_endDisplayText != null) _endDisplayText.Visibility = Visibility.Collapsed;
                if (_startPlaceholderText != null) _startPlaceholderText.Visibility = Visibility.Collapsed;
                if (_endPlaceholderText != null) _endPlaceholderText.Visibility = Visibility.Collapsed;
                if (_rangeSeparator != null) _rangeSeparator.Visibility = Visibility.Collapsed;

                if (Value != null)
                {
                    if (_placeholderText != null) _placeholderText.Visibility = Visibility.Collapsed;
                    if (_displayText != null)
                    {
                        _displayText.Text = FormatTime(Value.Value);
                        _displayText.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (_placeholderText != null) _placeholderText.Visibility = Visibility.Visible;
                    if (_displayText != null) _displayText.Visibility = Visibility.Collapsed;
                }
            // }
        }

        private string FormatTime(DateTime time)
        {
            var format = ValueFormat ?? PickerOptions?.Format ?? "HH:mm:ss";
            return time.ToString(format);
        }

        private bool IsDescendantOf(DependencyObject? origin, DependencyObject? root)
        {
            if (origin == null || root == null) return false;
            var current = origin;
            while (current != null)
            {
                if (current == root) return true;
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(current);
                if (parent == null)
                {
                    parent = LogicalTreeHelper.GetParent(current);
                }
                current = parent;
            }
            return false;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 使 input 获取焦点
        /// </summary>
        public new void Focus()
        {
            if (_displayArea != null)
            {
                _displayArea.Focus();
                OnFocus?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}

