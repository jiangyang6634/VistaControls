using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaRadio - 单选框控件
    /// </summary>
    public class VistaRadio : CheckBox
    {
        private RadioGroup? _parentGroup;

        static VistaRadio()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaRadio),
                new FrameworkPropertyMetadata(typeof(VistaRadio)));
        }

        public VistaRadio()
        {
            Checked += VistaRadio_Checked;
            Unchecked += VistaRadio_Unchecked;
            Loaded += VistaRadio_Loaded;
        }

        #region 依赖属性

        /// <summary>
        /// Radio 的值（label）
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(object), typeof(VistaRadio),
                new PropertyMetadata(null));

        /// <summary>
        /// 是否显示边框
        /// </summary>
        public static readonly DependencyProperty BorderProperty =
            DependencyProperty.Register(nameof(Border), typeof(bool), typeof(VistaRadio),
                new PropertyMetadata(false));

        /// <summary>
        /// Radio 的尺寸：medium, small, mini
        /// </summary>
        public static readonly DependencyProperty RadioSizeProperty =
            DependencyProperty.Register(nameof(RadioSize), typeof(RadioSize), typeof(VistaRadio),
                new PropertyMetadata(RadioSize.Default));

        /// <summary>
        /// 分组名称，同组的单选框互斥
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(VistaRadio),
                new PropertyMetadata(string.Empty));

        #endregion

        #region 属性

        public object Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public bool Border
        {
            get => (bool)GetValue(BorderProperty);
            set => SetValue(BorderProperty, value);
        }

        public RadioSize RadioSize
        {
            get => (RadioSize)GetValue(RadioSizeProperty);
            set => SetValue(RadioSizeProperty, value);
        }

        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 选中状态改变事件
        /// </summary>
        public event EventHandler<object?>? Change;

        #endregion

        #region 事件处理

        private void VistaRadio_Loaded(object sender, RoutedEventArgs e)
        {
            // 查找父 RadioGroup
            FindParentGroup();
        }

        private void VistaRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_parentGroup != null)
            {
                // 通知父组当前项被选中
                _parentGroup.OnRadioChecked(this);
            }
            else
            {
                // 如果不在 RadioGroup 中，取消同组其他单选框的选中状态
                UncheckOtherRadiosInGroup();
            }

            // 触发 Change 事件
            Change?.Invoke(this, Label);
        }

        private void UncheckOtherRadiosInGroup()
        {
            // 查找窗口根元素
            var root = GetRootElement();
            if (root == null)
                return;

            // 遍历整个逻辑树，查找同组的其他单选框
            UncheckRadiosInElement(root);
        }

        private FrameworkElement? GetRootElement()
        {
            var current = this as DependencyObject;
            while (current != null)
            {
                var parent = LogicalTreeHelper.GetParent(current);
                if (parent == null)
                {
                    // 找到根元素（通常是 Window）
                    return current as FrameworkElement;
                }
                current = parent;
            }
            return null;
        }

        private void UncheckRadiosInElement(DependencyObject element)
        {
            if (element == null)
                return;

            // 检查当前元素是否是 VistaRadio
            if (element is VistaRadio radio && radio != this)
            {
                // 检查是否在同一组
                bool isSameGroup = false;
                
                if (string.IsNullOrEmpty(GroupName) && string.IsNullOrEmpty(radio.GroupName))
                {
                    // 默认组：都为空时互斥（但需要检查是否在同一父容器中）
                    var thisParent = LogicalTreeHelper.GetParent(this);
                    var radioParent = LogicalTreeHelper.GetParent(radio);
                    
                    // 如果它们有相同的直接父容器，则互斥
                    if (thisParent != null && thisParent == radioParent)
                    {
                        isSameGroup = true;
                    }
                }
                else if (!string.IsNullOrEmpty(GroupName) && radio.GroupName == GroupName)
                {
                    // 同组：GroupName 相同时互斥
                    isSameGroup = true;
                }

                if (isSameGroup && radio.IsChecked == true)
                {
                    radio.IsChecked = false;
                }
            }

            // 递归查找子元素
            foreach (var child in LogicalTreeHelper.GetChildren(element))
            {
                if (child is DependencyObject childDO)
                {
                    UncheckRadiosInElement(childDO);
                }
            }
        }

        private void VistaRadio_Unchecked(object sender, RoutedEventArgs e)
        {
            // 单选框组中，取消选中通常由组管理
        }

        private void FindParentGroup()
        {
            var parent = LogicalTreeHelper.GetParent(this);
            while (parent != null)
            {
                if (parent is RadioGroup group)
                {
                    _parentGroup = group;
                    group.RegisterRadio(this);
                    break;
                }
                parent = LogicalTreeHelper.GetParent(parent);
            }
        }

        #endregion

        protected override void OnClick()
        {
            // 单选框特性：已选中的单选框不能通过再次点击取消选中
            if (IsChecked == true)
            {
                // 已经选中，不做任何操作
                return;
            }

            // 未选中，执行选中操作
            base.OnClick();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsEnabled)
            {
                e.Handled = true;
                return;
            }

            base.OnMouseLeftButtonDown(e);
        }
    }

    /// <summary>
    /// Radio 尺寸枚举
    /// </summary>
    public enum RadioSize
    {
        Default,
        Medium,
        Small,
        Mini
    }
}

