using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaSelect - 下拉选择器（单选/多选）
    /// </summary>
    public class VistaSelect : ItemsControl
    {
        private Popup? _popup;
        private Border? _displayArea;
        private Button? _clearButton;
        private TextBlock? _singleText;
        private TextBlock? _placeholderText;
        private ItemsControl? _tagList;
        private TextBlock? _collapsedText;

        static VistaSelect()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaSelect),
                new FrameworkPropertyMetadata(typeof(VistaSelect)));
        }

        public VistaSelect()
        {
            Loaded += (_, __) => UpdateDisplay();
            AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDownInternal), true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = GetTemplateChild("PART_Popup") as Popup;
            _displayArea = GetTemplateChild("PART_Display") as Border;
            _clearButton = GetTemplateChild("PART_Clear") as Button;
            _singleText = GetTemplateChild("singleText") as TextBlock;
            _placeholderText = GetTemplateChild("placeholder") as TextBlock;
            _tagList = GetTemplateChild("tagList") as ItemsControl;
            _collapsedText = GetTemplateChild("collapsedText") as TextBlock;
            if (_displayArea != null)
            {
                _displayArea.MouseLeftButtonDown += (_, __) => TogglePopup();
            }
            if (_clearButton != null)
            {
                _clearButton.Click += (_, __) => ClearSelection();
            }
            // 全局点击关闭
            if (Application.Current?.MainWindow != null)
            {
                Application.Current.MainWindow.PreviewMouseDown -= GlobalPreviewMouseDown;
                Application.Current.MainWindow.PreviewMouseDown += GlobalPreviewMouseDown;
            }
            UpdateDisplay();
        }

        #region 依赖属性

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaSelect),
                new PropertyMetadata("请选择"));

        public static readonly DependencyProperty IsSelectDisabledProperty =
            DependencyProperty.Register(nameof(IsSelectDisabled), typeof(bool), typeof(VistaSelect),
                new PropertyMetadata(false, OnDisabledChanged));

        public static readonly DependencyProperty ClearableProperty =
            DependencyProperty.Register(nameof(Clearable), typeof(bool), typeof(VistaSelect),
                new PropertyMetadata(false));

        public static readonly DependencyProperty MultipleProperty =
            DependencyProperty.Register(nameof(Multiple), typeof(bool), typeof(VistaSelect),
                new PropertyMetadata(false, OnMultipleChanged));

        public static readonly DependencyProperty CollapseTagsProperty =
            DependencyProperty.Register(nameof(CollapseTags), typeof(bool), typeof(VistaSelect),
                new PropertyMetadata(false, OnCollapseChanged));

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(VistaSelect),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValueChanged));

        public static readonly DependencyProperty SelectedValuesProperty =
            DependencyProperty.Register(nameof(SelectedValues), typeof(IList), typeof(VistaSelect),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedValuesChanged));

        #endregion

        #region 属性

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public bool IsSelectDisabled
        {
            get => (bool)GetValue(IsSelectDisabledProperty);
            set => SetValue(IsSelectDisabledProperty, value);
        }

        public bool Clearable
        {
            get => (bool)GetValue(ClearableProperty);
            set => SetValue(ClearableProperty, value);
        }

        public bool Multiple
        {
            get => (bool)GetValue(MultipleProperty);
            set => SetValue(MultipleProperty, value);
        }

        public bool CollapseTags
        {
            get => (bool)GetValue(CollapseTagsProperty);
            set => SetValue(CollapseTagsProperty, value);
        }

        public object? SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public IList? SelectedValues
        {
            get => (IList?)GetValue(SelectedValuesProperty);
            set => SetValue(SelectedValuesProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler<object?>? Change;

        #endregion

        #region 交互

        private void OnMouseLeftButtonDownInternal(object sender, MouseButtonEventArgs e)
        {
            if (IsSelectDisabled) { e.Handled = true; return; }
            // 点选 Option
            if (_popup?.IsOpen == true)
            {
                var origin = e.OriginalSource as DependencyObject;
                var opt = FindAncestorOption(origin);
                if (opt != null)
                {
                    if (opt.IsOptionDisabled) { e.Handled = true; return; }
                    if (Multiple)
                    {
                        EnsureSelectedValues();
                        var key = opt.Value ?? opt.Label;
                        var exists = SelectedValues!.Cast<object?>().Any(v => Equals(v, key));
                        if (exists)
                        {
                            var item = SelectedValues!.Cast<object?>().First(v => Equals(v, key));
                            SelectedValues!.Remove(item);
                        }
                        else
                        {
                            SelectedValues!.Add(key);
                        }
                        Change?.Invoke(this, SelectedValues);
                        UpdateDisplay();
                    }
                    else
                    {
                        SelectedValue = opt.Value ?? opt.Label;
                        Change?.Invoke(this, SelectedValue);
                        UpdateDisplay();
                        if (_popup != null) _popup.IsOpen = false;
                    }
                    e.Handled = true;
                    return;
                }
            }
        }

        private void TogglePopup()
        {
            if (IsSelectDisabled) return;
            if (_popup == null) return;
            _popup.IsOpen = !_popup.IsOpen;
        }

        private void GlobalPreviewMouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (_popup == null || !_popup.IsOpen) return;
            var source = e.OriginalSource as DependencyObject;
            // 点击是否在显示区域或弹出层内部
            if (IsDescendantOf(source, _displayArea)) return;
            if (IsDescendantOf(source, _popup.Child)) return;
            _popup.IsOpen = false;
        }

        private void ClearSelection()
        {
            if (IsSelectDisabled) return;
            if (Multiple)
            {
                if (SelectedValues != null) SelectedValues.Clear();
                Change?.Invoke(this, SelectedValues);
            }
            else
            {
                SelectedValue = null;
                Change?.Invoke(this, SelectedValue);
            }
            UpdateDisplay();
        }

        private static void OnDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 样式触发处理
        }

        private static void OnMultipleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSelect s)
            {
                s.UpdateDisplay();
            }
        }

        private static void OnCollapseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSelect s)
            {
                s.UpdateDisplay();
            }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSelect s)
            {
                s.UpdateDisplay();
            }
        }

        private static void OnSelectedValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSelect s)
            {
                s.UpdateDisplay();
            }
        }

        private void EnsureSelectedValues()
        {
            if (SelectedValues == null)
            {
                SelectedValues = new ObservableCollection<object>();
            }
        }

        private void UpdateDisplay()
        {
            // 更新主显示区域的文本/标签可见性
            if (_displayArea == null)
                return;

            // 清空按钮
            if (_clearButton != null)
            {
                bool hasValue = false;
                if (Multiple)
                    hasValue = SelectedValues != null && SelectedValues.Count > 0;
                else
                    hasValue = SelectedValue != null;
                _clearButton.Visibility = (Clearable && hasValue) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (Multiple)
            {
                if (SelectedValues != null && SelectedValues.Count > 0)
                {
                    if (_placeholderText != null) _placeholderText.Visibility = Visibility.Collapsed;
                    if (_singleText != null) _singleText.Visibility = Visibility.Collapsed;
                    if (CollapseTags)
                    {
                        if (_tagList != null) _tagList.Visibility = Visibility.Collapsed;
                        if (_collapsedText != null)
                        {
                            _collapsedText.Text = $"{SelectedValues.Count} 项已选";
                            _collapsedText.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        if (_collapsedText != null) _collapsedText.Visibility = Visibility.Collapsed;
                        if (_tagList != null)
                        {
                            _tagList.ItemsSource = SelectedValues;
                            _tagList.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    if (_placeholderText != null) _placeholderText.Visibility = Visibility.Visible;
                    if (_singleText != null) _singleText.Visibility = Visibility.Collapsed;
                    if (_tagList != null) _tagList.Visibility = Visibility.Collapsed;
                    if (_collapsedText != null) _collapsedText.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (SelectedValue != null)
                {
                    if (_placeholderText != null) _placeholderText.Visibility = Visibility.Collapsed;
                    if (_tagList != null) _tagList.Visibility = Visibility.Collapsed;
                    if (_collapsedText != null) _collapsedText.Visibility = Visibility.Collapsed;
                    if (_singleText != null)
                    {
                        // 在 Items 中找到对应 Label
                        var label = FindLabelForValue(SelectedValue);
                        _singleText.Text = label ?? SelectedValue?.ToString() ?? "";
                        _singleText.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (_placeholderText != null) _placeholderText.Visibility = Visibility.Visible;
                    if (_singleText != null) _singleText.Visibility = Visibility.Collapsed;
                    if (_tagList != null) _tagList.Visibility = Visibility.Collapsed;
                    if (_collapsedText != null) _collapsedText.Visibility = Visibility.Collapsed;
                }
            }
        }

        private string? FindLabelForValue(object? value)
        {
            if (value == null) return null;
            foreach (var item in Items)
            {
                if (item is VistaOption opt)
                {
                    var val = opt.Value ?? opt.Label;
                    if (Equals(val, value)) return opt.Label;
                }
                else if (item is VistaOptionGroup group)
                {
                    foreach (var it in group.Items)
                    {
                        if (it is VistaOption opt2)
                        {
                            var val = opt2.Value ?? opt2.Label;
                            if (Equals(val, value)) return opt2.Label;
                        }
                    }
                }
            }
            return null;
        }

        private VistaOption? FindAncestorOption(DependencyObject? origin)
        {
            while (origin != null)
            {
                if (origin is VistaOption vo) return vo;
                // Visual tree up
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(origin);
                if (parent == null)
                {
                    // try logical
                    parent = LogicalTreeHelper.GetParent(origin);
                }
                origin = parent;
            }
            return null;
        }

        private bool IsDescendantOf(DependencyObject? origin, DependencyObject? root)
        {
            if (origin == null || root == null) return false;
            var current = origin;
            while (current != null)
            {
                if (current == root) return true;
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(current);
                if (parent == null)
                {
                    parent = LogicalTreeHelper.GetParent(current);
                }
                current = parent;
            }
            return false;
        }

        #endregion
    }
}


