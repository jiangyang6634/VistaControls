using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VistaControls
{
    /// <summary>
    /// VistaTag - 标签控件
    /// </summary>
    public class VistaTag : ContentControl
    {
        private Button? _closeButton;

        static VistaTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaTag),
                new FrameworkPropertyMetadata(typeof(VistaTag)));
        }

        public VistaTag()
        {
            // 点击事件
            PreviewMouseDown += VistaTag_PreviewMouseDown;
            Cursor = Cursors.Hand;
        }

        private void VistaTag_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 检查是否点击了关闭按钮
            var source = e.OriginalSource as FrameworkElement;
            while (source != null)
            {
                if (source is Button button && button == _closeButton)
                {
                    // 点击的是关闭按钮，不触发 click 事件
                    return;
                }
                source = source.Parent as FrameworkElement;
            }

            // 触发 click 事件
            OnClick();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 绑定关闭按钮
            if (GetTemplateChild("PART_CloseButton") is Button closeButton)
            {
                _closeButton = closeButton;
                _closeButton.Click -= CloseButton_Click;
                _closeButton.Click += CloseButton_Click;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OnClose();
        }

        #region 依赖属性

        /// <summary>
        /// 标签类型：success, info, warning, danger
        /// </summary>
        public static readonly DependencyProperty TagTypeProperty =
            DependencyProperty.Register(nameof(TagType), typeof(TagType), typeof(VistaTag),
                new PropertyMetadata(TagType.Default));

        /// <summary>
        /// 是否可关闭
        /// </summary>
        public static readonly DependencyProperty ClosableProperty =
            DependencyProperty.Register(nameof(Closable), typeof(bool), typeof(VistaTag),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否禁用渐变动画
        /// </summary>
        public static readonly DependencyProperty DisableTransitionsProperty =
            DependencyProperty.Register(nameof(DisableTransitions), typeof(bool), typeof(VistaTag),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否有边框描边
        /// </summary>
        public static readonly DependencyProperty HitProperty =
            DependencyProperty.Register(nameof(Hit), typeof(bool), typeof(VistaTag),
                new PropertyMetadata(false));

        /// <summary>
        /// 自定义背景色
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(VistaTag),
                new PropertyMetadata(null));

        /// <summary>
        /// 自定义文字颜色
        /// </summary>
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(nameof(TextColor), typeof(Brush), typeof(VistaTag),
                new PropertyMetadata(null));

        #endregion

        #region 属性

        public TagType TagType
        {
            get => (TagType)GetValue(TagTypeProperty);
            set => SetValue(TagTypeProperty, value);
        }

        public bool Closable
        {
            get => (bool)GetValue(ClosableProperty);
            set => SetValue(ClosableProperty, value);
        }

        public bool DisableTransitions
        {
            get => (bool)GetValue(DisableTransitionsProperty);
            set => SetValue(DisableTransitionsProperty, value);
        }

        public bool Hit
        {
            get => (bool)GetValue(HitProperty);
            set => SetValue(HitProperty, value);
        }

        public Brush? Color
        {
            get => (Brush?)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Brush? TextColor
        {
            get => (Brush?)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 点击事件
        /// </summary>
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(VistaTag));

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        protected virtual void OnClick()
        {
            var args = new RoutedEventArgs(ClickEvent, this);
            RaiseEvent(args);
        }

        /// <summary>
        /// 关闭事件
        /// </summary>
        public static readonly RoutedEvent CloseEvent =
            EventManager.RegisterRoutedEvent(nameof(Close), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(VistaTag));

        public event RoutedEventHandler Close
        {
            add => AddHandler(CloseEvent, value);
            remove => RemoveHandler(CloseEvent, value);
        }

        protected virtual void OnClose()
        {
            var args = new RoutedEventArgs(CloseEvent, this);
            RaiseEvent(args);
        }

        #endregion
    }

    /// <summary>
    /// 标签类型枚举
    /// </summary>
    public enum TagType
    {
        Default,
        Success,
        Info,
        Warning,
        Danger
    }
}

