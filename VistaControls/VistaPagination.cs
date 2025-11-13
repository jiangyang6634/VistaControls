using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VistaControls
{
    /// <summary>
    /// VistaPagination - 分页控件
    /// </summary>
    public class VistaPagination : Control
    {
        private Button? _prevButton;
        private Button? _nextButton;
        private ItemsControl? _pagerContainer;
        private VistaInput? _jumperInput;
        private VistaSelect? _sizesSelect;
        private TextBlock? _totalText;

        static VistaPagination()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaPagination),
                new FrameworkPropertyMetadata(typeof(VistaPagination)));
        }

        public VistaPagination()
        {
            Loaded += VistaPagination_Loaded;
        }

        private void VistaPagination_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePagination();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _prevButton = GetTemplateChild("PART_PrevButton") as Button;
            _nextButton = GetTemplateChild("PART_NextButton") as Button;
            _pagerContainer = GetTemplateChild("PART_PagerContainer") as ItemsControl;
            _jumperInput = GetTemplateChild("PART_JumperInput") as VistaInput;
            _sizesSelect = GetTemplateChild("PART_SizesSelect") as VistaSelect;
            _totalText = GetTemplateChild("PART_TotalText") as TextBlock;

            if (_prevButton != null)
            {
                _prevButton.Click -= PrevButton_Click;
                _prevButton.Click += PrevButton_Click;
            }

            if (_nextButton != null)
            {
                _nextButton.Click -= NextButton_Click;
                _nextButton.Click += NextButton_Click;
            }

            if (_jumperInput != null)
            {
                _jumperInput.KeyDown -= JumperInput_KeyDown;
                _jumperInput.KeyDown += JumperInput_KeyDown;
            }

            if (_sizesSelect != null)
            {
                _sizesSelect.Change -= SizesSelect_Change;
                _sizesSelect.Change += SizesSelect_Change;
                UpdateSizesSelect();
            }

            // 为页码按钮绑定点击事件
            if (_pagerContainer != null)
            {
                _pagerContainer.Loaded += (s, e) => AttachPagerButtonEvents();
            }

            UpdatePagination();
        }

        private void AttachPagerButtonEvents()
        {
            if (_pagerContainer == null) return;

            // 遍历 ItemsControl 的子元素，为每个按钮绑定点击事件
            var itemsPanel = _pagerContainer.ItemsPanel?.LoadContent() as Panel;
            if (itemsPanel != null)
            {
                foreach (var item in _pagerContainer.Items)
                {
                    var container = _pagerContainer.ItemContainerGenerator.ContainerFromItem(item);
                    if (container is ContentPresenter presenter)
                    {
                        var button = FindVisualChild<Button>(presenter);
                        if (button != null && button.Tag is int page)
                        {
                            button.Click -= PagerButton_Click;
                            button.Click += PagerButton_Click;
                        }
                    }
                }
            }
        }

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                {
                    return t;
                }
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                var newPage = CurrentPage - 1;
                SetCurrentValue(CurrentPageProperty, newPage);
                OnPrevClick(newPage);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var totalPages = GetTotalPages();
            if (CurrentPage < totalPages)
            {
                var newPage = CurrentPage + 1;
                SetCurrentValue(CurrentPageProperty, newPage);
                OnNextClick(newPage);
            }
        }

        private void JumperInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _jumperInput != null)
            {
                if (int.TryParse(_jumperInput.Text, out var page))
                {
                    var totalPages = GetTotalPages();
                    page = Math.Max(1, Math.Min(totalPages, page));
                    SetCurrentValue(CurrentPageProperty, page);
                    _jumperInput.Text = string.Empty;
                }
            }
        }

        private void SizesSelect_Change(object sender, object e)
        {
            if (_sizesSelect != null && _sizesSelect.SelectedValue is int size)
            {
                SetCurrentValue(PageSizeProperty, size);
                // 重新计算当前页，确保不超出范围
                var totalPages = GetTotalPages();
                if (CurrentPage > totalPages)
                {
                    SetCurrentValue(CurrentPageProperty, totalPages);
                }
                OnSizeChange(size);
            }
        }

        private void PagerButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int page)
            {
                SetCurrentValue(CurrentPageProperty, page);
            }
        }

        #region 依赖属性

        /// <summary>
        /// 是否使用小型分页样式
        /// </summary>
        public static readonly DependencyProperty SmallProperty =
            DependencyProperty.Register(nameof(Small), typeof(bool), typeof(VistaPagination),
                new PropertyMetadata(false, OnAppearanceChanged));

        /// <summary>
        /// 是否为分页按钮添加背景色
        /// </summary>
        public static readonly DependencyProperty HasBackgroundProperty =
            DependencyProperty.Register(nameof(HasBackground), typeof(bool), typeof(VistaPagination),
                new PropertyMetadata(false, OnAppearanceChanged));

        /// <summary>
        /// 每页显示条目个数
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(VistaPagination),
                new PropertyMetadata(10, OnPaginationChanged));

        /// <summary>
        /// 总条目数
        /// </summary>
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register(nameof(Total), typeof(int), typeof(VistaPagination),
                new PropertyMetadata(0, OnPaginationChanged));

        /// <summary>
        /// 总页数（如果设置了 Total，则根据 Total 和 PageSize 计算）
        /// </summary>
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register(nameof(PageCount), typeof(int), typeof(VistaPagination),
                new PropertyMetadata(0, OnPaginationChanged));

        /// <summary>
        /// 页码按钮的数量，当总页数超过该值时会折叠
        /// </summary>
        public static readonly DependencyProperty PagerCountProperty =
            DependencyProperty.Register(nameof(PagerCount), typeof(int), typeof(VistaPagination),
                new PropertyMetadata(7, OnPaginationChanged, CoercePagerCount));

        /// <summary>
        /// 当前页数
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(int), typeof(VistaPagination),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentPageChanged, CoerceCurrentPage));

        /// <summary>
        /// 组件布局，子组件名用逗号分隔
        /// </summary>
        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register(nameof(Layout), typeof(string), typeof(VistaPagination),
                new PropertyMetadata("prev, pager, next, jumper, ->, total", OnLayoutChanged));

        /// <summary>
        /// 每页显示个数选择器的选项设置
        /// </summary>
        public static readonly DependencyProperty PageSizesProperty =
            DependencyProperty.Register(nameof(PageSizes), typeof(ObservableCollection<int>), typeof(VistaPagination),
                new PropertyMetadata(null, OnPageSizesChanged));

        /// <summary>
        /// 替代图标显示的上一页文字
        /// </summary>
        public static readonly DependencyProperty PrevTextProperty =
            DependencyProperty.Register(nameof(PrevText), typeof(string), typeof(VistaPagination),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 替代图标显示的下一页文字
        /// </summary>
        public static readonly DependencyProperty NextTextProperty =
            DependencyProperty.Register(nameof(NextText), typeof(string), typeof(VistaPagination),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 是否禁用
        /// </summary>
        public static readonly DependencyProperty IsDisabledProperty =
            DependencyProperty.Register(nameof(IsDisabled), typeof(bool), typeof(VistaPagination),
                new PropertyMetadata(false, OnAppearanceChanged));

        /// <summary>
        /// 只有一页时是否隐藏
        /// </summary>
        public static readonly DependencyProperty HideOnSinglePageProperty =
            DependencyProperty.Register(nameof(HideOnSinglePage), typeof(bool), typeof(VistaPagination),
                new PropertyMetadata(false, OnAppearanceChanged));

        #endregion

        #region 属性

        public bool Small
        {
            get => (bool)GetValue(SmallProperty);
            set => SetValue(SmallProperty, value);
        }

        public bool HasBackground
        {
            get => (bool)GetValue(HasBackgroundProperty);
            set => SetValue(HasBackgroundProperty, value);
        }

        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            set => SetValue(PageSizeProperty, value);
        }

        public int Total
        {
            get => (int)GetValue(TotalProperty);
            set => SetValue(TotalProperty, value);
        }

        public int PageCount
        {
            get => (int)GetValue(PageCountProperty);
            set => SetValue(PageCountProperty, value);
        }

        public int PagerCount
        {
            get => (int)GetValue(PagerCountProperty);
            set => SetValue(PagerCountProperty, value);
        }

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public string Layout
        {
            get => (string)GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }

        public ObservableCollection<int>? PageSizes
        {
            get => (ObservableCollection<int>?)GetValue(PageSizesProperty);
            set => SetValue(PageSizesProperty, value);
        }

        public string PrevText
        {
            get => (string)GetValue(PrevTextProperty);
            set => SetValue(PrevTextProperty, value);
        }

        public string NextText
        {
            get => (string)GetValue(NextTextProperty);
            set => SetValue(NextTextProperty, value);
        }

        public bool IsDisabled
        {
            get => (bool)GetValue(IsDisabledProperty);
            set => SetValue(IsDisabledProperty, value);
        }

        public bool HideOnSinglePage
        {
            get => (bool)GetValue(HideOnSinglePageProperty);
            set => SetValue(HideOnSinglePageProperty, value);
        }

        #endregion

        #region 内部属性

        /// <summary>
        /// 页码按钮列表
        /// </summary>
        public static readonly DependencyProperty PagerButtonsProperty =
            DependencyProperty.Register(nameof(PagerButtons), typeof(ObservableCollection<PagerButtonInfo>), typeof(VistaPagination),
                new PropertyMetadata(new ObservableCollection<PagerButtonInfo>()));

        public ObservableCollection<PagerButtonInfo> PagerButtons
        {
            get => (ObservableCollection<PagerButtonInfo>)GetValue(PagerButtonsProperty);
            private set => SetValue(PagerButtonsProperty, value);
        }

        /// <summary>
        /// 总页数（计算值）
        /// </summary>
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register(nameof(TotalPages), typeof(int), typeof(VistaPagination),
                new PropertyMetadata(0));

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            private set => SetValue(TotalPagesProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// pageSize 改变时会触发
        /// </summary>
        public static readonly RoutedEvent SizeChangeEvent =
            EventManager.RegisterRoutedEvent(nameof(SizeChange), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(VistaPagination));

        public event RoutedEventHandler SizeChange
        {
            add => AddHandler(SizeChangeEvent, value);
            remove => RemoveHandler(SizeChangeEvent, value);
        }

        protected virtual void OnSizeChange(int pageSize)
        {
            var args = new PaginationEventArgs(SizeChangeEvent, this) { PageSize = pageSize };
            RaiseEvent(args);
        }

        /// <summary>
        /// currentPage 改变时会触发
        /// </summary>
        public static readonly RoutedEvent CurrentChangeEvent =
            EventManager.RegisterRoutedEvent(nameof(CurrentChange), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(VistaPagination));

        public event RoutedEventHandler CurrentChange
        {
            add => AddHandler(CurrentChangeEvent, value);
            remove => RemoveHandler(CurrentChangeEvent, value);
        }

        protected virtual void OnCurrentChange(int currentPage)
        {
            var args = new PaginationEventArgs(CurrentChangeEvent, this) { CurrentPage = currentPage };
            RaiseEvent(args);
        }

        /// <summary>
        /// 用户点击上一页按钮改变当前页后触发
        /// </summary>
        public static readonly RoutedEvent PrevClickEvent =
            EventManager.RegisterRoutedEvent(nameof(PrevClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(VistaPagination));

        public event RoutedEventHandler PrevClick
        {
            add => AddHandler(PrevClickEvent, value);
            remove => RemoveHandler(PrevClickEvent, value);
        }

        protected virtual void OnPrevClick(int currentPage)
        {
            var args = new PaginationEventArgs(PrevClickEvent, this) { CurrentPage = currentPage };
            RaiseEvent(args);
            OnCurrentChange(currentPage);
        }

        /// <summary>
        /// 用户点击下一页按钮改变当前页后触发
        /// </summary>
        public static readonly RoutedEvent NextClickEvent =
            EventManager.RegisterRoutedEvent(nameof(NextClick), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(VistaPagination));

        public event RoutedEventHandler NextClick
        {
            add => AddHandler(NextClickEvent, value);
            remove => RemoveHandler(NextClickEvent, value);
        }

        protected virtual void OnNextClick(int currentPage)
        {
            var args = new PaginationEventArgs(NextClickEvent, this) { CurrentPage = currentPage };
            RaiseEvent(args);
            OnCurrentChange(currentPage);
        }

        #endregion

        #region 私有方法

        private static object CoercePagerCount(DependencyObject d, object baseValue)
        {
            if (baseValue is int count)
            {
                // 必须是大于等于 5 且小于等于 21 的奇数
                if (count < 5) return 5;
                if (count > 21) return 21;
                if (count % 2 == 0) return count + 1; // 如果是偶数，加1变成奇数
                return count;
            }
            return 7;
        }

        private static object CoerceCurrentPage(DependencyObject d, object baseValue)
        {
            if (d is VistaPagination pagination && baseValue is int page)
            {
                var totalPages = pagination.GetTotalPages();
                return Math.Max(1, Math.Min(totalPages, page));
            }
            return 1;
        }

        private static void OnAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaPagination pagination)
            {
                pagination.UpdatePagination();
            }
        }

        private static void OnPaginationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaPagination pagination)
            {
                pagination.UpdatePagination();
            }
        }

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaPagination pagination)
            {
                pagination.UpdatePagination();
                if (e.NewValue is int newPage)
                {
                    pagination.OnCurrentChange(newPage);
                }
            }
        }

        private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaPagination pagination)
            {
                pagination.UpdatePaginationLayout();
            }
        }

        private static void OnPageSizesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaPagination pagination)
            {
                pagination.UpdateSizesSelect();
            }
        }

        private int GetTotalPages()
        {
            if (PageCount > 0)
            {
                return PageCount;
            }
            if (Total > 0 && PageSize > 0)
            {
                return (int)Math.Ceiling((double)Total / PageSize);
            }
            return 0;
        }

        private void UpdatePagination()
        {
            TotalPages = GetTotalPages();
            UpdatePagerButtons();
            UpdateButtonsState();
            UpdateTotalText();
        }

        private void UpdatePagerButtons()
        {
            var totalPages = GetTotalPages();
            var buttons = new ObservableCollection<PagerButtonInfo>();

            if (totalPages <= 0)
            {
                PagerButtons = buttons;
                return;
            }

            if (totalPages <= PagerCount)
            {
                // 总页数不超过最大按钮数，显示所有页码
                for (int i = 1; i <= totalPages; i++)
                {
                    buttons.Add(new PagerButtonInfo
                    {
                        Page = i,
                        Text = i.ToString(),
                        IsCurrent = i == CurrentPage
                    });
                }
            }
            else
            {
                // 总页数超过最大按钮数，需要折叠
                var halfCount = PagerCount / 2;

                if (CurrentPage <= halfCount + 1)
                {
                    // 当前页在左侧，显示前几页和最后几页
                    for (int i = 1; i <= PagerCount - 2; i++)
                    {
                        buttons.Add(new PagerButtonInfo
                        {
                            Page = i,
                            Text = i.ToString(),
                            IsCurrent = i == CurrentPage
                        });
                    }
                    buttons.Add(new PagerButtonInfo { Page = -1, Text = "...", IsEllipsis = true });
                    buttons.Add(new PagerButtonInfo
                    {
                        Page = totalPages,
                        Text = totalPages.ToString(),
                        IsCurrent = totalPages == CurrentPage
                    });
                }
                else if (CurrentPage >= totalPages - halfCount)
                {
                    // 当前页在右侧，显示第一页和后几页
                    buttons.Add(new PagerButtonInfo
                    {
                        Page = 1,
                        Text = "1",
                        IsCurrent = 1 == CurrentPage
                    });
                    buttons.Add(new PagerButtonInfo { Page = -1, Text = "...", IsEllipsis = true });
                    for (int i = totalPages - (PagerCount - 3); i <= totalPages; i++)
                    {
                        buttons.Add(new PagerButtonInfo
                        {
                            Page = i,
                            Text = i.ToString(),
                            IsCurrent = i == CurrentPage
                        });
                    }
                }
                else
                {
                    // 当前页在中间
                    buttons.Add(new PagerButtonInfo
                    {
                        Page = 1,
                        Text = "1",
                        IsCurrent = 1 == CurrentPage
                    });
                    buttons.Add(new PagerButtonInfo { Page = -1, Text = "...", IsEllipsis = true });
                    
                    var startPage = CurrentPage - halfCount + 2;
                    var endPage = CurrentPage + halfCount - 2;
                    for (int i = startPage; i <= endPage; i++)
                    {
                        buttons.Add(new PagerButtonInfo
                        {
                            Page = i,
                            Text = i.ToString(),
                            IsCurrent = i == CurrentPage
                        });
                    }
                    
                    buttons.Add(new PagerButtonInfo { Page = -1, Text = "...", IsEllipsis = true });
                    buttons.Add(new PagerButtonInfo
                    {
                        Page = totalPages,
                        Text = totalPages.ToString(),
                        IsCurrent = totalPages == CurrentPage
                    });
                }
            }

            PagerButtons = buttons;
            
            // 更新后重新绑定按钮事件
            Dispatcher.BeginInvoke(new Action(() => AttachPagerButtonEvents()), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void UpdateButtonsState()
        {
            var totalPages = GetTotalPages();
            
            if (_prevButton != null)
            {
                _prevButton.IsEnabled = !IsDisabled && CurrentPage > 1;
            }

            if (_nextButton != null)
            {
                _nextButton.IsEnabled = !IsDisabled && CurrentPage < totalPages;
            }
        }

        private void UpdateTotalText()
        {
            if (_totalText != null)
            {
                _totalText.Text = $"共 {Total} 条";
            }
        }

        private void UpdatePaginationLayout()
        {
            // 布局更新在 XAML 中通过触发器处理
        }

        private void UpdateSizesSelect()
        {
            if (_sizesSelect != null && PageSizes != null && PageSizes.Count > 0)
            {
                var options = PageSizes.Select(size => new VistaOption
                {
                    Label = $"{size} 条/页",
                    Value = size
                }).ToList();
                _sizesSelect.ItemsSource = options;
                if (!PageSizes.Contains(PageSize))
                {
                    // 如果当前 PageSize 不在选项中，使用第一个选项
                    SetCurrentValue(PageSizeProperty, PageSizes[0]);
                }
                _sizesSelect.SelectedValue = PageSize;
            }
        }

        #endregion
    }

    /// <summary>
    /// 页码按钮信息
    /// </summary>
    public class PagerButtonInfo
    {
        public int Page { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCurrent { get; set; }
        public bool IsEllipsis { get; set; }
    }

    /// <summary>
    /// 分页事件参数
    /// </summary>
    public class PaginationEventArgs : RoutedEventArgs
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginationEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
        }
    }
}

