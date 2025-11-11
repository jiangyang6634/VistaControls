using System;
using System.Collections.Generic;

namespace VistaControls
{
    /// <summary>
    /// Time Select Options - 固定时间点选择器的配置选项
    /// </summary>
    public class TimeSelectOptions
    {
        /// <summary>
        /// 开始时间，格式：HH:mm
        /// </summary>
        public string Start { get; set; } = "09:00";

        /// <summary>
        /// 结束时间，格式：HH:mm
        /// </summary>
        public string End { get; set; } = "18:00";

        /// <summary>
        /// 间隔时间，格式：HH:mm
        /// </summary>
        public string Step { get; set; } = "00:30";

        /// <summary>
        /// 最小时间，小于该时间的时间段将被禁用，格式：HH:mm
        /// </summary>
        public string? MinTime { get; set; }

        /// <summary>
        /// 最大时间，大于该时间的时间段将被禁用，格式：HH:mm
        /// </summary>
        public string? MaxTime { get; set; }
    }

    /// <summary>
    /// Time Picker Options - 任意时间点选择器的配置选项
    /// </summary>
    public class TimePickerOptions
    {
        /// <summary>
        /// 可选时间段，例如'18:30:00 - 20:30:00'或者传入数组['09:30:00 - 12:00:00', '14:30:00 - 18:30:00']
        /// </summary>
        public object? SelectableRange { get; set; }

        /// <summary>
        /// 时间格式化，默认：'HH:mm:ss'
        /// </summary>
        public string Format { get; set; } = "HH:mm:ss";
    }
}

