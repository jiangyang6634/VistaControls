using System.Windows;
using System.Windows.Controls;

namespace VistaControls
{
    /// <summary>
    /// ButtonGroup - 按钮组控件，用于将多个按钮组合在一起
    /// </summary>
    public class ButtonGroup : ItemsControl
    {
        static ButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonGroup),
                new FrameworkPropertyMetadata(typeof(ButtonGroup)));
        }

        public ButtonGroup()
        {
            // 默认使用 StackPanel 作为 ItemsPanel
            ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
            ((FrameworkElementFactory)ItemsPanel.VisualTree).SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
        }
    }
}

