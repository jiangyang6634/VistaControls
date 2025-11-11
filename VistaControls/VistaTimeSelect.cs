using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaTimeSelect - 固定时间点选择器
    /// 用于选择固定的时间点（通过 start、end、step 指定可选时间）
    /// </summary>
    public class VistaTimeSelect : Control
    {
        private Popup? _popup;
        private Border? _displayArea;
        private Button? _clearButton;
        private TextBlock? _displayText;
        private TextBlock? _placeholderText;
        private ScrollViewer? _timeListScrollViewer;
        private StackPanel? _timeListPanel;

        static VistaTimeSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTimeSelect),
                new FrameworkPropertyMetadata(typeof(VistaTimeSelect)));
        }

        public VistaTimeSelect()
        {
            Loaded += VistaTimeSelect_Loaded;
            _timeOptions = new ObservableCollection<string>();
        }

        private void VistaTimeSelect_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateTimeOptions();
            UpdateDisplay();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild("PART_Popup") as Popup;
            _displayArea = GetTemplateChild("PART_Display") as Border;
            _clearButton = GetTemplateChild("PART_Clear") as Button;
            _displayText = GetTemplateChild("displayText") as TextBlock;
            _placeholderText = GetTemplateChild("placeholder") as TextBlock;
            _timeListScrollViewer = GetTemplateChild("timeListScrollViewer") as ScrollViewer;
            _timeListPanel = GetTemplateChild("timeListPanel") as StackPanel;

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

            // 只在首次或配置改变时生成选项列表
            if (_timeOptions.Count == 0)
            {
                GenerateTimeOptions();
            }
            else
            {
                UpdateTimeList();
            }
            UpdateDisplay();
        }

        #region 依赖属性

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(VistaTimeSelect),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaTimeSelect),
                new PropertyMetadata("选择时间"));

        public static readonly DependencyProperty ReadonlyProperty =
            DependencyProperty.Register(nameof(Readonly), typeof(bool), typeof(VistaTimeSelect),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(VistaTimeSelect),
                new PropertyMetadata(false));

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(VistaTimeSelect),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ClearableProperty =
            DependencyProperty.Register(nameof(Clearable), typeof(bool), typeof(VistaTimeSelect),
                new PropertyMetadata(true));

        public static readonly DependencyProperty InputSizeProperty =
            DependencyProperty.Register(nameof(InputSize), typeof(InputSize), typeof(VistaTimeSelect),
                new PropertyMetadata(InputSize.Default));

        public static readonly DependencyProperty PickerOptionsProperty =
            DependencyProperty.Register(nameof(PickerOptions), typeof(TimeSelectOptions), typeof(VistaTimeSelect),
                new PropertyMetadata(new TimeSelectOptions(), OnPickerOptionsChanged));

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(string), typeof(VistaTimeSelect),
                new PropertyMetadata(null));

        #endregion

        #region 属性

        public string? Value
        {
            get => (string?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
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

        public TimeSelectOptions PickerOptions
        {
            get => (TimeSelectOptions)GetValue(PickerOptionsProperty);
            set => SetValue(PickerOptionsProperty, value);
        }

        public string? DefaultValue
        {
            get => (string?)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler<string?>? Change;
        public event EventHandler? Blur;
        public event EventHandler? OnFocus;

        #endregion

        #region 私有字段和方法

        private readonly ObservableCollection<string> _timeOptions;

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTimeSelect select)
            {
                select.UpdateDisplay();
            }
        }

        private static void OnPickerOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTimeSelect select)
            {
                select.GenerateTimeOptions();
                // UpdateTimeList 会在 GenerateTimeOptions 中自动调用（如果面板已初始化）
            }
        }

        private void GenerateTimeOptions()
        {
            _timeOptions.Clear();

            try
            {
                var start = ParseTime(PickerOptions.Start);
                var end = ParseTime(PickerOptions.End);
                var step = ParseTimeSpan(PickerOptions.Step);
                var minTime = PickerOptions.MinTime != null ? ParseTime(PickerOptions.MinTime) : (TimeSpan?)null;
                var maxTime = PickerOptions.MaxTime != null ? ParseTime(PickerOptions.MaxTime) : (TimeSpan?)null;

                var current = start;
                while (current <= end)
                {
                    // 检查是否在允许范围内
                    if (minTime.HasValue && current < minTime.Value)
                    {
                        current = current.Add(step);
                        continue;
                    }

                    if (maxTime.HasValue && current > maxTime.Value)
                    {
                        break;
                    }

                    _timeOptions.Add(FormatTime(current));
                    current = current.Add(step);
                }
            }
            catch
            {
                // 如果解析失败，使用默认值
                _timeOptions.Clear();
            }

            // 生成选项后立即更新列表（如果面板已初始化）
            if (_timeListPanel != null)
            {
                UpdateTimeList();
            }
        }

        private TimeSpan ParseTime(string timeStr)
        {
            var parts = timeStr.Split(':');
            if (parts.Length >= 2)
            {
                var hours = int.Parse(parts[0]);
                var minutes = int.Parse(parts[1]);
                return new TimeSpan(hours, minutes, 0);
            }
            throw new ArgumentException("Invalid time format");
        }

        private TimeSpan ParseTimeSpan(string timeStr)
        {
            var parts = timeStr.Split(':');
            if (parts.Length >= 2)
            {
                var hours = int.Parse(parts[0]);
                var minutes = int.Parse(parts[1]);
                return new TimeSpan(hours, minutes, 0);
            }
            throw new ArgumentException("Invalid time format");
        }

        private string FormatTime(TimeSpan time)
        {
            return $"{time.Hours:D2}:{time.Minutes:D2}";
        }

        private void UpdateTimeList()
        {
            if (_timeListPanel == null) return;

            // 清空现有按钮（必须在UI线程）
            _timeListPanel.Children.Clear();

            // 复制时间选项列表（避免在异步过程中被修改）
            var timeOptionsCopy = new List<string>(_timeOptions);
            var currentValue = Value; // 保存当前值用于高亮

            // 异步生成按钮列表，使用 async/await 模式，不阻塞UI线程
            _ = UpdateTimeListAsync(timeOptionsCopy, currentValue);
        }

        private async Task UpdateTimeListAsync(List<string> timeOptionsCopy, string currentValue)
        {
            // 在后台线程准备按钮数据
            var buttonData = await Task.Run(() =>
            {
                var data = new List<(string Time, bool IsSelected)>();
                foreach (var timeOption in timeOptionsCopy)
                {
                    data.Add((timeOption, timeOption == currentValue));
                }
                return data;
            });

            // 回到UI线程更新UI（await 会自动回到调用线程，即UI线程）
            // 但为了确保，我们显式使用 Dispatcher，并使用 Background 优先级，不阻塞其他UI操作
            await Dispatcher.InvokeAsync(() =>
            {
                // 检查控件是否还存在（防止在异步过程中控件被销毁）
                if (_timeListPanel == null)
                    return;

                // 清空现有按钮
                _timeListPanel.Children.Clear();

                // 添加按钮
                foreach (var (time, isSelected) in buttonData)
                {
                    var timeValue = time; // 避免闭包捕获问题
                    var button = new Button
                    {
                        Content = timeValue,
                        Tag = timeValue,
                        Margin = new Thickness(0, 2, 0, 2),
                        Padding = new Thickness(8, 4, 8, 4),
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Background = isSelected ?
                            new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xEC, 0xF5, 0xFF)) :
                            System.Windows.Media.Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x60, 0x62, 0x66)),
                        Cursor = Cursors.Hand
                    };

                    // 尝试从资源中获取样式
                    try
                    {
                        var style = FindResource("TimeOptionButtonStyle") as Style;
                        if (style != null)
                        {
                            button.Style = style;
                        }
                    }
                    catch
                    {
                        // 如果找不到样式,使用默认样式
                    }

                    button.Click += (s, e) =>
                    {
                        if (s is Button btn && btn.Tag is string timeStr)
                        {
                            Value = timeStr;
                            Change?.Invoke(this, timeStr);
                            UpdateDisplay();
                            if (_popup != null) _popup.IsOpen = false;
                        }
                    };

                    _timeListPanel.Children.Add(button);
                }
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private void TogglePopup()
        {
            if (IsDisabled || Readonly) return;
            if (_popup == null) return;
            _popup.IsOpen = !_popup.IsOpen;
            if (_popup.IsOpen)
            {
                // 只在列表为空或需要更新选中状态时才更新
                if (_timeListPanel == null || _timeListPanel.Children.Count == 0)
                {
                    UpdateTimeList();
                }
                else
                {
                    // 只更新选中状态的高亮
                    UpdateSelectedHighlight();
                }
                OnFocus?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UpdateSelectedHighlight()
        {
            if (_timeListPanel == null) return;
            foreach (var child in _timeListPanel.Children)
            {
                if (child is Button btn && btn.Tag is string time)
                {
                    if (time == Value)
                    {
                        btn.Background = new System.Windows.Media.SolidColorBrush(
                            System.Windows.Media.Color.FromRgb(0xEC, 0xF5, 0xFF));
                    }
                    else
                    {
                        btn.Background = System.Windows.Media.Brushes.Transparent;
                    }
                }
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
            Value = null;
            Change?.Invoke(this, null);
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_displayArea == null) return;

            if (_clearButton != null)
            {
                _clearButton.Visibility = (Clearable && !string.IsNullOrEmpty(Value) && !IsDisabled)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            if (string.IsNullOrEmpty(Value))
            {
                if (_placeholderText != null) _placeholderText.Visibility = Visibility.Visible;
                if (_displayText != null) _displayText.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (_placeholderText != null) _placeholderText.Visibility = Visibility.Collapsed;
                if (_displayText != null)
                {
                    _displayText.Text = Value;
                    _displayText.Visibility = Visibility.Visible;
                }
            }
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

