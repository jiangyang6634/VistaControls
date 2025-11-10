using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaInputNumber - 数字计数器输入框
    /// </summary>
    public class VistaInputNumber : TextBox
    {
        private Button? _btnIncrease;
        private Button? _btnDecrease;
        private bool _suppressTextChanged;

        static VistaInputNumber()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaInputNumber),
                new FrameworkPropertyMetadata(typeof(VistaInputNumber)));
        }

        public VistaInputNumber()
        {
            Text = FormatValue(Value);
            Loaded += (_, __) => SyncTextFromValue();
            PreviewTextInput += OnPreviewTextInputNumeric;
            PreviewKeyDown += OnPreviewKeyDownNumeric;
            DataObject.AddPastingHandler(this, OnPasteNumeric);
            TextChanged += OnTextChanged;
            LostFocus += (_, __) => CoerceAndFormat();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _btnIncrease = GetTemplateChild("PART_Increase") as Button;
            _btnDecrease = GetTemplateChild("PART_Decrease") as Button;
            if (_btnIncrease != null) _btnIncrease.Click += (_, __) => StepOnce(+1);
            if (_btnDecrease != null) _btnDecrease.Click += (_, __) => StepOnce(-1);
            UpdateButtonsEnabled();
        }

        #region 依赖属性

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(VistaInputNumber),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValueWithinRange));

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(double), typeof(VistaInputNumber),
                new PropertyMetadata(double.NegativeInfinity, OnRangeChanged));

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(double), typeof(VistaInputNumber),
                new PropertyMetadata(double.PositiveInfinity, OnRangeChanged));

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(double), typeof(VistaInputNumber),
                new PropertyMetadata(1d, OnStepChanged));

        public static readonly DependencyProperty StepStrictlyProperty =
            DependencyProperty.Register(nameof(StepStrictly), typeof(bool), typeof(VistaInputNumber),
                new PropertyMetadata(false));

        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register(nameof(Precision), typeof(int?), typeof(VistaInputNumber),
                new PropertyMetadata(null, OnPrecisionChanged));

        public static readonly DependencyProperty NumberSizeProperty =
            DependencyProperty.Register(nameof(NumberSize), typeof(InputSize), typeof(VistaInputNumber),
                new PropertyMetadata(InputSize.Default));

        public static readonly DependencyProperty ControlsProperty =
            DependencyProperty.Register(nameof(Controls), typeof(bool), typeof(VistaInputNumber),
                new PropertyMetadata(true));

        public static readonly DependencyProperty ControlsPositionProperty =
            DependencyProperty.Register(nameof(ControlsPosition), typeof(string), typeof(VistaInputNumber),
                new PropertyMetadata("default")); // "right" | "default"

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(VistaInputNumber),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaInputNumber),
                new PropertyMetadata(string.Empty));

        #endregion

        #region 属性

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public double Min
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        public bool StepStrictly
        {
            get => (bool)GetValue(StepStrictlyProperty);
            set => SetValue(StepStrictlyProperty, value);
        }

        public int? Precision
        {
            get => (int?)GetValue(PrecisionProperty);
            set => SetValue(PrecisionProperty, value);
        }

        public InputSize NumberSize
        {
            get => (InputSize)GetValue(NumberSizeProperty);
            set => SetValue(NumberSizeProperty, value);
        }

        public bool Controls
        {
            get => (bool)GetValue(ControlsProperty);
            set => SetValue(ControlsProperty, value);
        }

        public string ControlsPosition
        {
            get => (string)GetValue(ControlsPositionProperty);
            set => SetValue(ControlsPositionProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler<ValueChangedEventArgs>? Changed;

        #endregion

        #region 逻辑

        private static object CoerceValueWithinRange(DependencyObject d, object baseValue)
        {
            if (d is VistaInputNumber n && baseValue is double v)
            {
                var clamped = Math.Max(n.Min, Math.Min(n.Max, v));
                if (n.StepStrictly && n.Step > 0 && double.IsFinite(n.Min))
                {
                    // 只能是 step 的倍数（相对 Min 对齐）
                    var offset = v - n.Min;
                    var k = Math.Round(offset / n.Step);
                    clamped = n.Min + k * n.Step;
                }
                if (n.Precision is int p && p >= 0)
                {
                    clamped = RoundWithPrecision(clamped, ClampPrecision(p));
                }
                return clamped;
            }
            return baseValue;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInputNumber n)
            {
                n.SyncTextFromValue();
                n.UpdateButtonsEnabled();
                n.Changed?.Invoke(n, new ValueChangedEventArgs((double)e.OldValue, (double)e.NewValue));
            }
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInputNumber n)
            {
                n.Value = (double)CoerceValueWithinRange(n, n.Value);
                n.UpdateButtonsEnabled();
            }
        }

        private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInputNumber n)
            {
                if (n.Step <= 0) n.Step = 1;
                // precision 不得小于 step 的小数位数
                if (n.Precision is int p && p >= 0)
                {
                    var stepDecimals = Math.Min(15, GetDecimals(n.Step));
                    var target = ClampPrecision(Math.Max(p, stepDecimals));
                    if (target != p) n.Precision = target;
                }
            }
        }

        private static void OnPrecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInputNumber n)
            {
                if (n.Precision is int p)
                {
                    var clamped = ClampPrecision(p);
                    var stepDecimals = Math.Min(15, GetDecimals(n.Step));
                    var target = ClampPrecision(Math.Max(clamped, stepDecimals));
                    if (target != p)
                    {
                        n.Precision = target;
                        return;
                    }
                }
                n.Value = (double)CoerceValueWithinRange(n, n.Value);
            }
        }

        private void StepOnce(int dir)
        {
            if (!IsEnabled) return;
            var target = Value + dir * Step;
            Value = target; // Coerce handles clamping and precision
        }

        private void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (_suppressTextChanged) return;
            if (!IsFocused) return;
            if (TryParseText(Text, out var parsed))
            {
                var old = Value;
                var coerced = (double)CoerceValueWithinRange(this, parsed);
                if (!AreClose(old, coerced))
                {
                    Value = coerced;
                    // SyncTextFromValue will format
                }
            }
        }

        private void CoerceAndFormat()
        {
            var coerced = (double)CoerceValueWithinRange(this, Value);
            if (!AreClose(coerced, Value))
            {
                Value = coerced;
            }
            else
            {
                SyncTextFromValue();
            }
        }

        private void SyncTextFromValue()
        {
            _suppressTextChanged = true;
            try
            {
                Text = FormatValue(Value);
                CaretIndex = Text.Length;
            }
            finally
            {
                _suppressTextChanged = false;
            }
        }

        private void UpdateButtonsEnabled()
        {
            if (_btnIncrease != null) _btnIncrease.IsEnabled = IsEnabled && Value < Max;
            if (_btnDecrease != null) _btnDecrease.IsEnabled = IsEnabled && Value > Min;
        }

        private void OnPreviewTextInputNumeric(object sender, TextCompositionEventArgs e)
        {
            // 允许数字、小数点、负号
            var text = GetTextAfterInput(e.Text);
            e.Handled = !TryParseText(text, out _);
        }

        private void OnPreviewKeyDownNumeric(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                e.Handled = true;
                StepOnce(+1);
                return;
            }
            if (e.Key == Key.Down)
            {
                e.Handled = true;
                StepOnce(-1);
                return;
            }
        }

        private void OnPasteNumeric(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                var pasteText = e.DataObject.GetData(DataFormats.UnicodeText) as string ?? string.Empty;
                var text = GetTextAfterPaste(pasteText);
                if (!TryParseText(text, out _))
                {
                    e.CancelCommand();
                }
            }
        }

        private string GetTextAfterInput(string input)
        {
            var start = SelectionStart;
            var newText = (Text ?? string.Empty).Remove(start, SelectionLength).Insert(start, input);
            return newText;
        }

        private string GetTextAfterPaste(string paste)
        {
            var start = SelectionStart;
            var newText = (Text ?? string.Empty).Remove(start, SelectionLength).Insert(start, paste);
            return newText;
        }

        private static bool TryParseText(string? text, out double value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                value = 0;
                return true;
            }
            var style = NumberStyles.Float | NumberStyles.AllowLeadingSign;
            return double.TryParse(text, style, CultureInfo.InvariantCulture, out value)
                   || double.TryParse(text, style, CultureInfo.CurrentCulture, out value);
        }

        private string FormatValue(double v)
        {
            if (Precision is int p)
            {
                var pp = ClampPrecision(p);
                var rounded = RoundWithPrecision(v, pp);
                if (pp <= 0) return rounded.ToString(CultureInfo.CurrentCulture);
                // 动态格式：不补零
                var fmt = "0." + new string('#', pp);
                return rounded.ToString(fmt, CultureInfo.CurrentCulture);
            }
            return v.ToString(CultureInfo.CurrentCulture);
        }

        private static int GetDecimals(double x)
        {
            var s = x.ToString("G17", CultureInfo.InvariantCulture);
            var idx = s.IndexOf('.');
            var dec = idx >= 0 ? s.Length - idx - 1 : 0;
            return Math.Min(15, Math.Max(0, dec));
        }

        private static double RoundWithPrecision(double v, int p)
        {
            var pp = ClampPrecision(p);
            return Math.Round(v, pp, MidpointRounding.AwayFromZero);
        }

        private static bool AreClose(double a, double b)
        {
            return Math.Abs(a - b) <= 1e-12;
        }
        
        private static int ClampPrecision(int p)
        {
            if (p < 0) return 0;
            if (p > 15) return 15;
            return p;
        }

        #endregion

        #region 方法

        public new void Focus()
        {
            base.Focus();
        }

        public new void SelectAll()
        {
            base.SelectAll();
        }

        #endregion
    }

    public class ValueChangedEventArgs : EventArgs
    {
        public double OldValue { get; }
        public double NewValue { get; }
        public ValueChangedEventArgs(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}


