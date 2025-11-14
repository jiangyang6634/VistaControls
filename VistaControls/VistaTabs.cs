using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VistaControls
{
    /// <summary>
    /// VistaTabs - 标签页容器
    /// </summary>
    public class VistaTabs : ItemsControl
    {
        private ContentControl? _tabContentContainer;
        private Dictionary<string, VistaTabPane> _tabPanes = new Dictionary<string, VistaTabPane>();

        static VistaTabs()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTabs),
                new FrameworkPropertyMetadata(typeof(VistaTabs)));
        }

        public VistaTabs()
        {
            Loaded += VistaTabs_Loaded;
        }

        private void VistaTabs_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTabPanes();
            UpdateSelectedTab();
        }

        private Button? _addTabButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _tabContentContainer = GetTemplateChild("PART_TabContentContainer") as ContentControl;
            _addTabButton = GetTemplateChild("PART_AddTabButton") as Button;

            if (_addTabButton != null)
            {
                _addTabButton.Click += AddTabButton_Click;
            }

            UpdateTabPanes();
            UpdateSelectedTab();
            UpdateAddButtonVisibility();
        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            OnTabAdd();
        }

        private void UpdateAddButtonVisibility()
        {
            if (_addTabButton != null)
            {
                // 如果设置了 Closable 或者有任何标签页设置了 Closable，显示添加按钮
                bool hasClosable = Closable || Items.OfType<VistaTabPane>().Any(p => p.Closable);
                _addTabButton.Visibility = hasClosable ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            
            // 当 Closable 属性改变时，更新添加按钮的可见性
            if (e.Property == ClosableProperty)
            {
                UpdateAddButtonVisibility();
            }
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            UpdateTabPanes();
            UpdateSelectedTab();
            UpdateAddButtonVisibility();
        }

        internal void UpdateTabPanes()
        {
            _tabPanes.Clear();
            foreach (var item in Items)
            {
                if (item is VistaTabPane pane)
                {
                    var name = pane.TabName;
                    if (string.IsNullOrEmpty(name))
                    {
                        var index = Items.IndexOf(pane);
                        name = (index + 1).ToString();
                        pane.TabName = name;
                    }
                    _tabPanes[name] = pane;
                }
            }
        }

        internal void UpdateSelectedTab()
        {
            var selectedName = Value;
            if (string.IsNullOrEmpty(selectedName))
            {
                // 默认选中第一个
                if (_tabPanes.Count > 0)
                {
                    var firstPane = _tabPanes.Values.FirstOrDefault(p => !p.IsTabDisabled);
                    if (firstPane != null)
                    {
                        selectedName = firstPane.Name;
                        Value = selectedName;
                    }
                }
            }

            // 更新所有标签页的选中状态
            foreach (var pane in _tabPanes.Values)
            {
                pane.IsSelected = pane.TabName == selectedName;
                
                // 如果使用延迟渲染，只有选中的标签页才渲染内容
                if (pane.Lazy && !pane.IsSelected)
                {
                    // 延迟渲染：未选中的标签页不显示内容
                    continue;
                }
            }

            // 更新内容区域
            if (_tabContentContainer != null && !string.IsNullOrEmpty(selectedName) && _tabPanes.ContainsKey(selectedName))
            {
                var selectedPane = _tabPanes[selectedName];
                
                // 显示选中的标签页内容（只显示 Content，而不是整个 Pane）
                // 如果使用延迟渲染，只有选中的标签页才显示内容
                if (selectedPane.Lazy && !selectedPane.IsSelected)
                {
                    _tabContentContainer.Content = null;
                }
                else
                {
                    _tabContentContainer.Content = selectedPane.Content;
                }
            }
            else if (_tabContentContainer != null)
            {
                _tabContentContainer.Content = null;
            }
        }

        internal void OnTabClick(string tabName)
        {
            if (!_tabPanes.ContainsKey(tabName))
                return;

            var pane = _tabPanes[tabName];
            if (pane.IsTabDisabled)
                return;

            // 调用 before-leave 钩子
            if (BeforeLeave != null)
            {
                var oldName = Value;
                var result = BeforeLeave(tabName, oldName);
                if (result == false)
                {
                    return; // 阻止切换
                }
            }

            var oldValue = Value;
            Value = tabName;
            UpdateSelectedTab();

            // 触发 tab-click 事件
            RaiseEvent(new TabClickEventArgs(TabClickEvent, pane));
        }

        internal void OnTabClose(string tabName)
        {
            if (!_tabPanes.ContainsKey(tabName))
                return;

            var pane = _tabPanes[tabName];

            // 触发 tab-remove 事件
            RaiseEvent(new TabRemoveEventArgs(TabRemoveEvent, tabName));

            // 触发 edit 事件
            RaiseEvent(new TabEditEventArgs(EditEvent, tabName, "remove"));

            // 从集合中移除
            Items.Remove(pane);
            _tabPanes.Remove(tabName);

            // 如果关闭的是当前选中的标签页，切换到其他标签页
            if (Value == tabName)
            {
                var remainingPanes = _tabPanes.Values.Where(p => !p.IsTabDisabled).ToList();
                if (remainingPanes.Count > 0)
                {
                    Value = remainingPanes[0].TabName;
                }
                else
                {
                    Value = string.Empty;
                }
            }

            UpdateSelectedTab();
        }

        internal void OnTabAdd()
        {
            // 触发 tab-add 事件
            RaiseEvent(new RoutedEventArgs(TabAddEvent));

            // 触发 edit 事件
            RaiseEvent(new TabEditEventArgs(EditEvent, string.Empty, "add"));
        }

        #region 依赖属性

        /// <summary>
        /// 绑定值，选中选项卡的 name
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(VistaTabs),
                new PropertyMetadata(string.Empty, OnValueChanged));

        /// <summary>
        /// 标签是否可关闭
        /// </summary>
        public static readonly DependencyProperty ClosableProperty =
            DependencyProperty.Register(nameof(Closable), typeof(bool), typeof(VistaTabs),
                new PropertyMetadata(false, OnClosableChanged));

        private static void OnClosableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTabs tabs)
            {
                tabs.UpdateAddButtonVisibility();
            }
        }

        /// <summary>
        /// 选项卡所在位置
        /// </summary>
        public static readonly DependencyProperty TabPositionProperty =
            DependencyProperty.Register(nameof(TabPosition), typeof(TabPosition), typeof(VistaTabs),
                new PropertyMetadata(TabPosition.Top, OnTabPositionChanged));

        /// <summary>
        /// 标签的宽度是否自撑开
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(nameof(Stretch), typeof(bool), typeof(VistaTabs),
                new PropertyMetadata(false));

        /// <summary>
        /// 切换标签之前的钩子，若返回 false，则阻止切换
        /// </summary>
        public static readonly DependencyProperty BeforeLeaveProperty =
            DependencyProperty.Register(nameof(BeforeLeave), typeof(Func<string, string, bool>), typeof(VistaTabs),
                new PropertyMetadata(null));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTabs tabs)
            {
                tabs.UpdateSelectedTab();
            }
        }

        private static void OnTabPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTabs tabs)
            {
                tabs.UpdateTabPosition();
            }
        }

        private void UpdateTabPosition()
        {
            // 通过样式触发器更新，这里不需要额外处理
        }

        #endregion

        #region 属性

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public bool Closable
        {
            get => (bool)GetValue(ClosableProperty);
            set => SetValue(ClosableProperty, value);
        }

        public TabPosition TabPosition
        {
            get => (TabPosition)GetValue(TabPositionProperty);
            set => SetValue(TabPositionProperty, value);
        }

        public bool Stretch
        {
            get => (bool)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public Func<string, string, bool>? BeforeLeave
        {
            get => (Func<string, string, bool>?)GetValue(BeforeLeaveProperty);
            set => SetValue(BeforeLeaveProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// tab 被选中时触发
        /// </summary>
        public static readonly RoutedEvent TabClickEvent =
            EventManager.RegisterRoutedEvent(nameof(TabClick), RoutingStrategy.Bubble,
                typeof(EventHandler<TabClickEventArgs>), typeof(VistaTabs));

        public event EventHandler<TabClickEventArgs> TabClick
        {
            add => AddHandler(TabClickEvent, value);
            remove => RemoveHandler(TabClickEvent, value);
        }

        /// <summary>
        /// 点击 tab 移除按钮后触发
        /// </summary>
        public static readonly RoutedEvent TabRemoveEvent =
            EventManager.RegisterRoutedEvent(nameof(TabRemove), RoutingStrategy.Bubble,
                typeof(EventHandler<TabRemoveEventArgs>), typeof(VistaTabs));

        public event EventHandler<TabRemoveEventArgs> TabRemove
        {
            add => AddHandler(TabRemoveEvent, value);
            remove => RemoveHandler(TabRemoveEvent, value);
        }

        /// <summary>
        /// 点击 tabs 的新增按钮后触发
        /// </summary>
        public static readonly RoutedEvent TabAddEvent =
            EventManager.RegisterRoutedEvent(nameof(TabAdd), RoutingStrategy.Bubble,
                typeof(EventHandler), typeof(VistaTabs));

        public event EventHandler TabAdd
        {
            add => AddHandler(TabAddEvent, value);
            remove => RemoveHandler(TabAddEvent, value);
        }

        /// <summary>
        /// 点击 tabs 的新增按钮或 tab 被关闭后触发
        /// </summary>
        public static readonly RoutedEvent EditEvent =
            EventManager.RegisterRoutedEvent(nameof(Edit), RoutingStrategy.Bubble,
                typeof(EventHandler<TabEditEventArgs>), typeof(VistaTabs));

        public event EventHandler<TabEditEventArgs> Edit
        {
            add => AddHandler(EditEvent, value);
            remove => RemoveHandler(EditEvent, value);
        }

        #endregion
    }

    /// <summary>
    /// 选项卡位置
    /// </summary>
    public enum TabPosition
    {
        Top,
        Right,
        Bottom,
        Left
    }

    /// <summary>
    /// Tab 点击事件参数
    /// </summary>
    public class TabClickEventArgs : RoutedEventArgs
    {
        public VistaTabPane Tab { get; }

        public TabClickEventArgs(RoutedEvent routedEvent, VistaTabPane tab) : base(routedEvent)
        {
            Tab = tab;
        }
    }

    /// <summary>
    /// Tab 移除事件参数
    /// </summary>
    public class TabRemoveEventArgs : RoutedEventArgs
    {
        public string TabName { get; }

        public TabRemoveEventArgs(RoutedEvent routedEvent, string tabName) : base(routedEvent)
        {
            TabName = tabName;
        }
    }

    /// <summary>
    /// Tab 编辑事件参数
    /// </summary>
    public class TabEditEventArgs : RoutedEventArgs
    {
        public string TargetName { get; }
        public string Action { get; }

        public TabEditEventArgs(RoutedEvent routedEvent, string targetName, string action) : base(routedEvent)
        {
            TargetName = targetName;
            Action = action;
        }
    }
}

