using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaTabPane - 标签页项
    /// </summary>
    public class VistaTabPane : ContentControl
    {
        static VistaTabPane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTabPane),
                new FrameworkPropertyMetadata(typeof(VistaTabPane)));
        }

        #region 依赖属性

        /// <summary>
        /// 选项卡标题
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(VistaTabPane),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 是否禁用
        /// </summary>
        public static readonly DependencyProperty IsTabDisabledProperty =
            DependencyProperty.Register(nameof(IsTabDisabled), typeof(bool), typeof(VistaTabPane),
                new PropertyMetadata(false));

        /// <summary>
        /// 与选项卡绑定值 value 对应的标识符，表示选项卡别名
        /// </summary>
        public new static readonly DependencyProperty NameProperty =
            DependencyProperty.Register(nameof(TabName), typeof(string), typeof(VistaTabPane),
                new PropertyMetadata(string.Empty, OnNameChanged));

        /// <summary>
        /// 标签是否可关闭
        /// </summary>
        public static readonly DependencyProperty ClosableProperty =
            DependencyProperty.Register(nameof(Closable), typeof(bool), typeof(VistaTabPane),
                new PropertyMetadata(false));

        /// <summary>
        /// 标签是否延迟渲染
        /// </summary>
        public static readonly DependencyProperty LazyProperty =
            DependencyProperty.Register(nameof(Lazy), typeof(bool), typeof(VistaTabPane),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否选中
        /// </summary>
        internal static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(VistaTabPane),
                new PropertyMetadata(false));

        private static void OnNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaTabPane pane && pane.Parent is VistaTabs tabs)
            {
                tabs.UpdateTabPanes();
                tabs.UpdateSelectedTab();
            }
        }

        #endregion

        #region 属性

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public bool IsTabDisabled
        {
            get => (bool)GetValue(IsTabDisabledProperty);
            set => SetValue(IsTabDisabledProperty, value);
        }

        public new string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// 选项卡别名（与 Name 相同，用于避免与 FrameworkElement.Name 冲突）
        /// </summary>
        public string TabName
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        public bool Closable
        {
            get => (bool)GetValue(ClosableProperty);
            set => SetValue(ClosableProperty, value);
        }

        public bool Lazy
        {
            get => (bool)GetValue(LazyProperty);
            set => SetValue(LazyProperty, value);
        }

        internal bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion

        private Border? _tabHeaderButton;
        private Button? _closeButton;

        public VistaTabPane()
        {
            Loaded += VistaTabPane_Loaded;
            MouseLeftButtonDown += VistaTabPane_MouseLeftButtonDown;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tabHeaderButton = GetTemplateChild("tabHeaderButton") as Border;
            _closeButton = GetTemplateChild("closeButton") as Button;
            
            if (_closeButton != null)
            {
                _closeButton.Click += CloseButton_Click;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true; // 阻止事件冒泡
            OnCloseButtonClick();
        }

        private void VistaTabPane_Loaded(object sender, RoutedEventArgs e)
        {
            // 如果 Name 为空，自动设置为索引
            if (string.IsNullOrEmpty(TabName) && Parent is VistaTabs tabs)
            {
                var index = tabs.Items.IndexOf(this);
                if (index >= 0)
                {
                    TabName = (index + 1).ToString();
                }
            }
        }

        private void VistaTabPane_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsTabDisabled) return;

            if (Parent is VistaTabs tabs)
            {
                tabs.OnTabClick(TabName);
            }
        }

        internal void OnCloseButtonClick()
        {
            if (Parent is VistaTabs tabs)
            {
                tabs.OnTabClose(TabName);
            }
        }
    }
}

