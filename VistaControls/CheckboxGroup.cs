using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VistaControls
{
    /// <summary>
    /// CheckboxGroup - 复选框组控件，用于管理一组复选框
    /// </summary>
    public class CheckboxGroup : Panel
    {
        private readonly List<VistaCheckbox> _checkboxes = new List<VistaCheckbox>();
        private ObservableCollection<object>? _selectedValues;

        static CheckboxGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckboxGroup),
                new FrameworkPropertyMetadata(typeof(CheckboxGroup)));
        }

        public CheckboxGroup()
        {
            _selectedValues = new ObservableCollection<object>();
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
        /// 当前选中的值数组
        /// </summary>
        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.Register(nameof(SelectedValues), typeof(IList), typeof(CheckboxGroup),
                new PropertyMetadata(null, OnSelectedValuesChanged));

        /// <summary>
        /// 复选框组尺寸
        /// </summary>
        public static readonly DependencyProperty CheckboxSizeProperty =
            DependencyProperty.Register(nameof(CheckboxSize), typeof(CheckboxSize), typeof(CheckboxGroup),
                new PropertyMetadata(CheckboxSize.Default, OnCheckboxSizeChanged));

        /// <summary>
        /// 是否禁用
        /// </summary>
        public static readonly DependencyProperty IsGroupDisabledProperty =
            DependencyProperty.Register(nameof(IsGroupDisabled), typeof(bool), typeof(CheckboxGroup),
                new PropertyMetadata(false, OnIsGroupDisabledChanged));

        /// <summary>
        /// 可被勾选的 checkbox 的最小数量
        /// </summary>
        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register(nameof(Min), typeof(int), typeof(CheckboxGroup),
                new PropertyMetadata(0));

        /// <summary>
        /// 可被勾选的 checkbox 的最大数量
        /// </summary>
        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register(nameof(Max), typeof(int), typeof(CheckboxGroup),
                new PropertyMetadata(int.MaxValue));

        /// <summary>
        /// 按钮形式的 Checkbox 激活时的文本颜色
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(nameof(TextColor), typeof(string), typeof(CheckboxGroup),
                new PropertyMetadata("#ffffff"));

        /// <summary>
        /// 按钮形式的 Checkbox 激活时的填充色和边框色
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(string), typeof(CheckboxGroup),
                new PropertyMetadata("#409EFF"));

        #endregion

        #region 属性

        public IList? SelectedValues
        {
            get => (IList?)GetValue(SelectedValuesProperty);
            set => SetValue(SelectedValuesProperty, value);
        }

        public CheckboxSize CheckboxSize
        {
            get => (CheckboxSize)GetValue(CheckboxSizeProperty);
            set => SetValue(CheckboxSizeProperty, value);
        }

        public bool IsGroupDisabled
        {
            get => (bool)GetValue(IsGroupDisabledProperty);
            set => SetValue(IsGroupDisabledProperty, value);
        }

        public int Min
        {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        public int Max
        {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        public string TextColor
        {
            get => (string)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public string Fill
        {
            get => (string)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 选中值改变事件
        /// </summary>
        public event EventHandler<IList?>? SelectionChanged;

        #endregion

        #region 内部方法

        internal void RegisterCheckbox(VistaCheckbox checkbox)
        {
            if (!_checkboxes.Contains(checkbox))
            {
                _checkboxes.Add(checkbox);
                
                // 如果组被禁用，禁用所有复选框
                if (IsGroupDisabled)
                {
                    checkbox.IsEnabled = false;
                }
                
                // 设置尺寸
                checkbox.CheckboxSize = CheckboxSize;
                
                // 检查是否应该被选中
                if (SelectedValues != null)
                {
                    var label = checkbox.Label ?? checkbox.Content;
                    if (label != null)
                    {
                        bool shouldBeChecked = SelectedValues.Cast<object>().Any(v => object.Equals(v, label));
                        checkbox.IsChecked = shouldBeChecked;
                    }
                }
            }
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            
            if (visualAdded is VistaCheckbox checkbox)
            {
                RegisterCheckbox(checkbox);
            }
        }

        internal void OnCheckboxChecked(VistaCheckbox checkbox)
        {
            if (SelectedValues == null)
            {
                SelectedValues = new ObservableCollection<object>();
            }

            var label = checkbox.Label ?? checkbox.Content;
            if (label == null)
                return;

            // 检查是否已达到最大数量
            if (SelectedValues.Count >= Max)
            {
                // 如果已达到最大数量，取消当前选中
                checkbox.IsChecked = false;
                return;
            }

            // 添加选中值
            if (!SelectedValues.Cast<object>().Any(v => object.Equals(v, label)))
            {
                SelectedValues.Add(label);
                OnSelectionChanged();
            }
        }

        internal void OnCheckboxUnchecked(VistaCheckbox checkbox)
        {
            if (SelectedValues == null)
                return;

            var label = checkbox.Label ?? checkbox.Content;
            if (label == null)
                return;

            // 检查是否已达到最小数量（如果 Min > 0 且当前数量等于 Min，则不允许取消）
            if (Min > 0 && SelectedValues.Count <= Min)
            {
                // 如果已达到最小数量，重新选中
                checkbox.IsChecked = true;
                return;
            }

            // 移除选中值
            var itemToRemove = SelectedValues.Cast<object>().FirstOrDefault(v => object.Equals(v, label));
            if (itemToRemove != null)
            {
                SelectedValues.Remove(itemToRemove);
                OnSelectionChanged();
            }
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, SelectedValues);
        }

        private static void OnSelectedValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CheckboxGroup group)
            {
                // 如果 SelectedValues 为 null，创建一个新的 ObservableCollection
                if (group.SelectedValues == null)
                {
                    group.SelectedValues = new ObservableCollection<object>();
                }
                group.UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            if (SelectedValues == null)
            {
                // 取消所有选中
                foreach (var checkbox in _checkboxes)
                {
                    checkbox.IsChecked = false;
                }
                return;
            }

            // 根据 SelectedValues 选中对应的复选框
            foreach (var checkbox in _checkboxes)
            {
                var label = checkbox.Label ?? checkbox.Content;
                if (label != null)
                {
                    bool shouldBeChecked = SelectedValues.Cast<object>().Any(v => object.Equals(v, label));
                    checkbox.IsChecked = shouldBeChecked;
                }
            }
        }

        private static void OnCheckboxSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CheckboxGroup group)
            {
                // 更新所有子复选框的尺寸
                foreach (var checkbox in group._checkboxes)
                {
                    checkbox.CheckboxSize = group.CheckboxSize;
                }
            }
        }

        private static void OnIsGroupDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CheckboxGroup group)
            {
                // 更新所有子复选框的禁用状态
                foreach (var checkbox in group._checkboxes)
                {
                    checkbox.IsEnabled = !group.IsGroupDisabled;
                }
            }
        }

        #endregion
    }
}

