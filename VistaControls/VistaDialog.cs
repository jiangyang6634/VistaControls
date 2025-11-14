using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VistaControls
{
    /// <summary>
    /// VistaDialog - 模态弹窗
    /// </summary>
    public class VistaDialog : ContentControl
    {
        private Grid? _root;
        private Border? _dialogContainer;
        private FrameworkElement? _modalOverlay;
        private Button? _closeButton;
        private Button? _addButtonPlaceholder;
        private ContentPresenter? _footerPresenter;
        private bool _suppressVisibilityChange;
        private bool _forceCloseWithoutBefore;
        private object? _cachedContent;
        private bool _contentDestroyed;

        static VistaDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaDialog),
                new FrameworkPropertyMetadata(typeof(VistaDialog)));
        }

        public VistaDialog()
        {
            Loaded += VistaDialog_Loaded;
            Unloaded += VistaDialog_Unloaded;
        }

        private void VistaDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown += Window_PreviewKeyDown;
                window.SizeChanged += Window_SizeChanged;
            }

            UpdateVisualState(false);
            UpdateDialogLayout();
        }

        private void VistaDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown -= Window_PreviewKeyDown;
                window.SizeChanged -= Window_SizeChanged;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateDialogLayout();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_modalOverlay != null)
            {
                _modalOverlay.MouseLeftButtonDown -= ModalOverlay_MouseLeftButtonDown;
            }

            if (_closeButton != null)
            {
                _closeButton.Click -= CloseButton_Click;
            }

            _root = GetTemplateChild("PART_Root") as Grid;
            _dialogContainer = GetTemplateChild("PART_DialogContainer") as Border;
            _modalOverlay = GetTemplateChild("PART_Modal") as FrameworkElement;
            _closeButton = GetTemplateChild("PART_CloseButton") as Button;
            _footerPresenter = GetTemplateChild("PART_FooterPresenter") as ContentPresenter;

            if (_modalOverlay != null)
            {
                _modalOverlay.MouseLeftButtonDown += ModalOverlay_MouseLeftButtonDown;
            }

            if (_closeButton != null)
            {
                _closeButton.Click += CloseButton_Click;
            }

            UpdateFooterContent();
            UpdateVisualState(false);
            UpdateDialogLayout();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsVisibleDialog || !CloseOnPressEscape)
                return;

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CloseDialog();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void ModalOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CloseOnClickModal)
            {
                CloseDialog();
            }
        }

        private bool IsVisibleDialog => Visible;

        private static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDialog dialog)
            {
                if (dialog._suppressVisibilityChange)
                    return;

                bool isVisible = (bool)e.NewValue;
                if (isVisible)
                {
                    dialog.ShowInternal();
                }
                else
                {
                    if (dialog._forceCloseWithoutBefore)
                    {
                        dialog._forceCloseWithoutBefore = false;
                        dialog.HideInternalCore();
                    }
                    else
                    {
                        dialog.HideInternal();
                    }
                }
            }
        }

        private void ShowInternal()
        {
            if (_contentDestroyed && _cachedContent != null)
            {
                Content = _cachedContent;
                _contentDestroyed = false;
            }

            _root?.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            UpdateVisualState(true);
            UpdateDialogLayout();

            Open?.Invoke(this, EventArgs.Empty);
            Dispatcher.BeginInvoke(new Action(() => Opened?.Invoke(this, EventArgs.Empty)),
                System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void HideInternal()
        {
            var args = new DialogBeforeCloseEventArgs(ForceCloseAfterBeforeClose);
            BeforeClose?.Invoke(this, args);

            if (args.Cancel)
            {
                _suppressVisibilityChange = true;
                SetCurrentValue(VisibleProperty, true);
                _suppressVisibilityChange = false;
                return;
            }

            HideInternalCore();
        }

        private void HideInternalCore()
        {
            Close?.Invoke(this, EventArgs.Empty);

            if (DestroyOnClose && Content != null)
            {
                _cachedContent = Content;
                Content = null;
                _contentDestroyed = true;
            }

            _root?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            UpdateVisualState(false);

            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void ForceCloseAfterBeforeClose()
        {
            if (Visible)
            {
                _forceCloseWithoutBefore = true;
                SetCurrentValue(VisibleProperty, false);
            }
        }

        private void CloseDialog()
        {
            if (!Visible)
                return;

            SetCurrentValue(VisibleProperty, false);
        }

        private void UpdateVisualState(bool opening)
        {
            if (_root == null)
                return;

            _root.Visibility = Visible ? Visibility.Visible : Visibility.Collapsed;

            if (_modalOverlay != null)
            {
                _modalOverlay.Visibility = Modal ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateFooterContent()
        {
            if (_footerPresenter == null)
                return;

            if (Footer != null)
            {
                _footerPresenter.Content = Footer;
            }
        }

        private void UpdateDialogLayout()
        {
            if (_dialogContainer == null || _root == null)
                return;

            if (Fullscreen)
            {
                _dialogContainer.Width = double.NaN;
                _dialogContainer.Margin = new Thickness(0);
                _dialogContainer.HorizontalAlignment = HorizontalAlignment.Stretch;
                _dialogContainer.VerticalAlignment = VerticalAlignment.Stretch;
            }
            else
            {
                var parent = Window.GetWindow(this);
                double availableWidth = _root.ActualWidth;
                if (availableWidth <= 0 && parent != null)
                {
                    availableWidth = parent.ActualWidth;
                }

                double targetWidth = CalculateLength(DialogWidth, availableWidth, availableWidth * 0.5);
                _dialogContainer.Width = double.IsNaN(targetWidth) ? availableWidth * 0.5 : targetWidth;

                double availableHeight = _root.ActualHeight;
                if (availableHeight <= 0 && parent != null)
                {
                    availableHeight = parent.ActualHeight;
                }
                double topMargin = CalculateLength(Top, availableHeight, availableHeight * 0.15);
                if (double.IsNaN(topMargin))
                {
                    topMargin = availableHeight * 0.15;
                }

                _dialogContainer.VerticalAlignment = VerticalAlignment.Top;
                _dialogContainer.HorizontalAlignment = HorizontalAlignment.Center;
                _dialogContainer.Margin = new Thickness(0, topMargin, 0, 40);
            }
        }

        private double CalculateLength(string? input, double relative, double defaultValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                return defaultValue;

            var value = input.Trim().ToLowerInvariant();

            try
            {
                if (value.EndsWith("%"))
                {
                    if (double.TryParse(value.TrimEnd('%'), out double percent))
                    {
                        return relative * percent / 100.0;
                    }
                }
                else if (value.EndsWith("vh"))
                {
                    if (double.TryParse(value[..^2], out double vh))
                    {
                        return relative * vh / 100.0;
                    }
                }
                else if (value.EndsWith("px"))
                {
                    if (double.TryParse(value[..^2], out double px))
                    {
                        return px;
                    }
                }
                else
                {
                    if (double.TryParse(value, out double direct))
                    {
                        return direct;
                    }
                }
            }
            catch
            {
                return defaultValue;
            }

            return defaultValue;
        }

        #region 依赖属性

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(nameof(Visible), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(false, OnIsVisibleChanged));

        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(VistaDialog),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleContentProperty =
            DependencyProperty.Register(nameof(TitleContent), typeof(object), typeof(VistaDialog),
                new PropertyMetadata(null));

        public object? TitleContent
        {
            get => GetValue(TitleContentProperty);
            set => SetValue(TitleContentProperty, value);
        }

        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(VistaDialog),
                new PropertyMetadata(null));

        public DataTemplate? TitleTemplate
        {
            get => (DataTemplate?)GetValue(TitleTemplateProperty);
            set => SetValue(TitleTemplateProperty, value);
        }

        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register(nameof(DialogWidth), typeof(string), typeof(VistaDialog),
                new PropertyMetadata("50%", OnDialogLayoutPropertyChanged));

        public string DialogWidth
        {
            get => (string)GetValue(DialogWidthProperty);
            set => SetValue(DialogWidthProperty, value);
        }

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(nameof(Top), typeof(string), typeof(VistaDialog),
                new PropertyMetadata("120", OnDialogLayoutPropertyChanged));

        public string Top
        {
            get => (string)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public static readonly DependencyProperty FullscreenProperty =
            DependencyProperty.Register(nameof(Fullscreen), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(false, OnDialogLayoutPropertyChanged));

        public bool Fullscreen
        {
            get => (bool)GetValue(FullscreenProperty);
            set => SetValue(FullscreenProperty, value);
        }

        public static readonly DependencyProperty ModalProperty =
            DependencyProperty.Register(nameof(Modal), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(true, OnModalChanged));

        public bool Modal
        {
            get => (bool)GetValue(ModalProperty);
            set => SetValue(ModalProperty, value);
        }

        public static readonly DependencyProperty ModalAppendToBodyProperty =
            DependencyProperty.Register(nameof(ModalAppendToBody), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(true));

        public bool ModalAppendToBody
        {
            get => (bool)GetValue(ModalAppendToBodyProperty);
            set => SetValue(ModalAppendToBodyProperty, value);
        }

        public static readonly DependencyProperty AppendToBodyProperty =
            DependencyProperty.Register(nameof(AppendToBody), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(false));

        public bool AppendToBody
        {
            get => (bool)GetValue(AppendToBodyProperty);
            set => SetValue(AppendToBodyProperty, value);
        }

        public static readonly DependencyProperty LockScrollProperty =
            DependencyProperty.Register(nameof(LockScroll), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(true));

        public bool LockScroll
        {
            get => (bool)GetValue(LockScrollProperty);
            set => SetValue(LockScrollProperty, value);
        }

        public static readonly DependencyProperty CustomClassProperty =
            DependencyProperty.Register(nameof(CustomClass), typeof(string), typeof(VistaDialog),
                new PropertyMetadata(string.Empty));

        public string CustomClass
        {
            get => (string)GetValue(CustomClassProperty);
            set => SetValue(CustomClassProperty, value);
        }

        public static readonly DependencyProperty CloseOnClickModalProperty =
            DependencyProperty.Register(nameof(CloseOnClickModal), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(true));

        public bool CloseOnClickModal
        {
            get => (bool)GetValue(CloseOnClickModalProperty);
            set => SetValue(CloseOnClickModalProperty, value);
        }

        public static readonly DependencyProperty CloseOnPressEscapeProperty =
            DependencyProperty.Register(nameof(CloseOnPressEscape), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(true));

        public bool CloseOnPressEscape
        {
            get => (bool)GetValue(CloseOnPressEscapeProperty);
            set => SetValue(CloseOnPressEscapeProperty, value);
        }

        public static readonly DependencyProperty ShowCloseProperty =
            DependencyProperty.Register(nameof(ShowClose), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(true));

        public bool ShowClose
        {
            get => (bool)GetValue(ShowCloseProperty);
            set => SetValue(ShowCloseProperty, value);
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(nameof(Center), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(false));

        public bool Center
        {
            get => (bool)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly DependencyProperty DestroyOnCloseProperty =
            DependencyProperty.Register(nameof(DestroyOnClose), typeof(bool), typeof(VistaDialog),
                new PropertyMetadata(false));

        public bool DestroyOnClose
        {
            get => (bool)GetValue(DestroyOnCloseProperty);
            set => SetValue(DestroyOnCloseProperty, value);
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register(nameof(Footer), typeof(object), typeof(VistaDialog),
                new PropertyMetadata(null, OnFooterChanged));

        public object? Footer
        {
            get => GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        public static readonly DependencyProperty FooterTemplateProperty =
            DependencyProperty.Register(nameof(FooterTemplate), typeof(DataTemplate), typeof(VistaDialog),
                new PropertyMetadata(null));

        public DataTemplate? FooterTemplate
        {
            get => (DataTemplate?)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        #endregion

        #region 事件

        public event EventHandler? Open;
        public event EventHandler? Opened;
        public event EventHandler? Close;
        public event EventHandler? Closed;
        public event EventHandler<DialogBeforeCloseEventArgs>? BeforeClose;

        #endregion

        private static void OnFooterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDialog dialog)
            {
                dialog.UpdateFooterContent();
            }
        }

        private static void OnDialogLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDialog dialog)
            {
                dialog.UpdateDialogLayout();
            }
        }

        private static void OnModalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaDialog dialog)
            {
                dialog.UpdateVisualState(dialog.Visible);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!Visible || !CloseOnPressEscape)
                return;

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CloseDialog();
            }
        }
    }

    public class DialogBeforeCloseEventArgs : EventArgs
    {
        private readonly Action _closeAction;

        public DialogBeforeCloseEventArgs(Action closeAction)
        {
            _closeAction = closeAction;
        }

        /// <summary>
        /// 是否取消关闭
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 调用后立即关闭弹窗（不会再次触发 BeforeClose）
        /// </summary>
        public void Close()
        {
            _closeAction?.Invoke();
        }
    }
}

