using System.Collections;
using System.Linq;
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
            
            // 监听 Items 集合变化
            this.Loaded += ButtonGroup_Loaded;
            this.ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
        }

        private void ButtonGroup_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtonPositions();
        }

        private void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e)
        {
            UpdateButtonPositions();
        }

        /// <summary>
        /// 更新按钮组中所有按钮的位置属性
        /// </summary>
        private void UpdateButtonPositions()
        {
            // 从可视化树中查找所有 VistaButton
            var buttons = FindVisualChildren<VistaButton>(this).ToList();

            if (buttons.Count == 0)
            {
                // 如果可视化树中没有找到，尝试从 Items 集合中获取
                buttons = this.Items.OfType<VistaButton>().ToList();
            }

            if (buttons.Count == 0)
                return;

            if (buttons.Count == 1)
            {
                buttons[0].ButtonGroupPosition = ButtonGroupPosition.Only;
            }
            else
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (i == 0)
                        buttons[i].ButtonGroupPosition = ButtonGroupPosition.First;
                    else if (i == buttons.Count - 1)
                        buttons[i].ButtonGroupPosition = ButtonGroupPosition.Last;
                    else
                        buttons[i].ButtonGroupPosition = ButtonGroupPosition.Middle;
                }
            }
        }

        /// <summary>
        /// 在可视化树中查找指定类型的子元素
        /// </summary>
        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject? depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject? child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    if (child != null)
                    {
                        foreach (T childOfChild in FindVisualChildren<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdateButtonPositions();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContentPresenter();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is VistaButton;
        }
    }
}

