using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VistaControls
{
    public enum ExpandTrigger
    {
        Click,
        Hover
    }

    /// <summary>
    /// VistaCascader - 级联选择器
    /// </summary>
    public class VistaCascader : Control
    {
        private Popup? _popup;
        private Border? _display;
        private Button? _clear;
        private readonly List<CascaderOption> _currentPath = new List<CascaderOption>();

        static VistaCascader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaCascader),
                new FrameworkPropertyMetadata(typeof(VistaCascader)));
        }

        public VistaCascader()
        {
            Loaded += (_, __) => UpdateDisplay();
            if (Application.Current?.MainWindow != null)
            {
                Application.Current.MainWindow.PreviewMouseDown += GlobalCloseOnOutside;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = GetTemplateChild("PART_Popup") as Popup;
            _display = GetTemplateChild("PART_Display") as Border;
            _clear = GetTemplateChild("PART_Clear") as Button;

            if (_display != null)
            {
                _display.MouseLeftButtonDown += (_, __) => TogglePopup();
            }
            if (_clear != null)
            {
                _clear.Click += (_, __) => ClearSelection();
            }
            UpdateMenusToRoot();
            UpdateDisplay();

            // 在弹出层内部捕获鼠标事件，处理 Hover/Click 导航与选择
            if (_popup?.Child is FrameworkElement fe)
            {
                fe.AddHandler(UIElement.MouseMoveEvent, new MouseEventHandler(OnPopupMouseMove), true);
                fe.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPopupMouseLeftButtonDown), true);
            }
        }

        #region 依赖属性

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaCascader),
                new PropertyMetadata("请选择"));

        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(nameof(Options), typeof(ObservableCollection<CascaderOption>), typeof(VistaCascader),
                new PropertyMetadata(new ObservableCollection<CascaderOption>(), OnOptionsChanged));

        public static readonly DependencyProperty ExpandTriggerProperty =
            DependencyProperty.Register(nameof(ExpandTrigger), typeof(ExpandTrigger), typeof(VistaCascader),
                new PropertyMetadata(ExpandTrigger.Click));

        public static readonly DependencyProperty ClearableProperty =
            DependencyProperty.Register(nameof(Clearable), typeof(bool), typeof(VistaCascader),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ShowAllLevelsProperty =
            DependencyProperty.Register(nameof(ShowAllLevels), typeof(bool), typeof(VistaCascader),
                new PropertyMetadata(true, OnShowLevelsChanged));

        public static readonly DependencyProperty MultipleProperty =
            DependencyProperty.Register(nameof(Multiple), typeof(bool), typeof(VistaCascader),
                new PropertyMetadata(false));

        public static readonly DependencyProperty CollapseTagsProperty =
            DependencyProperty.Register(nameof(CollapseTags), typeof(bool), typeof(VistaCascader),
                new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register(nameof(SelectedPath), typeof(ObservableCollection<object>), typeof(VistaCascader),
                new FrameworkPropertyMetadata(new ObservableCollection<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedPathChanged));

        public static readonly DependencyProperty SelectedPathsProperty =
            DependencyProperty.Register(nameof(SelectedPaths), typeof(ObservableCollection<IList>), typeof(VistaCascader),
                new FrameworkPropertyMetadata(new ObservableCollection<IList>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedPathsChanged));

        internal static readonly DependencyProperty MenusProperty =
            DependencyProperty.Register(nameof(Menus), typeof(ObservableCollection<ObservableCollection<CascaderOption>>), typeof(VistaCascader),
                new PropertyMetadata(new ObservableCollection<ObservableCollection<CascaderOption>>()));

        #endregion

        #region 属性

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public ObservableCollection<CascaderOption> Options
        {
            get => (ObservableCollection<CascaderOption>)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }

        public ExpandTrigger ExpandTrigger
        {
            get => (ExpandTrigger)GetValue(ExpandTriggerProperty);
            set => SetValue(ExpandTriggerProperty, value);
        }

        public bool Clearable
        {
            get => (bool)GetValue(ClearableProperty);
            set => SetValue(ClearableProperty, value);
        }

        public bool ShowAllLevels
        {
            get => (bool)GetValue(ShowAllLevelsProperty);
            set => SetValue(ShowAllLevelsProperty, value);
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

        public ObservableCollection<object> SelectedPath
        {
            get => (ObservableCollection<object>)GetValue(SelectedPathProperty);
            set => SetValue(SelectedPathProperty, value);
        }

        public ObservableCollection<IList> SelectedPaths
        {
            get => (ObservableCollection<IList>)GetValue(SelectedPathsProperty);
            set => SetValue(SelectedPathsProperty, value);
        }

        internal ObservableCollection<ObservableCollection<CascaderOption>> Menus
        {
            get => (ObservableCollection<ObservableCollection<CascaderOption>>)GetValue(MenusProperty);
            set => SetValue(MenusProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler<IList>? Change;

        #endregion

        #region 行为

        private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaCascader c) c.UpdateMenusToRoot();
        }

        private static void OnShowLevelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaCascader c) c.UpdateDisplay();
        }

        private static void OnSelectedPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaCascader c) c.UpdateDisplay();
        }

        private static void OnSelectedPathsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaCascader c) c.UpdateDisplay();
        }

        private void TogglePopup()
        {
            if (_popup == null) return;
            _popup.IsOpen = !_popup.IsOpen;
            if (_popup.IsOpen) UpdateMenusToRoot();
        }

        private void ClearSelection()
        {
            if (Multiple)
            {
                SelectedPaths.Clear();
                Change?.Invoke(this, SelectedPaths);
            }
            else
            {
                SelectedPath.Clear();
                Change?.Invoke(this, SelectedPath);
            }
            UpdateDisplay();
        }

        private void GlobalCloseOnOutside(object? sender, MouseButtonEventArgs e)
        {
            if (_popup == null || !_popup.IsOpen) return;
            var src = e.OriginalSource as DependencyObject;
            if (IsDescendantOf(src, _popup.Child)) return;
            if (IsDescendantOf(src, _display)) return;
            _popup.IsOpen = false;
        }

        private bool IsDescendantOf(DependencyObject? child, DependencyObject? root = null)
        {
            if (child == null || root == null) return false;
            var cur = child;
            while (cur != null)
            {
                if (cur == root) return true;
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(cur);
                if (parent == null) parent = LogicalTreeHelper.GetParent(cur);
                cur = parent;
            }
            return false;
        }

        internal void UpdateMenusToRoot()
        {
            Menus.Clear();
            Menus.Add(Options);
        }

        internal void ExpandToChildren(CascaderOption option, int levelIndex)
        {
            // 砍掉后面列
            while (Menus.Count > levelIndex + 1)
            {
                Menus.RemoveAt(Menus.Count - 1);
            }
            if (option.Children != null && option.Children.Count > 0)
            {
                Menus.Add(option.Children);
            }

            // 更新当前路径到该层
            if (_currentPath.Count > levelIndex)
                _currentPath.RemoveRange(levelIndex, _currentPath.Count - levelIndex);
            if (_currentPath.Count == levelIndex)
                _currentPath.Add(option);
            else
                _currentPath[levelIndex] = option;
        }

        internal void SelectLeafPath(List<CascaderOption> path)
        {
            if (Multiple)
            {
                var values = path.Select(p => p.Value ?? p.Label).ToList();
                // 如果已存在则取消
                var exists = SelectedPaths.Any(p => PathEquals(p, values));
                if (exists)
                {
                    var item = SelectedPaths.First(p => PathEquals(p, values));
                    SelectedPaths.Remove(item);
                }
                else
                {
                    SelectedPaths.Add(values);
                }
                Change?.Invoke(this, SelectedPaths);
            }
            else
            {
                SelectedPath.Clear();
                foreach (var p in path)
                {
                    SelectedPath.Add(p.Value ?? p.Label);
                }
                Change?.Invoke(this, SelectedPath);
                if (_popup != null) _popup.IsOpen = false;
            }
            UpdateDisplay();
        }

        private static bool PathEquals(IList a, IList b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!Equals(a[i], b[i])) return false;
            }
            return true;
        }

        private void UpdateDisplay()
        {
            if (_display == null) return;
            var singleText = FindTemplateChild<TextBlock>("singlePathText");
            var placeholder = FindTemplateChild<TextBlock>("placeholderText");
            var tags = FindTemplateChild<ItemsControl>("multiTags");
            var collapsed = FindTemplateChild<TextBlock>("multiCollapsedText");

            bool has = false;
            if (Multiple)
            {
                has = SelectedPaths != null && SelectedPaths.Count > 0;
                if (has)
                {
                    if (placeholder != null) placeholder.Visibility = Visibility.Collapsed;
                    if (collapsed != null && CollapseTags)
                    {
                        collapsed.Text = $"{SelectedPaths.Count} 项已选";
                        collapsed.Visibility = Visibility.Visible;
                        if (tags != null) tags.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (collapsed != null) collapsed.Visibility = Visibility.Collapsed;
                        if (tags != null)
                        {
                            var display = SelectedPaths.Select(p => PathToDisplay(p)).ToList();
                            tags.ItemsSource = display;
                            tags.Visibility = Visibility.Visible;
                        }
                    }
                    if (singleText != null) singleText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (placeholder != null) placeholder.Visibility = Visibility.Visible;
                    if (singleText != null) singleText.Visibility = Visibility.Collapsed;
                    if (tags != null) tags.Visibility = Visibility.Collapsed;
                    if (collapsed != null) collapsed.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                has = SelectedPath != null && SelectedPath.Count > 0;
                if (has)
                {
                    if (placeholder != null) placeholder.Visibility = Visibility.Collapsed;
                    if (tags != null) tags.Visibility = Visibility.Collapsed;
                    if (collapsed != null) collapsed.Visibility = Visibility.Collapsed;
                    if (singleText != null)
                    {
                        singleText.Text = PathToDisplay(SelectedPath);
                        singleText.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (placeholder != null) placeholder.Visibility = Visibility.Visible;
                    if (singleText != null) singleText.Visibility = Visibility.Collapsed;
                    if (tags != null) tags.Visibility = Visibility.Collapsed;
                    if (collapsed != null) collapsed.Visibility = Visibility.Collapsed;
                }
            }

            if (_clear != null)
            {
                _clear.Visibility = (Clearable && has) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private string PathToDisplay(IList path)
        {
            if (ShowAllLevels)
            {
                return string.Join(" / ", path.Cast<object>().Select(v => FindLabelByValue(v) ?? v?.ToString() ?? ""));
            }
            else
            {
                var last = path.Count > 0 ? path[path.Count - 1] : null;
                return FindLabelByValue(last) ?? last?.ToString() ?? "";
            }
        }

        private string? FindLabelByValue(object? value)
        {
            if (value == null) return null;
            string? Search(ObservableCollection<CascaderOption> opts)
            {
                foreach (var o in opts)
                {
                    if (Equals(o.Value ?? o.Label, value)) return o.Label;
                    var sub = Search(o.Children);
                    if (sub != null) return sub;
                }
                return null;
            }
            return Search(Options);
        }

        private T? FindTemplateChild<T>(string name) where T : DependencyObject
        {
            return _display?.FindName(name) as T;
        }

        #endregion

        #region 事件桥接（模板中使用）
        private void OnPopupMouseMove(object sender, MouseEventArgs e)
        {
            if (ExpandTrigger != ExpandTrigger.Hover) return;
            HandleItemNavigate(e.OriginalSource as DependencyObject, false);
        }

        private void OnPopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ExpandTrigger != ExpandTrigger.Click) return;
            HandleItemNavigate(e.OriginalSource as DependencyObject, true);
        }

        private void HandleItemNavigate(DependencyObject? origin, bool isClick)
        {
            var cp = FindContentPresenter(origin);
            if (cp?.Content is CascaderOption option)
            {
                var columnIc = ItemsControl.ItemsControlFromItemContainer(cp);
                int level = GetColumnLevelIndex(columnIc);

                if (option.Disabled) return;

                if (option.Children != null && option.Children.Count > 0)
                {
                    ExpandToChildren(option, level);
                }
                else
                {
                    var path = BuildPath(option);
                    SelectLeafPath(path);
                }
            }
        }

        private int GetColumnLevelIndex(ItemsControl? columnIc)
        {
            if (columnIc == null) return 0;
            var src = columnIc.ItemsSource as IEnumerable<CascaderOption>;
            if (src != null)
            {
                for (int i = 0; i < Menus.Count; i++)
                {
                    if (ReferenceEquals(Menus[i], src)) return i;
                }
            }
            return 0;
        }

        private ContentPresenter? FindContentPresenter(DependencyObject? origin)
        {
            var current = origin;
            while (current != null)
            {
                if (current is ContentPresenter cp && cp.Content is CascaderOption) return cp;
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(current);
                if (parent == null) parent = LogicalTreeHelper.GetParent(current);
                current = parent;
            }
            return null;
        }

        private List<CascaderOption> BuildPath(CascaderOption leaf)
        {
            // 简化：利用当前 Menus 和 _currentPath 叠加叶子生成路径
            var path = new List<CascaderOption>(_currentPath);
            if (path.Count == 0 || !ReferenceEquals(path.Last(), leaf))
            {
                if (leaf != null) path.Add(leaf);
            }
            return path;
        }

        #endregion
    }
}
