using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Dom.Svg;

namespace VistaControls
{
    /// <summary>
    /// VistaButton - 仿 Element UI 风格的按钮控件
    /// </summary>
    public class VistaButton : Button
    {
        static VistaButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaButton),
                new FrameworkPropertyMetadata(typeof(VistaButton)));
        }

        #region 依赖属性

        /// <summary>
        /// 按钮类型：primary, success, info, warning, danger, text
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register(nameof(ButtonType), typeof(ButtonType), typeof(VistaButton),
                new PropertyMetadata(ButtonType.Default, OnButtonTypeChanged));

        /// <summary>
        /// 是否朴素按钮
        /// </summary>
        public static readonly DependencyProperty PlainProperty =
            DependencyProperty.Register(nameof(Plain), typeof(bool), typeof(VistaButton),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否圆角按钮
        /// </summary>
        public static readonly DependencyProperty RoundProperty =
            DependencyProperty.Register(nameof(Round), typeof(bool), typeof(VistaButton),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否圆形按钮
        /// </summary>
        public static readonly DependencyProperty CircleProperty =
            DependencyProperty.Register(nameof(Circle), typeof(bool), typeof(VistaButton),
                new PropertyMetadata(false));

        /// <summary>
        /// 是否加载中状态
        /// </summary>
        public static readonly DependencyProperty LoadingProperty =
            DependencyProperty.Register(nameof(Loading), typeof(bool), typeof(VistaButton),
                new PropertyMetadata(false, OnLoadingChanged));

        /// <summary>
        /// 图标路径（相对于 icons 文件夹）
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(string), typeof(VistaButton),
                new PropertyMetadata(string.Empty, OnIconChanged));

        /// <summary>
        /// 图标源（内部使用）
        /// </summary>
        internal static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(VistaButton),
                new PropertyMetadata(null));

        /// <summary>
        /// 按钮尺寸：medium, small, mini
        /// </summary>
        public static readonly DependencyProperty ButtonSizeProperty =
            DependencyProperty.Register(nameof(ButtonSize), typeof(ButtonSize), typeof(VistaButton),
                new PropertyMetadata(ButtonSize.Default));

        /// <summary>
        /// 原生 type 属性：button, submit, reset
        /// </summary>
        public static readonly DependencyProperty NativeTypeProperty =
            DependencyProperty.Register(nameof(NativeType), typeof(string), typeof(VistaButton),
                new PropertyMetadata("button"));

        /// <summary>
        /// 按钮在按钮组中的位置（内部使用）
        /// </summary>
        internal static readonly DependencyProperty ButtonGroupPositionProperty =
            DependencyProperty.Register(nameof(ButtonGroupPosition), typeof(ButtonGroupPosition), typeof(VistaButton),
                new PropertyMetadata(ButtonGroupPosition.None));

        #endregion

        #region 属性

        public ButtonType ButtonType
        {
            get => (ButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public bool Plain
        {
            get => (bool)GetValue(PlainProperty);
            set => SetValue(PlainProperty, value);
        }

        public bool Round
        {
            get => (bool)GetValue(RoundProperty);
            set => SetValue(RoundProperty, value);
        }

        public bool Circle
        {
            get => (bool)GetValue(CircleProperty);
            set => SetValue(CircleProperty, value);
        }

        public bool Loading
        {
            get => (bool)GetValue(LoadingProperty);
            set => SetValue(LoadingProperty, value);
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        internal ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public ButtonSize ButtonSize
        {
            get => (ButtonSize)GetValue(ButtonSizeProperty);
            set => SetValue(ButtonSizeProperty, value);
        }

        public string NativeType
        {
            get => (string)GetValue(NativeTypeProperty);
            set => SetValue(NativeTypeProperty, value);
        }

        internal ButtonGroupPosition ButtonGroupPosition
        {
            get => (ButtonGroupPosition)GetValue(ButtonGroupPositionProperty);
            set => SetValue(ButtonGroupPositionProperty, value);
        }

        #endregion

        #region 私有方法

        private static void OnButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaButton button)
            {
                button.UpdateButtonStyle();
            }
        }

        private static void OnLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaButton button)
            {
                button.IsEnabled = !button.Loading;
            }
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaButton button && e.NewValue is string iconName)
            {
                button.UpdateIconSource(iconName);
            }
        }

        private void UpdateIconSource(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                IconSource = null!;
                return;
            }

            // 检查是否是SVG文件
            bool isSvg = iconName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase);

            try
            {
                if (isSvg)
                {
                    // 处理SVG图标
                    LoadSvgIcon(iconName);
                }
                else
                {
                    // 处理PNG/其他图片格式
                    LoadBitmapIcon(iconName);
                }
            }
            catch
            {
                IconSource = null!;
            }
        }

        private void LoadBitmapIcon(string iconName)
        {
            try
            {
                // 尝试使用 pack URI 加载资源
                var packUri = new Uri($"pack://application:,,,/VistaControls;component/icons/{iconName}", UriKind.Absolute);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = packUri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                IconSource = bitmap;
            }
            catch
            {
                // 如果 pack URI 失败，尝试从文件系统加载
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyPath = Path.GetDirectoryName(assembly.Location);
                    var iconPath = Path.Combine(assemblyPath ?? "", "icons", iconName);
                    
                    if (File.Exists(iconPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        IconSource = bitmap;
                    }
                    else
                    {
                        IconSource = null!;
                    }
                }
                catch
                {
                    IconSource = null!;
                }
            }
        }

        private void LoadSvgIcon(string iconName)
        {
            try
            {
                // 使用 SharpVectors 加载 SVG
                var settings = new WpfDrawingSettings
                {
                    IncludeRuntime = true,
                    TextAsGeometry = false,
                    OptimizePath = true
                };

                DrawingGroup? drawing = null;
                
                // 尝试从资源加载
                try
                {
                    var packUri = new Uri($"pack://application:,,,/VistaControls;component/icons/{iconName}", UriKind.Absolute);
                    var streamInfo = Application.GetResourceStream(packUri);
                    if (streamInfo != null)
                    {
                        using (var stream = streamInfo.Stream)
                        {
                            var reader = new FileSvgReader(settings);
                            drawing = reader.Read(stream);
                        }
                    }
                }
                catch
                {
                    // 如果资源加载失败，尝试从文件系统加载
                }

                // 从文件系统加载
                if (drawing == null)
                {
                    try
                    {
                        var assembly = Assembly.GetExecutingAssembly();
                        var assemblyPath = Path.GetDirectoryName(assembly.Location);
                        var iconPath = Path.Combine(assemblyPath ?? "", "icons", iconName);
                        
                        if (File.Exists(iconPath))
                        {
                            var reader = new FileSvgReader(settings);
                            drawing = reader.Read(iconPath);
                        }
                    }
                    catch
                    {
                        // 忽略错误
                    }
                }

                if (drawing != null)
                {
                    // 应用按钮的前景色到SVG图标
                    ApplyForegroundToDrawing(drawing, Foreground);
                    
                    // 将 Drawing 转换为 ImageSource
                    var drawingImage = new DrawingImage(drawing);
                    IconSource = drawingImage;
                }
                else
                {
                    IconSource = null!;
                }
            }
            catch
            {
                IconSource = null!;
            }
        }

        /// <summary>
        /// 将按钮的前景色应用到SVG Drawing
        /// </summary>
        private void ApplyForegroundToDrawing(Drawing drawing, Brush foreground)
        {
            if (drawing is DrawingGroup group)
            {
                foreach (var child in group.Children)
                {
                    ApplyForegroundToDrawing(child, foreground);
                }
            }
            else if (drawing is GeometryDrawing geometryDrawing)
            {
                // 如果当前使用的是SolidColorBrush且颜色是白色或接近白色，则替换为按钮前景色
                if (geometryDrawing.Brush is SolidColorBrush brush)
                {
                    var color = brush.Color;
                    // 如果颜色是白色或接近白色（RGB值都大于240），则使用按钮前景色
                    if (color.R > 240 && color.G > 240 && color.B > 240)
                    {
                        geometryDrawing.Brush = foreground;
                    }
                }
                else if (geometryDrawing.Brush == null)
                {
                    geometryDrawing.Brush = foreground;
                }
            }
        }

        private void UpdateButtonStyle()
        {
            // 样式更新逻辑在模板中完成
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            // 如果设置了 NativeType，更新按钮的原生类型
            if (!string.IsNullOrEmpty(NativeType))
            {
                // WPF Button 的 Command 属性会处理提交逻辑
                // NativeType 主要用于样式和语义
            }
            
            // 如果图标是SVG，在模板应用后更新颜色（此时Foreground已设置）
            if (!string.IsNullOrEmpty(Icon) && Icon.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            {
                this.Loaded += (s, e) =>
                {
                    // 延迟更新，确保Foreground已应用
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (IconSource is DrawingImage drawingImage && drawingImage.Drawing is DrawingGroup group)
                        {
                            ApplyForegroundToDrawing(group, Foreground);
                        }
                    }), System.Windows.Threading.DispatcherPriority.Loaded);
                };
            }
        }
    }

    /// <summary>
    /// 按钮类型枚举
    /// </summary>
    public enum ButtonType
    {
        Default,
        Primary,
        Success,
        Info,
        Warning,
        Danger,
        Text
    }

    /// <summary>
    /// 按钮尺寸枚举
    /// </summary>
    public enum ButtonSize
    {
        Default,
        Medium,
        Small,
        Mini
    }

    /// <summary>
    /// 按钮在按钮组中的位置枚举
    /// </summary>
    internal enum ButtonGroupPosition
    {
        None,   // 不在按钮组中
        First,  // 第一个按钮
        Middle, // 中间按钮
        Last,   // 最后一个按钮
        Only    // 只有一个按钮
    }
}

