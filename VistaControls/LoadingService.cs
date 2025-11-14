using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VistaControls
{
    /// <summary>
    /// LoadingService - 加载服务，用于以服务方式调用 Loading
    /// </summary>
    public class LoadingService
    {
        private static LoadingInstance? _fullscreenInstance;

        /// <summary>
        /// 创建 Loading 实例
        /// </summary>
        /// <param name="options">Loading 配置选项</param>
        /// <returns>Loading 实例，可通过 Close 方法关闭</returns>
        public static LoadingInstance Service(LoadingOptions? options = null)
        {
            options ??= new LoadingOptions();

            // 如果指定了目标元素，创建针对该元素的 Loading
            if (options.Target != null)
            {
                return CreateTargetLoading(options);
            }

            // 全屏 Loading
            if (options.Fullscreen)
            {
                // 全屏 Loading 是单例的
                if (_fullscreenInstance != null && _fullscreenInstance.IsActive)
                {
                    return _fullscreenInstance;
                }

                _fullscreenInstance = CreateFullscreenLoading(options);
                return _fullscreenInstance;
            }

            // 默认创建全屏 Loading
            if (_fullscreenInstance != null && _fullscreenInstance.IsActive)
            {
                return _fullscreenInstance;
            }

            _fullscreenInstance = CreateFullscreenLoading(options);
            return _fullscreenInstance;
        }

        /// <summary>
        /// 创建针对指定元素的 Loading
        /// </summary>
        private static LoadingInstance CreateTargetLoading(LoadingOptions options)
        {
            var target = options.Target!;
            var overlay = new Border
            {
                Background = options.Background ?? new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var content = CreateLoadingContent(options);

            overlay.Child = content;

            // 使用 Adorner 或直接添加到目标元素的父容器
            if (target is FrameworkElement parent)
            {
                // 尝试找到 Grid 或 Panel 作为父容器
                Panel? container = null;
                
                // 如果目标元素本身就是 Panel，直接使用
                if (parent is Panel panel)
                {
                    container = panel;
                }
                else
                {
                    // 尝试从父级获取 Panel
                    var visualParent = System.Windows.Media.VisualTreeHelper.GetParent(parent) as Panel;
                    if (visualParent != null)
                    {
                        container = visualParent;
                    }
                }

                if (container != null)
                {
                    // 创建一个覆盖层容器
                    var overlayContainer = new Grid
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    // 如果目标元素本身就是容器，将覆盖层添加到容器中
                    if (parent == container)
                    {
                        container.Children.Add(overlayContainer);
                    }
                    else
                    {
                        // 获取目标元素在容器中的位置
                        var index = container.Children.IndexOf(parent);
                        if (index >= 0)
                        {
                            container.Children.Insert(index + 1, overlayContainer);
                        }
                        else
                        {
                            container.Children.Add(overlayContainer);
                        }
                    }

                    overlayContainer.Children.Add(overlay);

                    return new LoadingInstance(overlayContainer, options.Lock);
                }
            }

            // 如果无法添加到父容器，创建 Popup
            var popup = new Popup
            {
                Placement = System.Windows.Controls.Primitives.PlacementMode.Relative,
                PlacementTarget = target,
                StaysOpen = true,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.None
            };

            var popupContent = new Border
            {
                Background = options.Background ?? new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                Width = target is FrameworkElement fe ? fe.ActualWidth : 0,
                Height = target is FrameworkElement fe2 ? fe2.ActualHeight : 0
            };

            popupContent.Child = CreateLoadingContent(options);
            popup.Child = popupContent;
            popup.IsOpen = true;

            return new LoadingInstance(popup, options.Lock);
        }

        /// <summary>
        /// 创建全屏 Loading
        /// </summary>
        private static LoadingInstance CreateFullscreenLoading(LoadingOptions options)
        {
            var window = Application.Current.MainWindow;
            if (window == null)
            {
                throw new InvalidOperationException("无法找到主窗口。请确保 Application.Current.MainWindow 已设置。");
            }

            // 创建全屏覆盖层
            var overlay = new Border
            {
                Background = options.Background ?? new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            var content = CreateLoadingContent(options);
            overlay.Child = content;

            // 添加到主窗口
            Panel? container = null;
            if (window.Content is Panel panel)
            {
                container = panel;
            }
            else if (window.Content is FrameworkElement fe)
            {
                // 如果主窗口内容不是 Panel，创建一个新的 Grid 容器
                var grid = new Grid();
                var originalContent = window.Content;
                window.Content = null;
                grid.Children.Add(originalContent as UIElement ?? new ContentPresenter { Content = originalContent });
                window.Content = grid;
                container = grid;
            }

            if (container == null)
            {
                var grid = new Grid();
                window.Content = grid;
                container = grid;
            }

            // 将覆盖层添加到容器的最上层
            container.Children.Add(overlay);

            // 如果锁定，禁用窗口交互
            if (options.Lock)
            {
                window.IsEnabled = false;
            }

            return new LoadingInstance(overlay, options.Lock, window);
        }

        /// <summary>
        /// 创建加载内容（旋转动画 + 文本）
        /// </summary>
        private static UIElement CreateLoadingContent(LoadingOptions options)
        {
            var stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical
            };

            // 创建旋转的加载图标
            var spinner = new Path
            {
                Data = Geometry.Parse("M 50,50 A 40,40 0 1,1 50,10"),
                Stroke = new SolidColorBrush(Color.FromRgb(64, 158, 255)),
                StrokeThickness = 4,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                Width = 50,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var rotateTransform = new RotateTransform();
            spinner.RenderTransform = rotateTransform;
            spinner.RenderTransformOrigin = new Point(0.5, 0.5);

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);

            stackPanel.Children.Add(spinner);

            // 添加加载文本
            if (!string.IsNullOrEmpty(options.Text))
            {
                var textBlock = new TextBlock
                {
                    Text = options.Text,
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Color.FromRgb(64, 158, 255)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                stackPanel.Children.Add(textBlock);
            }

            return stackPanel;
        }
    }

    /// <summary>
    /// Loading 配置选项
    /// </summary>
    public class LoadingOptions
    {
        /// <summary>
        /// Loading 需要覆盖的 DOM 节点（在 WPF 中为 FrameworkElement）
        /// </summary>
        public FrameworkElement? Target { get; set; }

        /// <summary>
        /// 同 v-loading 指令中的 body 修饰符（在 WPF 中暂不支持）
        /// </summary>
        public bool Body { get; set; } = false;

        /// <summary>
        /// 全屏 Loading
        /// </summary>
        public bool Fullscreen { get; set; } = true;

        /// <summary>
        /// 是否锁定（禁用交互）
        /// </summary>
        public bool Lock { get; set; } = true;

        /// <summary>
        /// 显示在加载图标下方的加载文案
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// 自定义加载图标类名（在 WPF 中暂不支持，使用默认图标）
        /// </summary>
        public string? Spinner { get; set; }

        /// <summary>
        /// 遮罩背景色
        /// </summary>
        public Brush? Background { get; set; }

        /// <summary>
        /// Loading 的自定义类名
        /// </summary>
        public string? CustomClass { get; set; }
    }

    /// <summary>
    /// Loading 实例，可通过 Close 方法关闭
    /// </summary>
    public class LoadingInstance
    {
        private readonly UIElement _overlay;
        private readonly bool _lock;
        private readonly Window? _window;

        internal LoadingInstance(UIElement overlay, bool lockEnabled, Window? window = null)
        {
            _overlay = overlay;
            _lock = lockEnabled;
            _window = window;
            IsActive = true;
        }

        /// <summary>
        /// Loading 是否处于活动状态
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// 关闭 Loading
        /// </summary>
        public void Close()
        {
            if (!IsActive) return;

            IsActive = false;

            // 从父容器中移除
            var parent = System.Windows.Media.VisualTreeHelper.GetParent(_overlay);
            if (parent is Panel panel)
            {
                panel.Children.Remove(_overlay);
            }
            else if (parent is Grid grid)
            {
                grid.Children.Remove(_overlay);
            }
            else if (_overlay is Popup popup)
            {
                popup.IsOpen = false;
            }

            // 如果锁定了窗口，恢复窗口交互
            if (_lock && _window != null)
            {
                _window.IsEnabled = true;
            }
        }
    }
}

