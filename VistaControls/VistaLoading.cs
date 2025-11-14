using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace VistaControls
{
    /// <summary>
    /// VistaLoading - 加载控件
    /// </summary>
    public class VistaLoading : ContentControl
    {
        private Border? _loadingOverlay;
        private Border? _loadingContent;
        private Path? _spinnerPath;
        private TextBlock? _loadingText;

        static VistaLoading()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaLoading),
                new FrameworkPropertyMetadata(typeof(VistaLoading)));
        }

        public VistaLoading()
        {
            Loaded += VistaLoading_Loaded;
        }

        private void VistaLoading_Loaded(object sender, RoutedEventArgs e)
        {
            StartSpinnerAnimation();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _loadingOverlay = GetTemplateChild("PART_LoadingOverlay") as Border;
            _loadingContent = GetTemplateChild("PART_LoadingContent") as Border;
            _spinnerPath = GetTemplateChild("PART_Spinner") as Path;
            _loadingText = GetTemplateChild("PART_LoadingText") as TextBlock;

            StartSpinnerAnimation();
        }

        private void StartSpinnerAnimation()
        {
            if (_spinnerPath == null) return;

            // 创建旋转动画
            var rotateTransform = new RotateTransform();
            _spinnerPath.RenderTransform = rotateTransform;
            _spinnerPath.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        #region 依赖属性

        /// <summary>
        /// 是否显示加载遮罩
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(VistaLoading),
                new PropertyMetadata(false));

        /// <summary>
        /// 加载文本
        /// </summary>
        public static readonly DependencyProperty LoadingTextProperty =
            DependencyProperty.Register(nameof(LoadingText), typeof(string), typeof(VistaLoading),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 遮罩背景色
        /// </summary>
        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(VistaLoading),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))));

        /// <summary>
        /// 自定义类名
        /// </summary>
        public static readonly DependencyProperty CustomClassProperty =
            DependencyProperty.Register(nameof(CustomClass), typeof(string), typeof(VistaLoading),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 是否锁定（禁用交互）
        /// </summary>
        public static readonly DependencyProperty LockProperty =
            DependencyProperty.Register(nameof(Lock), typeof(bool), typeof(VistaLoading),
                new PropertyMetadata(true));

        #endregion

        #region 属性

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public string LoadingText
        {
            get => (string)GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }

        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public string CustomClass
        {
            get => (string)GetValue(CustomClassProperty);
            set => SetValue(CustomClassProperty, value);
        }

        public bool Lock
        {
            get => (bool)GetValue(LockProperty);
            set => SetValue(LockProperty, value);
        }

        #endregion
    }
}

