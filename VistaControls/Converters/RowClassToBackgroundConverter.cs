using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace VistaControls.Converters
{
    public class RowClassToBackgroundConverter : IMultiValueConverter
    {
        // values[0]: AlternationIndex (int)
        // values[1]: DataContext (row item)
        // values[2]: VistaTable (templated parent)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3) return Binding.DoNothing;

            int rowIndex = values[0] is int i ? i : -1;
            var rowItem = values[1];
            var table = values[2] as VistaTable;

            var name = table?.RowClassNameSelector?.Invoke(rowItem!, rowIndex);
            if (string.IsNullOrWhiteSpace(name)) return Brushes.Transparent;

            switch (name)
            {
                case "warning-row":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF8E6")!); // oldlace 类似
                case "success-row":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F9EB")!);
                case "info-row":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4F4F5")!);
                case "danger-row":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF0F0")!);
                default:
                    return Brushes.Transparent;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

