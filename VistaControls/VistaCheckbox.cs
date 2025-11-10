using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaCheckbox - 复选框控件
    /// </summary>
    public class VistaCheckbox : CheckBox
    {
        private CheckboxGroup? _parentGroup;

        static VistaCheckbox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaCheckbox),
                new FrameworkPropertyMetadata(typeof(VistaCheckbox)));
        }

        public VistaCheckbox()
        {
            Checked += VistaCheckbox_Checked;
            Unchecked += VistaCheckbox_Unchecked;
            Indeterminate += VistaCheckbox_Indeterminate;
            Loaded += VistaCheckbox_Loaded;
        }

        #region 依赖属性

        /// <summary>
        /// Checkbox 的值（label），在 checkbox-group 中使用
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(object), typeof(VistaCheckbox),
                new PropertyMetadata(null));

        /// <summary>
        /// 选中时的值（用于 true-label）
        /// </summary>
        public static readonly DependencyProperty TrueLabelProperty =
            DependencyProperty.Register(nameof(TrueLabel), typeof(object), typeof(VistaCheckbox),
                new PropertyMetadata(null));

        /// <summary>
        /// 未选中时的值（用于 false-label）
        /// </summary>
        public static readonly DependencyProperty FalseLabelProperty =
            DependencyProperty.Register(nameof(FalseLabel), typeof(object), typeof(VistaCheckbox),
                new PropertyMetadata(null));

        /// <summary>
        /// 是否显示边框
        /// </summary>
        public static readonly DependencyProperty BorderProperty =
            DependencyProperty.Register(nameof(Border), typeof(bool), typeof(VistaCheckbox),
                new PropertyMetadata(false));

        /// <summary>
        /// Checkbox 的尺寸：medium, small, mini
        /// </summary>
        public static readonly DependencyProperty CheckboxSizeProperty =
            DependencyProperty.Register(nameof(CheckboxSize), typeof(CheckboxSize), typeof(VistaCheckbox),
                new PropertyMetadata(CheckboxSize.Default));


        /// <summary>
        /// 设置 indeterminate 状态，只负责样式控制
        /// </summary>
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register(nameof(IsIndeterminate), typeof(bool), typeof(VistaCheckbox),
                new PropertyMetadata(false, OnIsIndeterminateChanged));

        #endregion

        #region 属性

        public object Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public object TrueLabel
        {
            get => GetValue(TrueLabelProperty);
            set => SetValue(TrueLabelProperty, value);
        }

        public object FalseLabel
        {
            get => GetValue(FalseLabelProperty);
            set => SetValue(FalseLabelProperty, value);
        }

        public bool Border
        {
            get => (bool)GetValue(BorderProperty);
            set => SetValue(BorderProperty, value);
        }

        public CheckboxSize CheckboxSize
        {
            get => (CheckboxSize)GetValue(CheckboxSizeProperty);
            set => SetValue(CheckboxSizeProperty, value);
        }

        public bool IsIndeterminate
        {
            get => (bool)GetValue(IsIndeterminateProperty);
            set => SetValue(IsIndeterminateProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 当绑定值变化时触发的事件
        /// </summary>
        public event EventHandler<object?>? CheckedChanged;

        #endregion

        #region 事件处理

        private void VistaCheckbox_Loaded(object sender, RoutedEventArgs e)
        {
            // 查找父 CheckboxGroup
            FindParentGroup();
            
            // 如果设置了 IsIndeterminate，更新状态
            if (IsIndeterminate)
            {
                this.IsChecked = null;
            }
        }

        private void VistaCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (IsIndeterminate)
            {
                IsIndeterminate = false;
            }

            if (_parentGroup != null)
            {
                // 通知父组当前项被选中
                _parentGroup.OnCheckboxChecked(this);
            }
            else
            {
                // 单独使用时触发事件
                OnCheckedChanged();
            }
        }

        private void VistaCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (IsIndeterminate)
            {
                IsIndeterminate = false;
            }

            if (_parentGroup != null)
            {
                // 通知父组当前项被取消选中
                _parentGroup.OnCheckboxUnchecked(this);
            }
            else
            {
                // 单独使用时触发事件
                OnCheckedChanged();
            }
        }

        private void VistaCheckbox_Indeterminate(object sender, RoutedEventArgs e)
        {
            // Indeterminate 状态处理
        }

        private static void OnIsIndeterminateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaCheckbox checkbox)
            {
                if ((bool)e.NewValue)
                {
                    checkbox.IsChecked = null;
                }
            }
        }

        private void FindParentGroup()
        {
            var parent = LogicalTreeHelper.GetParent(this);
            while (parent != null)
            {
                if (parent is CheckboxGroup group)
                {
                    _parentGroup = group;
                    group.RegisterCheckbox(this);
                    break;
                }
                parent = LogicalTreeHelper.GetParent(parent);
            }
        }

        private void OnCheckedChanged()
        {
            object? value = null;
            if (IsChecked == true)
            {
                value = TrueLabel ?? true;
            }
            else if (IsChecked == false)
            {
                value = FalseLabel ?? false;
            }
            CheckedChanged?.Invoke(this, value);
        }

        #endregion

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
    /// Checkbox 尺寸枚举
    /// </summary>
    public enum CheckboxSize
    {
        Default,
        Medium,
        Small,
        Mini
    }
}

