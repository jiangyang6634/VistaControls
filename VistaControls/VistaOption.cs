using System.Windows;
using System.Windows.Controls;

namespace VistaControls
{
    /// <summary>
    /// VistaOption - 下拉选项
    /// </summary>
    public class VistaOption : ContentControl
    {
        static VistaOption()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaOption),
                new FrameworkPropertyMetadata(typeof(VistaOption)));
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(VistaOption),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(VistaOption),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IsOptionDisabledProperty =
            DependencyProperty.Register(nameof(IsOptionDisabled), typeof(bool), typeof(VistaOption),
                new PropertyMetadata(false));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool IsOptionDisabled
        {
            get => (bool)GetValue(IsOptionDisabledProperty);
            set => SetValue(IsOptionDisabledProperty, value);
        }
    }
}


