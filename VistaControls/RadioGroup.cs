using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace VistaControls
{
    /// <summary>
    /// RadioGroup - 单选框组控件，用于管理一组互斥的单选框
    /// </summary>
    public class RadioGroup : Panel
    {
        private readonly List<VistaRadio> _radios = new List<VistaRadio>();
        private object? _selectedValue;

        static RadioGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioGroup),
                new FrameworkPropertyMetadata(typeof(RadioGroup)));
        }

        public RadioGroup()
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double totalWidth = 0;
            double maxHeight = 0;
            int childCount = 0;
            
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                totalWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
                childCount++;
            }
            
            // 添加间距
            if (childCount > 1)
            {
                totalWidth += (childCount - 1) * 10; // 10px 间距
            }
            
            return new Size(totalWidth, maxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(x, 0, child.DesiredSize.Width, finalSize.Height));
                x += child.DesiredSize.Width + 10; // 10px 间距
            }
            return finalSize;
        }

        #region 依赖属性

        /// <summary>
        /// 当前选中的值
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(RadioGroup),
                new PropertyMetadata(null, OnSelectedValueChanged));

        /// <summary>
        /// 单选框组尺寸
        /// </summary>
        public static readonly DependencyProperty RadioSizeProperty =
            DependencyProperty.Register(nameof(RadioSize), typeof(RadioSize), typeof(RadioGroup),
                new PropertyMetadata(RadioSize.Default, OnRadioSizeChanged));

        /// <summary>
        /// 是否禁用
        /// </summary>
        public static readonly DependencyProperty IsGroupDisabledProperty =
            DependencyProperty.Register(nameof(IsGroupDisabled), typeof(bool), typeof(RadioGroup),
                new PropertyMetadata(false, OnIsGroupDisabledChanged));

        #endregion

        #region 属性

        public object? SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public RadioSize RadioSize
        {
            get => (RadioSize)GetValue(RadioSizeProperty);
            set => SetValue(RadioSizeProperty, value);
        }

        public bool IsGroupDisabled
        {
            get => (bool)GetValue(IsGroupDisabledProperty);
            set => SetValue(IsGroupDisabledProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 选中值改变事件
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;

        #endregion

        #region 内部方法

        internal void RegisterRadio(VistaRadio radio)
        {
            if (!_radios.Contains(radio))
            {
                _radios.Add(radio);
                
                // 如果组被禁用，禁用所有单选框
                if (IsGroupDisabled)
                {
                    radio.IsEnabled = false;
                }
                
                // 设置尺寸
                radio.RadioSize = RadioSize;
                
                // 检查是否应该被选中
                if (SelectedValue != null && object.Equals(radio.Label, SelectedValue))
                {
                    radio.IsChecked = true;
                }
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            
            if (visualAdded is VistaRadio radio)
            {
                RegisterRadio(radio);
            }
        }

        internal void OnRadioChecked(VistaRadio radio)
        {
            if (!radio.IsChecked == true)
                return;

            // 取消其他单选框的选中状态
            foreach (var r in _radios)
            {
                if (r != radio && r.IsChecked == true)
                {
                    r.IsChecked = false;
                }
            }

            // 更新选中值
            var newValue = radio.Label;
            if (!object.Equals(_selectedValue, newValue))
            {
                _selectedValue = newValue;
                SelectedValue = newValue;
                SelectionChanged?.Invoke(this, newValue);
            }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RadioGroup group)
            {
                group.UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            if (SelectedValue == null)
            {
                // 取消所有选中
                foreach (var radio in _radios)
                {
                    radio.IsChecked = false;
                }
                return;
            }

            // 根据 SelectedValue 选中对应的单选框
            foreach (var radio in _radios)
            {
                if (object.Equals(radio.Label, SelectedValue))
                {
                    radio.IsChecked = true;
                }
                else
                {
                    radio.IsChecked = false;
                }
            }
        }

        private static void OnRadioSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RadioGroup group)
            {
                // 更新所有子单选框的尺寸
                foreach (var radio in group._radios)
                {
                    radio.RadioSize = group.RadioSize;
                }
            }
        }

        private static void OnIsGroupDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RadioGroup group)
            {
                // 更新所有子单选框的禁用状态
                foreach (var radio in group._radios)
                {
                    radio.IsEnabled = !group.IsGroupDisabled;
                }
            }
        }

        #endregion
    }
}

