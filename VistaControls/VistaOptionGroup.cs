using System.Windows;
using System.Windows.Controls;

namespace VistaControls
{
    /// <summary>
    /// VistaOptionGroup - 选项分组
    /// </summary>
    public class VistaOptionGroup : HeaderedItemsControl
    {
        static VistaOptionGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaOptionGroup),
                new FrameworkPropertyMetadata(typeof(VistaOptionGroup)));
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(VistaOptionGroup),
                new PropertyMetadata(string.Empty));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
    }
}


