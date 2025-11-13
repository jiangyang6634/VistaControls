using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace VistaControls
{
    public enum BadgeType
    {
        Default,
        Primary,
        Success,
        Warning,
        Danger,
        Info
    }

    public class VistaBadge : ContentControl
    {
        private static readonly DependencyPropertyKey DisplayTextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(DisplayText), typeof(string), typeof(VistaBadge),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey IsBadgeVisiblePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsBadgeVisible), typeof(bool), typeof(VistaBadge),
                new PropertyMetadata(false));

        public static readonly DependencyProperty IsBadgeVisibleProperty = IsBadgeVisiblePropertyKey.DependencyProperty;

        static VistaBadge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaBadge),
                new FrameworkPropertyMetadata(typeof(VistaBadge)));
        }

        public VistaBadge()
        {
            Loaded += (_, __) => UpdateDisplayText();
        }

        public object? Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(VistaBadge),
                new PropertyMetadata(null, OnBadgePropertyChanged));

        public int? Max
        {
            get => (int?)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(int?), typeof(VistaBadge),
                new PropertyMetadata(null, OnBadgePropertyChanged));

        public bool IsDot
        {
            get => (bool)GetValue(IsDotProperty);
            set => SetValue(IsDotProperty, value);
        }

        public static readonly DependencyProperty IsDotProperty =
            DependencyProperty.Register(nameof(IsDot), typeof(bool), typeof(VistaBadge),
                new PropertyMetadata(false, OnBadgePropertyChanged));

        public bool Hidden
        {
            get => (bool)GetValue(HiddenProperty);
            set => SetValue(HiddenProperty, value);
        }

        public static readonly DependencyProperty HiddenProperty =
            DependencyProperty.Register(nameof(Hidden), typeof(bool), typeof(VistaBadge),
                new PropertyMetadata(false, OnBadgePropertyChanged));

        public BadgeType Type
        {
            get => (BadgeType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(BadgeType), typeof(VistaBadge),
                new PropertyMetadata(BadgeType.Danger));

        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            private set => SetValue(DisplayTextPropertyKey, value);
        }

        public bool IsBadgeVisible
        {
            get => (bool)GetValue(IsBadgeVisibleProperty);
            private set => SetValue(IsBadgeVisiblePropertyKey, value);
        }

        private static void OnBadgePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaBadge badge)
            {
                badge.UpdateDisplayText();
            }
        }

        private void UpdateDisplayText()
        {
            if (IsDot)
            {
                DisplayText = string.Empty;
                UpdateBadgeVisibility();
                return;
            }

            if (Value == null)
            {
                DisplayText = string.Empty;
                UpdateBadgeVisibility();
                return;
            }

            if (Value is string strValue)
            {
                DisplayText = strValue;
                UpdateBadgeVisibility();
                return;
            }

            if (Value is IConvertible convertible)
            {
                try
                {
                    var number = convertible.ToDecimal(CultureInfo.InvariantCulture);
                    if (Max.HasValue && number > Max.Value)
                    {
                        DisplayText = $"{Max.Value}+";
                    }
                    else
                    {
                        if (decimal.Truncate(number) == number)
                        {
                            DisplayText = decimal.ToInt64(number).ToString(CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            DisplayText = number.ToString("0.##", CultureInfo.InvariantCulture);
                        }
                    }
                    UpdateBadgeVisibility();
                    return;
                }
                catch
                {
                    // Ignore conversion failure and fallback to ToString
                }
            }

            DisplayText = Value?.ToString() ?? string.Empty;
            UpdateBadgeVisibility();
        }

        private void UpdateBadgeVisibility()
        {
            var hasContent = IsDot || !string.IsNullOrEmpty(DisplayText);
            IsBadgeVisible = !Hidden && hasContent;
        }
    }
}
