using System;
using System.Collections.Generic;
using System.Windows;

namespace VistaControls
{
    /// <summary>
    /// Date Picker Shortcut - 日期选择器快捷选项
    /// </summary>
    public class DatePickerShortcut
    {
        /// <summary>
        /// 快捷选项显示的文本
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 点击快捷选项时的回调函数，返回要选择的日期
        /// </summary>
        public Func<DateTime>? OnClick { get; set; }
    }

    /// <summary>
    /// Date Picker Options - 日期选择器的配置选项
    /// </summary>
    public class DatePickerOptions : DependencyObject
    {
        /// <summary>
        /// 快捷选项列表
        /// </summary>
        public static readonly DependencyProperty ShortcutsProperty =
            DependencyProperty.Register(nameof(Shortcuts), typeof(List<DatePickerShortcut>), typeof(DatePickerOptions), 
                new PropertyMetadata(null));

        public List<DatePickerShortcut> Shortcuts
        {
            get
            {
                var value = (List<DatePickerShortcut>?)GetValue(ShortcutsProperty);
                if (value == null)
                {
                    value = new List<DatePickerShortcut>();
                    SetValue(ShortcutsProperty, value);
                }
                return value;
            }
            set => SetValue(ShortcutsProperty, value);
        }

        /// <summary>
        /// 禁用日期的函数，返回 true 表示该日期被禁用
        /// </summary>
        public Func<DateTime, bool>? DisabledDate { get; set; }

        /// <summary>
        /// 日期格式化字符串，默认：'yyyy-MM-dd'
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(DatePickerOptions), 
                new PropertyMetadata("yyyy-MM-dd"));

        public string Format
        {
            get => (string)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }
    }
}

