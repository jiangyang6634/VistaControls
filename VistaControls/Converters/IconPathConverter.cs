using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VistaControls.Converters
{
    /// <summary>
    /// 将图标文件名转换为完整路径的转换器
    /// </summary>
    public class IconPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string iconName && !string.IsNullOrEmpty(iconName))
            {
                try
                {
                    // 获取当前程序集的位置
                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyPath = Path.GetDirectoryName(assembly.Location);
                    
                    // 构建图标路径
                    var iconPath = Path.Combine(assemblyPath ?? "", "icons", iconName);
                    
                    // 如果文件存在，返回 BitmapImage
                    if (File.Exists(iconPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        return bitmap;
                    }
                    
                    // 如果文件不存在，尝试使用 pack URI（适用于嵌入资源）
                    var packUri = new Uri($"pack://application:,,,/VistaControls;component/icons/{iconName}", UriKind.Absolute);
                    try
                    {
                        var bitmap2 = new BitmapImage();
                        bitmap2.BeginInit();
                        bitmap2.UriSource = packUri;
                        bitmap2.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap2.EndInit();
                        return bitmap2;
                    }
                    catch
                    {
                        // 如果 pack URI 也失败，返回 null
                        return null!;
                    }
                }
                catch
                {
                    return null!;
                }
            }
            
            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将字符串转换为 Visibility 的转换器
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                return System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将 null 转换为 Visibility 的转换器（null 时隐藏，非 null 时显示）
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将布尔值转换为圆角半径的转换器
    /// </summary>
    public class BoolToCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isRound && isRound)
            {
                if (parameter is string radiusStr && double.TryParse(radiusStr, out var radius))
                {
                    return new System.Windows.CornerRadius(radius);
                }
                return new System.Windows.CornerRadius(20);
            }
            return new System.Windows.CornerRadius(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将布尔值转换为 Visibility 的转换器
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Windows.Visibility visibility)
            {
                return visibility == System.Windows.Visibility.Visible;
            }
            return false;
        }
    }

    /// <summary>
    /// 将消息类型转换为颜色的转换器
    /// </summary>
    public class MessageTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VistaControls.MessageType messageType)
            {
                return messageType switch
                {
                    VistaControls.MessageType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#67C23A")),
                    VistaControls.MessageType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6A23C")),
                    VistaControls.MessageType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#909399")),
                    VistaControls.MessageType.Error => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F56C6C")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#909399"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#909399"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将布尔值转换为 HorizontalAlignment 的转换器
    /// </summary>
    public class BoolToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return HorizontalAlignment.Center;
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 字数统计转换器
    /// </summary>
    public class WordLimitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is string text && values[1] is int maxLength)
            {
                int currentLength = text?.Length ?? 0;
                return $"{currentLength} / {maxLength}";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 字符串到占位符可见性转换器（空字符串时显示占位符）
    /// </summary>
    public class StringToPlaceholderVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text && string.IsNullOrEmpty(text))
            {
                return System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Slider 进度条宽度转换器
    /// </summary>
    public class SliderProgressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 4 && 
                values[0] is double value && 
                values[1] is double min && 
                values[2] is double max && 
                values[3] is double trackWidth)
            {
                if (max == min) return 0.0;
                
                double percentage = (value - min) / (max - min);
                return Math.Max(0, Math.Min(trackWidth, trackWidth * percentage));
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Slider 间断点位置转换器
    /// </summary>
    public class SliderStopPositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 4 && 
                values[0] is double stopValue && 
                values[1] is double min && 
                values[2] is double max && 
                values[3] is double trackWidth)
            {
                if (max == min) return 0.0;
                
                double percentage = (stopValue - min) / (max - min);
                return Math.Max(0, Math.Min(trackWidth, trackWidth * percentage));
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将标签类型转换为背景色的转换器
    /// </summary>
    public class TagTypeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VistaControls.TagType tagType)
            {
                return tagType switch
                {
                    VistaControls.TagType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F9EB")),
                    VistaControls.TagType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F4F5")),
                    VistaControls.TagType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDF6EC")),
                    VistaControls.TagType.Danger => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF0F0")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF5FF"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF5FF"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将标签类型转换为文字颜色的转换器
    /// </summary>
    public class TagTypeToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VistaControls.TagType tagType)
            {
                return tagType switch
                {
                    VistaControls.TagType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#67C23A")),
                    VistaControls.TagType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#909399")),
                    VistaControls.TagType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6A23C")),
                    VistaControls.TagType.Danger => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F56C6C")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将标签类型转换为边框颜色的转换器
    /// </summary>
    public class TagTypeToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VistaControls.TagType tagType)
            {
                return tagType switch
                {
                    VistaControls.TagType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3E19D")),
                    VistaControls.TagType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8C9CC")),
                    VistaControls.TagType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEBE77")),
                    VistaControls.TagType.Danger => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAB6B6")),
                    _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3D8FF"))
                };
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3D8FF"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 标签背景色转换器（优先使用 Color，否则使用 TagType）
    /// </summary>
    public class TagBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                // 如果 Color 不为 null，使用 Color
                if (values[0] is Brush color && color != null)
                {
                    return color;
                }
                // 否则使用 TagType 转换
                if (values[1] is VistaControls.TagType tagType)
                {
                    return tagType switch
                    {
                        VistaControls.TagType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F9EB")),
                        VistaControls.TagType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F4F5")),
                        VistaControls.TagType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDF6EC")),
                        VistaControls.TagType.Danger => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF0F0")),
                        _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF5FF"))
                    };
                }
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ECF5FF"));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 标签边框颜色转换器（Hit 为 true 时使用 TagType 颜色，否则透明）
    /// </summary>
    public class TagBorderColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                bool hit = values[0] is bool h && h;
                if (!hit)
                {
                    return Brushes.Transparent;
                }
                if (values[1] is VistaControls.TagType tagType)
                {
                    return tagType switch
                    {
                        VistaControls.TagType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3E19D")),
                        VistaControls.TagType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C8C9CC")),
                        VistaControls.TagType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEBE77")),
                        VistaControls.TagType.Danger => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAB6B6")),
                        _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B3D8FF"))
                    };
                }
            }
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 进度条圆角半径转换器（圆角半径为高度的一半）
    /// </summary>
    public class ProgressCornerRadiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double height && height > 0)
            {
                // 圆角半径为高度的一半，但最小为 2
                return Math.Max(2, height / 2.0);
            }
            return 3.0; // 默认值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 进度条宽度转换器
    /// </summary>
    public class ProgressWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is double percentage && values[1] is double totalWidth)
            {
                // 如果 totalWidth 为 0 或 NaN，使用默认值 350
                var width = double.IsNaN(totalWidth) || totalWidth <= 0 ? 350 : totalWidth;
                return Math.Max(0, width * percentage / 100.0);
            }
            // 如果绑定失败，至少显示一个最小宽度
            if (values.Length >= 1 && values[0] is double pct)
            {
                return Math.Max(0, 350 * pct / 100.0);
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 标签文字颜色转换器（优先使用 TextColor，否则使用 TagType）
    /// </summary>
    public class TagTextColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"));
            }

            try
            {
                // 如果 TextColor 不为 null，使用 TextColor
                if (values[0] is Brush textColor && textColor != null)
                {
                    return textColor;
                }
            }
            catch
            {
                // 忽略错误，继续使用 TagType
            }

            try
            {
                // 否则使用 TagType 转换
                if (values[1] is VistaControls.TagType tagType)
                {
                    return tagType switch
                    {
                        VistaControls.TagType.Success => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#67C23A")),
                        VistaControls.TagType.Info => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#909399")),
                        VistaControls.TagType.Warning => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6A23C")),
                        VistaControls.TagType.Danger => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F56C6C")),
                        _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"))
                    };
                }
            }
            catch
            {
                // 忽略错误
            }

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

