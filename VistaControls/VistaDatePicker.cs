using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace VistaControls
{
    /// <summary>
    /// DatePickerType - 日期选择器类型
    /// </summary>
    public enum DatePickerType
    {
        Date,      // 日
        Week,      // 周
        Month,     // 月
        Year,      // 年
        Dates,     // 多个日期
        Months,    // 多个月
        Years      // 多个年
    }

    /// <summary>
    /// VistaDatePicker - 日期选择器
    /// 用于选择或输入日期
    /// </summary>
    public class VistaDatePicker : Control
    {
        private Popup? _popup;
        private Border? _displayArea;
        private Button? _clearButton;
        private TextBlock? _displayText;
        private TextBlock? _placeholderText;

        // 日历面板
        private Grid? _calendarPanel;
        private Button? _prevMonthButton;
        private Button? _nextMonthButton;
        private TextBlock? _monthYearText;
        private UniformGrid? _weekdayHeader;
        private UniformGrid? _dayGrid;
        private StackPanel? _shortcutsPanel;
        private StackPanel? _monthGrid;
        private StackPanel? _yearGrid;

        // 当前显示的月份
        private DateTime _currentMonth = DateTime.Now;
        // 当前选择的值
        private DateTime? _selectedDate;
        private List<DateTime>? _selectedDates;
        private List<DateTime>? _selectedMonths;
        private List<DateTime>? _selectedYears;

        static VistaDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaDatePicker),
                new FrameworkPropertyMetadata(typeof(VistaDatePicker)));
        }

        public VistaDatePicker()
        {
            Loaded += VistaDatePicker_Loaded;
        }

        private void VistaDatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            if (Value == null && DefaultValue != null)
            {
                Value = DefaultValue;
            }

            UpdateDisplay();
            UpdateCalendar();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = GetTemplateChild("PART_Popup") as Popup;
            _displayArea = GetTemplateChild("PART_Display") as Border;
            _clearButton = GetTemplateChild("PART_Clear") as Button;
            _displayText = GetTemplateChild("displayText") as TextBlock;
            _placeholderText = GetTemplateChild("placeholder") as TextBlock;

            UpdateClearButtonVisibility();

            _calendarPanel = GetTemplateChild("calendarPanel") as Grid;
            _prevMonthButton = GetTemplateChild("prevMonthButton") as Button;
            _nextMonthButton = GetTemplateChild("nextMonthButton") as Button;
            _monthYearText = GetTemplateChild("monthYearText") as TextBlock;
            _weekdayHeader = GetTemplateChild("weekdayHeader") as UniformGrid;
            _dayGrid = GetTemplateChild("dayGrid") as UniformGrid;
            _shortcutsPanel = GetTemplateChild("shortcutsPanel") as StackPanel;
            _monthGrid = GetTemplateChild("monthGrid") as StackPanel;
            _yearGrid = GetTemplateChild("yearGrid") as StackPanel;

            if (_displayArea != null)
            {
                _displayArea.MouseLeftButtonDown += (_, __) => TogglePopup();
            }

            if (_clearButton != null)
            {
                _clearButton.Click += (_, __) => ClearSelection();
            }

            if (_prevMonthButton != null)
            {
                _prevMonthButton.Click += (_, __) => NavigateMonth(-1);
            }

            if (_nextMonthButton != null)
            {
                _nextMonthButton.Click += (_, __) => NavigateMonth(1);
            }

            // 全局点击关闭
            if (Application.Current?.MainWindow != null)
            {
                Application.Current.MainWindow.PreviewMouseDown -= GlobalPreviewMouseDown;
                Application.Current.MainWindow.PreviewMouseDown += GlobalPreviewMouseDown;
            }

            UpdateDisplay();
            UpdateCalendar();
        }

        private void GlobalPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_popup != null && _popup.IsOpen)
            {
                var source = e.OriginalSource as DependencyObject;
                // 如果点击发生在弹出层内部，则不关闭
                if (source != null)
                {
                    // 1) 鼠标在 Popup 上方
                    if (_popup.IsMouseOver)
                    {
                        return;
                    }

                    // 2) 点击源属于 Popup 的可视树
                    if (_popup.Child != null && IsDescendantOf(_popup.Child, source))
                    {
                        return;
                    }

                    // 3) 点击源属于控件本身（输入框区域）
                    if (IsDescendantOf(this, source))
                    {
                        return;
                    }

                    // 否则视为点击在控件外部，关闭 Popup
                    _popup.IsOpen = false;
                }
            }
        }

        private bool IsDescendantOf(DependencyObject ancestor, DependencyObject descendant)
        {
            var current = descendant;
            while (current != null)
            {
                if (current == ancestor) return true;
                current = VisualTreeHelper.GetParent(current);
            }
            return false;
        }

        #region 依赖属性

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(DateTime?), typeof(VistaDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty ValueDatesProperty =
            DependencyProperty.Register(nameof(ValueDates), typeof(List<DateTime>), typeof(VistaDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueDatesChanged));

        public static readonly DependencyProperty ValueMonthsProperty =
            DependencyProperty.Register(nameof(ValueMonths), typeof(List<DateTime>), typeof(VistaDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueMonthsChanged));

        public static readonly DependencyProperty ValueYearsProperty =
            DependencyProperty.Register(nameof(ValueYears), typeof(List<DateTime>), typeof(VistaDatePicker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueYearsChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaDatePicker),
                new PropertyMetadata("选择日期"));

        public static readonly DependencyProperty ReadonlyProperty =
            DependencyProperty.Register(nameof(Readonly), typeof(bool), typeof(VistaDatePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(VistaDatePicker),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ClearableProperty =
            DependencyProperty.Register(nameof(Clearable), typeof(bool), typeof(VistaDatePicker),
                new PropertyMetadata(true));

        public static readonly DependencyProperty InputSizeProperty =
            DependencyProperty.Register(nameof(InputSize), typeof(InputSize), typeof(VistaDatePicker),
                new PropertyMetadata(InputSize.Default));

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(DatePickerType), typeof(VistaDatePicker),
                new PropertyMetadata(DatePickerType.Date, OnTypeChanged));

        public static readonly DependencyProperty PickerOptionsProperty =
            DependencyProperty.Register(nameof(PickerOptions), typeof(DatePickerOptions), typeof(VistaDatePicker),
                new PropertyMetadata(null, OnPickerOptionsChanged));

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(DateTime?), typeof(VistaDatePicker),
                new PropertyMetadata(null));

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(VistaDatePicker),
                new PropertyMetadata("yyyy-MM-dd"));

        #endregion

        #region 属性

        public DateTime? Value
        {
            get => (DateTime?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public List<DateTime>? ValueDates
        {
            get => (List<DateTime>?)GetValue(ValueDatesProperty);
            set => SetValue(ValueDatesProperty, value);
        }

        public List<DateTime>? ValueMonths
        {
            get => (List<DateTime>?)GetValue(ValueMonthsProperty);
            set => SetValue(ValueMonthsProperty, value);
        }

        public List<DateTime>? ValueYears
        {
            get => (List<DateTime>?)GetValue(ValueYearsProperty);
            set => SetValue(ValueYearsProperty, value);
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

        public DatePickerType Type
        {
            get => (DatePickerType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public DatePickerOptions PickerOptions
        {
            get
            {
                var value = (DatePickerOptions?)GetValue(PickerOptionsProperty);
                if (value == null)
                {
                    value = new DatePickerOptions();
                    SetValue(PickerOptionsProperty, value);
                }
                return value;
            }
            set => SetValue(PickerOptionsProperty, value);
        }

        public DateTime? DefaultValue
        {
            get => (DateTime?)GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }

        public string Format
        {
            get => (string)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler<object?>? Change;
        public event EventHandler? Blur;
        public event EventHandler? OnFocus;

        #endregion

        #region 私有方法

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDatePicker picker)
            {
                // 同步内部状态
                // 使用 e.NewValue 和 e.OldValue 来判断值是否真正改变
                var newValue = (DateTime?)e.NewValue;
                var oldValue = (DateTime?)e.OldValue;
                
                // 只有当值真正改变时才处理
                // 注意：WPF 依赖属性在值相等时通常不会触发回调，但为了安全起见还是检查
                if (newValue != oldValue)
                {
                    // 总是同步 _selectedDate，确保状态一致性
                    // 即使 SelectDate 等方法已经设置了 _selectedDate，这里也要同步，以防外部直接设置 Value
                    picker._selectedDate = newValue;
                    
                    // 更新显示和日历
                    // 注意：SelectDate 等方法也会调用 UpdateDisplay，可能会有重复调用
                    // 但 UpdateDisplay 和 UpdateCalendar 是幂等的，重复调用不会有问题
                    // 使用同步调用确保立即更新（如果使用异步可能会延迟）
                    picker.UpdateDisplay();
                    picker.UpdateCalendar();
                }
            }
        }

        private static void OnValueDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDatePicker picker)
            {
                picker._selectedDates = picker.ValueDates;
                picker.UpdateDisplay();
                picker.UpdateCalendar();
            }
        }

        private static void OnValueMonthsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDatePicker picker)
            {
                picker._selectedMonths = picker.ValueMonths;
                picker.UpdateDisplay();
                picker.UpdateCalendar();
            }
        }

        private static void OnValueYearsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDatePicker picker)
            {
                picker._selectedYears = picker.ValueYears;
                picker.UpdateDisplay();
                picker.UpdateCalendar();
            }
        }

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDatePicker picker)
            {
                picker.UpdateDisplay();
                picker.UpdateCalendar();
            }
        }

        private static void OnPickerOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDatePicker picker)
            {
                picker.UpdateCalendar();
            }
        }

        private void TogglePopup()
        {
            if (IsDisabled || Readonly) return;
            if (_popup == null) return;

            _popup.IsOpen = !_popup.IsOpen;
            if (_popup.IsOpen)
            {
                if (_selectedDate.HasValue)
                {
                    _currentMonth = _selectedDate.Value;
                }
                else if (_selectedDates != null && _selectedDates.Count > 0)
                {
                    _currentMonth = _selectedDates[0];
                }
                else
                {
                    _currentMonth = DateTime.Now;
                }

                UpdateCalendar();
                OnFocus?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Blur?.Invoke(this, EventArgs.Empty);
            }
        }

        private void ClearSelection()
        {
            Value = null;
            ValueDates = null;
            ValueMonths = null;
            ValueYears = null;
            _selectedDate = null;
            _selectedDates = null;
            _selectedMonths = null;
            _selectedYears = null;
            UpdateDisplay();
            Change?.Invoke(this, null);
        }

        private void NavigateMonth(int direction)
        {
            _currentMonth = _currentMonth.AddMonths(direction);
            UpdateCalendar();
        }

        private void UpdateDisplay()
        {
            if (_displayText == null || _placeholderText == null) return;

            string displayValue = string.Empty;

            switch (Type)
            {
                case DatePickerType.Date:
                    if (_selectedDate.HasValue)
                    {
                        displayValue = _selectedDate.Value.ToString(Format);
                    }
                    break;
                case DatePickerType.Week:
                    if (_selectedDate.HasValue)
                    {
                        var week = GetWeekOfYear(_selectedDate.Value);
                        displayValue = Format.Replace("yyyy", _selectedDate.Value.Year.ToString())
                                             .Replace("WW", week.ToString("D2"));
                    }
                    break;
                case DatePickerType.Month:
                    if (_selectedDate.HasValue)
                    {
                        displayValue = _selectedDate.Value.ToString("yyyy-MM");
                    }
                    break;
                case DatePickerType.Year:
                    if (_selectedDate.HasValue)
                    {
                        displayValue = _selectedDate.Value.ToString("yyyy");
                    }
                    break;
                case DatePickerType.Dates:
                    if (_selectedDates != null && _selectedDates.Count > 0)
                    {
                        displayValue = string.Join(", ", _selectedDates.Select(d => d.ToString(Format)));
                    }
                    break;
                case DatePickerType.Months:
                    if (_selectedMonths != null && _selectedMonths.Count > 0)
                    {
                        displayValue = string.Join(", ", _selectedMonths.Select(d => d.ToString("yyyy-MM")));
                    }
                    break;
                case DatePickerType.Years:
                    if (_selectedYears != null && _selectedYears.Count > 0)
                    {
                        displayValue = string.Join(", ", _selectedYears.Select(d => d.ToString("yyyy")));
                    }
                    break;
            }

            _displayText.Text = displayValue;
            _displayText.Visibility = string.IsNullOrEmpty(displayValue) ? Visibility.Collapsed : Visibility.Visible;
            _placeholderText.Visibility = string.IsNullOrEmpty(displayValue) ? Visibility.Visible : Visibility.Collapsed;

            UpdateClearButtonVisibility();
        }

        private void UpdateClearButtonVisibility()
        {
            if (_clearButton == null) return;

            bool hasValue = false;
            switch (Type)
            {
                case DatePickerType.Date:
                case DatePickerType.Week:
                case DatePickerType.Month:
                case DatePickerType.Year:
                    hasValue = _selectedDate.HasValue;
                    break;
                case DatePickerType.Dates:
                    hasValue = _selectedDates != null && _selectedDates.Count > 0;
                    break;
                case DatePickerType.Months:
                    hasValue = _selectedMonths != null && _selectedMonths.Count > 0;
                    break;
                case DatePickerType.Years:
                    hasValue = _selectedYears != null && _selectedYears.Count > 0;
                    break;
            }

            _clearButton.Visibility = (Clearable && hasValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        private void UpdateCalendar()
        {
            if (_calendarPanel == null) return;

            // 根据类型显示不同的面板
            if (_dayGrid != null) _dayGrid.Visibility = Type == DatePickerType.Date || Type == DatePickerType.Dates || Type == DatePickerType.Week ? Visibility.Visible : Visibility.Collapsed;
            if (_monthGrid != null) _monthGrid.Visibility = Type == DatePickerType.Month || Type == DatePickerType.Months ? Visibility.Visible : Visibility.Collapsed;
            if (_yearGrid != null) _yearGrid.Visibility = Type == DatePickerType.Year || Type == DatePickerType.Years ? Visibility.Visible : Visibility.Collapsed;

            // 更新快捷选项
            UpdateShortcuts();

            // 根据类型更新不同的视图
            switch (Type)
            {
                case DatePickerType.Date:
                case DatePickerType.Dates:
                case DatePickerType.Week:
                    UpdateDayCalendar();
                    break;
                case DatePickerType.Month:
                case DatePickerType.Months:
                    UpdateMonthCalendar();
                    break;
                case DatePickerType.Year:
                case DatePickerType.Years:
                    UpdateYearCalendar();
                    break;
            }
        }

        private void UpdateShortcuts()
        {
            if (_shortcutsPanel == null) return;

            _shortcutsPanel.Children.Clear();

            if (PickerOptions?.Shortcuts == null || PickerOptions.Shortcuts.Count == 0)
            {
                _shortcutsPanel.Visibility = Visibility.Collapsed;
                return;
            }

            _shortcutsPanel.Visibility = Visibility.Visible;

            foreach (var shortcut in PickerOptions.Shortcuts)
            {
                var button = new Button
                {
                    Content = shortcut.Text,
                    Margin = new Thickness(0, 2, 0, 2),
                    Padding = new Thickness(8, 4, 8, 4),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromRgb(0x60, 0x62, 0x66)),
                    Cursor = Cursors.Hand
                };

                button.Click += (s, e) =>
                {
                    if (shortcut.OnClick != null)
                    {
                        var date = shortcut.OnClick();
                        SelectDate(date);
                        if (_popup != null) _popup.IsOpen = false;
                    }
                };

                _shortcutsPanel.Children.Add(button);
            }
        }

        private void UpdateDayCalendar()
        {
            if (_dayGrid == null || _monthYearText == null) return;

            _dayGrid.Children.Clear();

            // 更新月份年份显示
            _monthYearText.Text = _currentMonth.ToString("yyyy年MM月");

            // 获取月份的第一天和最后一天
            var firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // 获取第一天是星期几（周一=0）
            int firstDayOfWeek = ((int)firstDay.DayOfWeek + 6) % 7; // 转换为周一=0

            // 添加空白单元格
            for (int i = 0; i < firstDayOfWeek; i++)
            {
                _dayGrid.Children.Add(new Border { Background = Brushes.Transparent });
            }

            // 添加日期按钮
            for (int day = 1; day <= lastDay.Day; day++)
            {
                var date = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                var button = CreateDayButton(date);
                _dayGrid.Children.Add(button);
            }
        }

        private Button CreateDayButton(DateTime date)
        {
            bool isSelected = false;
            bool isToday = date.Date == DateTime.Today;

            switch (Type)
            {
                case DatePickerType.Date:
                    isSelected = _selectedDate.HasValue && _selectedDate.Value.Date == date.Date;
                    break;
                case DatePickerType.Dates:
                    isSelected = _selectedDates != null && _selectedDates.Any(d => d.Date == date.Date);
                    break;
                case DatePickerType.Week:
                    if (_selectedDate.HasValue)
                    {
                        var selectedWeek = GetWeekOfYear(_selectedDate.Value);
                        var currentWeek = GetWeekOfYear(date);
                        isSelected = selectedWeek == currentWeek && _selectedDate.Value.Year == date.Year;
                    }
                    break;
            }

            bool isDisabled = PickerOptions?.DisabledDate != null && PickerOptions.DisabledDate(date);

            var button = new Button
            {
                Content = date.Day.ToString(),
                Tag = date,
                Margin = new Thickness(2),
                Padding = new Thickness(4),
                Background = isSelected ? new SolidColorBrush(Color.FromRgb(0xEC, 0xF5, 0xFF)) : Brushes.Transparent,
                Foreground = isDisabled ? new SolidColorBrush(Color.FromRgb(0xC0, 0xC4, 0xCC)) :
                    isToday ? new SolidColorBrush(Color.FromRgb(0x40, 0x9E, 0xFF)) :
                    new SolidColorBrush(Color.FromRgb(0x60, 0x62, 0x66)),
                BorderThickness = new Thickness(0),
                Cursor = isDisabled ? Cursors.No : Cursors.Hand,
                IsEnabled = !isDisabled
            };

            if (!isDisabled)
            {
                button.Click += (s, e) => SelectDate(date);
            }

            return button;
        }

        private void UpdateMonthCalendar()
        {
            if (_monthGrid == null || _monthYearText == null) return;

            _monthGrid.Children.Clear();

            // 更新年份显示
            _monthYearText.Text = _currentMonth.Year.ToString() + "年";

            // 显示12个月
            for (int month = 1; month <= 12; month++)
            {
                var date = new DateTime(_currentMonth.Year, month, 1);
                bool isSelected = false;

                if (Type == DatePickerType.Month)
                {
                    isSelected = _selectedDate.HasValue && _selectedDate.Value.Year == date.Year && _selectedDate.Value.Month == date.Month;
                }
                else if (Type == DatePickerType.Months)
                {
                    isSelected = _selectedMonths != null && _selectedMonths.Any(d => d.Year == date.Year && d.Month == date.Month);
                }

                bool isDisabled = PickerOptions?.DisabledDate != null && PickerOptions.DisabledDate(date);

                var button = new Button
                {
                    Content = $"{month}月",
                    Tag = date,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4),
                    Background = isSelected ? new SolidColorBrush(Color.FromRgb(0xEC, 0xF5, 0xFF)) : Brushes.Transparent,
                    Foreground = isDisabled ? new SolidColorBrush(Color.FromRgb(0xC0, 0xC4, 0xCC)) :
                        new SolidColorBrush(Color.FromRgb(0x60, 0x62, 0x66)),
                    BorderThickness = new Thickness(0),
                    Cursor = isDisabled ? Cursors.No : Cursors.Hand,
                    IsEnabled = !isDisabled
                };

                if (!isDisabled)
                {
                    var monthValue = month;
                    button.Click += (s, e) => SelectMonth(monthValue);
                }

                _monthGrid.Children.Add(button);
            }
        }

        private void UpdateYearCalendar()
        {
            if (_yearGrid == null || _monthYearText == null) return;

            _yearGrid.Children.Clear();

            // 显示当前年份前后各5年（共11年）
            int startYear = _currentMonth.Year - 5;
            int endYear = _currentMonth.Year + 5;

            // 更新显示
            _monthYearText.Text = $"{startYear}-{endYear}";

            // 显示年份
            for (int year = startYear; year <= endYear; year++)
            {
                var date = new DateTime(year, 1, 1);
                bool isSelected = false;

                if (Type == DatePickerType.Year)
                {
                    isSelected = _selectedDate.HasValue && _selectedDate.Value.Year == year;
                }
                else if (Type == DatePickerType.Years)
                {
                    isSelected = _selectedYears != null && _selectedYears.Any(d => d.Year == year);
                }

                bool isDisabled = PickerOptions?.DisabledDate != null && PickerOptions.DisabledDate(date);

                var button = new Button
                {
                    Content = year.ToString(),
                    Tag = date,
                    Margin = new Thickness(4),
                    Padding = new Thickness(8, 4, 8, 4),
                    Background = isSelected ? new SolidColorBrush(Color.FromRgb(0xEC, 0xF5, 0xFF)) : Brushes.Transparent,
                    Foreground = isDisabled ? new SolidColorBrush(Color.FromRgb(0xC0, 0xC4, 0xCC)) :
                        new SolidColorBrush(Color.FromRgb(0x60, 0x62, 0x66)),
                    BorderThickness = new Thickness(0),
                    Cursor = isDisabled ? Cursors.No : Cursors.Hand,
                    IsEnabled = !isDisabled
                };

                if (!isDisabled)
                {
                    var yearValue = year;
                    button.Click += (s, e) => SelectYear(yearValue);
                }

                _yearGrid.Children.Add(button);
            }
        }

        private void SelectDate(DateTime date)
        {
            switch (Type)
            {
                case DatePickerType.Date:
                    // 只使用日期部分，忽略时间
                    var dateOnly = date.Date;
                    _selectedDate = dateOnly;
                    // 使用 SetCurrentValue 确保即使值相同也能触发 PropertyChangedCallback
                    // 或者先设置为 null 再设置新值，确保值改变
                    if (Value == dateOnly)
                    {
                        // 如果值相同，先设置为 null 再设置新值，确保触发回调
                        SetCurrentValue(ValueProperty, null);
                    }
                    SetCurrentValue(ValueProperty, dateOnly);
                    Change?.Invoke(this, dateOnly);
                    if (_popup != null) _popup.IsOpen = false;
                    UpdateDisplay();
                    UpdateCalendar();
                    break;
                case DatePickerType.Dates:
                    if (_selectedDates == null)
                    {
                        _selectedDates = new List<DateTime>();
                    }
                    if (_selectedDates.Contains(date.Date))
                    {
                        _selectedDates.Remove(date.Date);
                    }
                    else
                    {
                        _selectedDates.Add(date.Date);
                    }
                    ValueDates = new List<DateTime>(_selectedDates);
                    Change?.Invoke(this, ValueDates);
                    UpdateDisplay();
                    UpdateCalendar();
                    break;
                case DatePickerType.Week:
                    // 选择周：找到该周的第一天（周一）
                    var weekStart = date.AddDays(-((int)date.DayOfWeek + 6) % 7).Date;
                    _selectedDate = weekStart;
                    // 使用 SetCurrentValue 确保即使值相同也能触发 PropertyChangedCallback
                    if (Value == weekStart)
                    {
                        SetCurrentValue(ValueProperty, null);
                    }
                    SetCurrentValue(ValueProperty, weekStart);
                    Change?.Invoke(this, weekStart);
                    if (_popup != null) _popup.IsOpen = false;
                    UpdateDisplay();
                    UpdateCalendar();
                    break;
            }
        }

        private void SelectMonth(int month)
        {
            var date = new DateTime(_currentMonth.Year, month, 1);

            if (Type == DatePickerType.Month)
            {
                _selectedDate = date;
                // 使用 SetCurrentValue 确保即使值相同也能触发 PropertyChangedCallback
                if (Value == date)
                {
                    SetCurrentValue(ValueProperty, null);
                }
                SetCurrentValue(ValueProperty, date);
                Change?.Invoke(this, date);
                if (_popup != null) _popup.IsOpen = false;
                UpdateDisplay();
                UpdateCalendar();
            }
            else if (Type == DatePickerType.Months)
            {
                if (_selectedMonths == null)
                {
                    _selectedMonths = new List<DateTime>();
                }
                if (_selectedMonths.Any(d => d.Year == date.Year && d.Month == date.Month))
                {
                    _selectedMonths.RemoveAll(d => d.Year == date.Year && d.Month == date.Month);
                }
                else
                {
                    _selectedMonths.Add(date);
                }
                ValueMonths = new List<DateTime>(_selectedMonths);
                Change?.Invoke(this, ValueMonths);
                UpdateDisplay();
                UpdateCalendar();
            }
        }

        private void SelectYear(int year)
        {
            var date = new DateTime(year, 1, 1);

            if (Type == DatePickerType.Year)
            {
                _selectedDate = date;
                // 使用 SetCurrentValue 确保即使值相同也能触发 PropertyChangedCallback
                if (Value == date)
                {
                    SetCurrentValue(ValueProperty, null);
                }
                SetCurrentValue(ValueProperty, date);
                Change?.Invoke(this, date);
                if (_popup != null) _popup.IsOpen = false;
                UpdateDisplay();
                UpdateCalendar();
            }
            else if (Type == DatePickerType.Years)
            {
                if (_selectedYears == null)
                {
                    _selectedYears = new List<DateTime>();
                }
                if (_selectedYears.Any(d => d.Year == year))
                {
                    _selectedYears.RemoveAll(d => d.Year == year);
                }
                else
                {
                    _selectedYears.Add(date);
                }
                ValueYears = new List<DateTime>(_selectedYears);
                Change?.Invoke(this, ValueYears);
                UpdateDisplay();
                UpdateCalendar();
            }
        }

        #endregion
    }
}

