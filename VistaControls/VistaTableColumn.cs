using System.Windows;

namespace VistaControls
{
    public class VistaTableColumn : DependencyObject
    {
        public static readonly DependencyProperty PropProperty =
            DependencyProperty.Register(nameof(Prop), typeof(string), typeof(VistaTableColumn), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(VistaTableColumn), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(VistaTableColumn), new PropertyMetadata(double.NaN));

        public string Prop
        {
            get => (string)GetValue(PropProperty);
            set => SetValue(PropProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }
    }
}

