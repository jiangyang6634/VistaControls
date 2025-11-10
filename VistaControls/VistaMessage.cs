using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace VistaControls
{
    /// <summary>
    /// VistaMessage - 消息提示控件
    /// </summary>
    public class VistaMessage : Control
    {
        private DispatcherTimer? _closeTimer;
        private Storyboard? _fadeOutStoryboard;

        static VistaMessage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaMessage),
                new FrameworkPropertyMetadata(typeof(VistaMessage)));
        }

        public VistaMessage()
        {
            Loaded += VistaMessage_Loaded;
            // 点击整个消息框即可关闭
            PreviewMouseDown += VistaMessage_PreviewMouseDown;
            Cursor = System.Windows.Input.Cursors.Hand;
        }

        private void VistaMessage_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 检查点击的元素
            var source = e.OriginalSource as FrameworkElement;
            
            // 向上查找，看是否点击的是关闭按钮
            while (source != null)
            {
                if (source is System.Windows.Controls.Button button && button.Name == "closeButton")
                {
                    // 点击的是关闭按钮，不处理（让关闭按钮自己处理）
                    return;
                }
                source = source.Parent as FrameworkElement;
            }
            
            // 点击消息框其他区域，关闭消息
            Close();
            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindCloseButton();
        }

        private void BindCloseButton()
        {
            // 绑定关闭按钮的点击事件
            if (GetTemplateChild("closeButton") is System.Windows.Controls.Button closeButton)
            {
                closeButton.Click -= CloseButton_Click;
                closeButton.Click += CloseButton_Click;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VistaMessage_Loaded(object sender, RoutedEventArgs e)
        {
            // 确保关闭按钮已绑定
            BindCloseButton();
            StartCloseTimer();
            PlayFadeInAnimation();
        }

        #region 依赖属性

        /// <summary>
        /// 消息文字
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(VistaMessage),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 消息类型：success, warning, info, error
        /// </summary>
        public static readonly DependencyProperty MessageTypeProperty =
            DependencyProperty.Register(nameof(MessageType), typeof(MessageType), typeof(VistaMessage),
                new PropertyMetadata(MessageType.Info));

        /// <summary>
        /// 是否显示关闭按钮
        /// </summary>
        public static readonly DependencyProperty ShowCloseProperty =
            DependencyProperty.Register(nameof(ShowClose), typeof(bool), typeof(VistaMessage),
                new PropertyMetadata(false));

        /// <summary>
        /// 文字是否居中
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(nameof(Center), typeof(bool), typeof(VistaMessage),
                new PropertyMetadata(false));

        /// <summary>
        /// 显示时间（毫秒），0 表示不自动关闭
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(nameof(Duration), typeof(int), typeof(VistaMessage),
                new PropertyMetadata(3000, OnDurationChanged));

        /// <summary>
        /// 是否将 message 作为 HTML 片段处理
        /// </summary>
        public static readonly DependencyProperty DangerouslyUseHTMLStringProperty =
            DependencyProperty.Register(nameof(DangerouslyUseHTMLString), typeof(bool), typeof(VistaMessage),
                new PropertyMetadata(false));

        /// <summary>
        /// 关闭时的回调函数
        /// </summary>
        public static readonly DependencyProperty OnCloseProperty =
            DependencyProperty.Register(nameof(OnClose), typeof(Action<VistaMessage>), typeof(VistaMessage),
                new PropertyMetadata(null));

        #endregion

        #region 属性

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public MessageType MessageType
        {
            get => (MessageType)GetValue(MessageTypeProperty);
            set => SetValue(MessageTypeProperty, value);
        }

        public bool ShowClose
        {
            get => (bool)GetValue(ShowCloseProperty);
            set => SetValue(ShowCloseProperty, value);
        }

        public bool Center
        {
            get => (bool)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public int Duration
        {
            get => (int)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public bool DangerouslyUseHTMLString
        {
            get => (bool)GetValue(DangerouslyUseHTMLStringProperty);
            set => SetValue(DangerouslyUseHTMLStringProperty, value);
        }

        public Action<VistaMessage>? OnClose
        {
            get => (Action<VistaMessage>?)GetValue(OnCloseProperty);
            set => SetValue(OnCloseProperty, value);
        }

        #endregion

        #region 事件处理

        private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaMessage message)
            {
                message.StartCloseTimer();
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 关闭消息
        /// </summary>
        public void Close()
        {
            PlayFadeOutAnimation(() =>
            {
                OnClose?.Invoke(this);
                
                // 从父容器中移除
                if (Parent is Panel panel)
                {
                    panel.Children.Remove(this);
                }
            });
        }

        #endregion

        #region 私有方法

        private void StartCloseTimer()
        {
            if (_closeTimer != null)
            {
                _closeTimer.Stop();
                _closeTimer = null;
            }

            if (Duration > 0)
            {
                _closeTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(Duration)
                };
                _closeTimer.Tick += (s, e) =>
                {
                    _closeTimer.Stop();
                    Close();
                };
                _closeTimer.Start();
            }
        }

        private void PlayFadeInAnimation()
        {
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            var slideDown = new ThicknessAnimation
            {
                From = new Thickness(0, -50, 0, 0),
                To = new Thickness(0),
                Duration = TimeSpan.FromMilliseconds(300)
            };

            var storyboard = new Storyboard();
            Storyboard.SetTarget(fadeIn, this);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(slideDown, this);
            Storyboard.SetTargetProperty(slideDown, new PropertyPath(MarginProperty));

            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(slideDown);
            storyboard.Begin();
        }

        private void PlayFadeOutAnimation(Action? onCompleted = null)
        {
            if (_fadeOutStoryboard != null)
            {
                return; // 已经在关闭动画中
            }

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200)
            };

            var slideUp = new ThicknessAnimation
            {
                From = new Thickness(0),
                To = new Thickness(0, -50, 0, 0),
                Duration = TimeSpan.FromMilliseconds(200)
            };

            _fadeOutStoryboard = new Storyboard();
            Storyboard.SetTarget(fadeOut, this);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(slideUp, this);
            Storyboard.SetTargetProperty(slideUp, new PropertyPath(MarginProperty));

            _fadeOutStoryboard.Children.Add(fadeOut);
            _fadeOutStoryboard.Children.Add(slideUp);
            
            _fadeOutStoryboard.Completed += (s, e) =>
            {
                onCompleted?.Invoke();
                _fadeOutStoryboard = null;
            };
            
            _fadeOutStoryboard.Begin();
        }

        #endregion
    }

    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        Success,
        Warning,
        Info,
        Error
    }
}

