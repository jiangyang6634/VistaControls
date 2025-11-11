using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VistaControls
{
    /// <summary>
    /// VistaSwitch - 开关控件
    /// </summary>
    public class VistaSwitch : ToggleButton
    {
        static VistaSwitch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaSwitch),
                new FrameworkPropertyMetadata(typeof(VistaSwitch)));
        }

        public VistaSwitch()
        {
            Checked += VistaSwitch_Checked;
            Unchecked += VistaSwitch_Unchecked;
        }

        #region 依赖属性

        /// <summary>
        /// 绑定值（boolean / string / number）
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(VistaSwitch),
                new PropertyMetadata(false, OnValueChanged));

        /// <summary>
        /// Switch 的宽度（像素）
        /// </summary>
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(VistaSwitch),
                new PropertyMetadata(40.0));

        /// <summary>
        /// Switch 打开时所显示图标的类名，设置此项会忽略 active-text
        /// </summary>
        public static readonly DependencyProperty ActiveIconClassProperty =
            DependencyProperty.Register(nameof(ActiveIconClass), typeof(string), typeof(VistaSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Switch 关闭时所显示图标的类名，设置此项会忽略 inactive-text
        /// </summary>
        public static readonly DependencyProperty InactiveIconClassProperty =
            DependencyProperty.Register(nameof(InactiveIconClass), typeof(string), typeof(VistaSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Switch 打开时的文字描述
        /// </summary>
        public static readonly DependencyProperty ActiveTextProperty =
            DependencyProperty.Register(nameof(ActiveText), typeof(string), typeof(VistaSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Switch 关闭时的文字描述
        /// </summary>
        public static readonly DependencyProperty InactiveTextProperty =
            DependencyProperty.Register(nameof(InactiveText), typeof(string), typeof(VistaSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Switch 打开时的值
        /// </summary>
        public static readonly DependencyProperty ActiveValueProperty =
            DependencyProperty.Register(nameof(ActiveValue), typeof(object), typeof(VistaSwitch),
                new PropertyMetadata(true));

        /// <summary>
        /// Switch 关闭时的值
        /// </summary>
        public static readonly DependencyProperty InactiveValueProperty =
            DependencyProperty.Register(nameof(InactiveValue), typeof(object), typeof(VistaSwitch),
                new PropertyMetadata(false));

        /// <summary>
        /// Switch 打开时的背景色
        /// </summary>
        public static readonly DependencyProperty ActiveColorProperty =
            DependencyProperty.Register(nameof(ActiveColor), typeof(Brush), typeof(VistaSwitch),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#409EFF"))));

        /// <summary>
        /// Switch 关闭时的背景色
        /// </summary>
        public static readonly DependencyProperty InactiveColorProperty =
            DependencyProperty.Register(nameof(InactiveColor), typeof(Brush), typeof(VistaSwitch),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0CCDA"))));

        /// <summary>
        /// Switch 对应的 name 属性
        /// </summary>
        public static readonly DependencyProperty SwitchNameProperty =
            DependencyProperty.Register(nameof(SwitchName), typeof(string), typeof(VistaSwitch),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 改变 switch 状态时是否触发表单的校验
        /// </summary>
        public static readonly DependencyProperty ValidateEventProperty =
            DependencyProperty.Register(nameof(ValidateEvent), typeof(bool), typeof(VistaSwitch),
                new PropertyMetadata(true));

        #endregion

        #region 属性

        /// <summary>
        /// 绑定值（boolean / string / number）
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Switch 的宽度（像素）
        /// </summary>
        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        /// <summary>
        /// Switch 打开时所显示图标的类名
        /// </summary>
        public string ActiveIconClass
        {
            get => (string)GetValue(ActiveIconClassProperty);
            set => SetValue(ActiveIconClassProperty, value);
        }

        /// <summary>
        /// Switch 关闭时所显示图标的类名
        /// </summary>
        public string InactiveIconClass
        {
            get => (string)GetValue(InactiveIconClassProperty);
            set => SetValue(InactiveIconClassProperty, value);
        }

        /// <summary>
        /// Switch 打开时的文字描述
        /// </summary>
        public string ActiveText
        {
            get => (string)GetValue(ActiveTextProperty);
            set => SetValue(ActiveTextProperty, value);
        }

        /// <summary>
        /// Switch 关闭时的文字描述
        /// </summary>
        public string InactiveText
        {
            get => (string)GetValue(InactiveTextProperty);
            set => SetValue(InactiveTextProperty, value);
        }

        /// <summary>
        /// Switch 打开时的值
        /// </summary>
        public object ActiveValue
        {
            get => GetValue(ActiveValueProperty);
            set => SetValue(ActiveValueProperty, value);
        }

        /// <summary>
        /// Switch 关闭时的值
        /// </summary>
        public object InactiveValue
        {
            get => GetValue(InactiveValueProperty);
            set => SetValue(InactiveValueProperty, value);
        }

        /// <summary>
        /// Switch 打开时的背景色
        /// </summary>
        public Brush ActiveColor
        {
            get => (Brush)GetValue(ActiveColorProperty);
            set => SetValue(ActiveColorProperty, value);
        }

        /// <summary>
        /// Switch 关闭时的背景色
        /// </summary>
        public Brush InactiveColor
        {
            get => (Brush)GetValue(InactiveColorProperty);
            set => SetValue(InactiveColorProperty, value);
        }

        /// <summary>
        /// Switch 对应的 name 属性
        /// </summary>
        public string SwitchName
        {
            get => (string)GetValue(SwitchNameProperty);
            set => SetValue(SwitchNameProperty, value);
        }

        /// <summary>
        /// 改变 switch 状态时是否触发表单的校验
        /// </summary>
        public bool ValidateEvent
        {
            get => (bool)GetValue(ValidateEventProperty);
            set => SetValue(ValidateEventProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// Switch 状态发生变化时的回调函数
        /// </summary>
        public event EventHandler<object>? Change;

        #endregion

        #region 私有方法

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaSwitch switchControl)
            {
                switchControl.UpdateIsCheckedFromValue();
            }
        }

        private void UpdateIsCheckedFromValue()
        {
            // 比较 Value 与 ActiveValue
            bool isActive = AreValuesEqual(Value, ActiveValue);
            if (IsChecked != isActive)
            {
                IsChecked = isActive;
            }
        }

        private void VistaSwitch_Checked(object sender, RoutedEventArgs e)
        {
            UpdateValueFromIsChecked();
            OnChange(ActiveValue);
        }

        private void VistaSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateValueFromIsChecked();
            OnChange(InactiveValue);
        }

        private void UpdateValueFromIsChecked()
        {
            object newValue = IsChecked == true ? ActiveValue : InactiveValue;
            if (!AreValuesEqual(Value, newValue))
            {
                Value = newValue;
            }
        }

        private bool AreValuesEqual(object? value1, object? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;

            // 尝试数值比较
            if (TryConvertToDouble(value1, out double d1) && TryConvertToDouble(value2, out double d2))
            {
                return Math.Abs(d1 - d2) < 0.0001;
            }

            // 字符串比较
            return value1.ToString() == value2.ToString();
        }

        private bool TryConvertToDouble(object value, out double result)
        {
            result = 0;
            if (value is double d)
            {
                result = d;
                return true;
            }
            if (value is int i)
            {
                result = i;
                return true;
            }
            if (value is float f)
            {
                result = f;
                return true;
            }
            if (value is string s && double.TryParse(s, out result))
            {
                return true;
            }
            return false;
        }

        private void OnChange(object newValue)
        {
            Change?.Invoke(this, newValue);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 使 Switch 获取焦点
        /// </summary>
        public new void Focus()
        {
            base.Focus();
        }

        #endregion
    }
}

