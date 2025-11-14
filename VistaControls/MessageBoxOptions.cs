using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace VistaControls
{
    /// <summary>
    /// MessageBox 配置选项
    /// </summary>
    public class MessageBoxOptions
    {
        /// <summary>
        /// MessageBox 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// MessageBox 消息正文内容
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 消息类型，用于显示图标
        /// </summary>
        public MessageBoxType? Type { get; set; }

        /// <summary>
        /// 自定义图标的类名，会覆盖 type
        /// </summary>
        public string? IconClass { get; set; }

        /// <summary>
        /// MessageBox 的自定义类名
        /// </summary>
        public string? CustomClass { get; set; }

        /// <summary>
        /// 若不使用 Promise，可以使用此参数指定 MessageBox 关闭后的回调
        /// </summary>
        public Action<MessageBoxResult, VistaMessageBox>? Callback { get; set; }

        /// <summary>
        /// MessageBox 是否显示右上角关闭按钮
        /// </summary>
        public bool ShowClose { get; set; } = true;

        /// <summary>
        /// MessageBox 关闭前的回调，会暂停实例的关闭
        /// </summary>
        public Func<MessageBoxResult, VistaMessageBox, Action, bool>? BeforeClose { get; set; }

        /// <summary>
        /// 是否将取消（点击取消按钮）与关闭（点击关闭按钮或遮罩层、按下 ESC 键）进行区分
        /// </summary>
        public bool DistinguishCancelAndClose { get; set; } = false;

        /// <summary>
        /// 是否在 MessageBox 出现时将 body 滚动锁定（在 WPF 中暂不支持）
        /// </summary>
        public bool LockScroll { get; set; } = true;

        /// <summary>
        /// 是否显示取消按钮
        /// </summary>
        public bool ShowCancelButton { get; set; } = false;

        /// <summary>
        /// 是否显示确定按钮
        /// </summary>
        public bool ShowConfirmButton { get; set; } = true;

        /// <summary>
        /// 取消按钮的文本内容
        /// </summary>
        public string CancelButtonText { get; set; } = "取消";

        /// <summary>
        /// 确定按钮的文本内容
        /// </summary>
        public string ConfirmButtonText { get; set; } = "确定";

        /// <summary>
        /// 取消按钮的自定义类名
        /// </summary>
        public string? CancelButtonClass { get; set; }

        /// <summary>
        /// 确定按钮的自定义类名
        /// </summary>
        public string? ConfirmButtonClass { get; set; }

        /// <summary>
        /// 是否可通过点击遮罩关闭 MessageBox
        /// </summary>
        public bool CloseOnClickModal { get; set; } = true;

        /// <summary>
        /// 是否可通过按下 ESC 键关闭 MessageBox
        /// </summary>
        public bool CloseOnPressEscape { get; set; } = true;

        /// <summary>
        /// 是否显示输入框
        /// </summary>
        public bool ShowInput { get; set; } = false;

        /// <summary>
        /// 输入框的占位符
        /// </summary>
        public string? InputPlaceholder { get; set; }

        /// <summary>
        /// 输入框的类型
        /// </summary>
        public string InputType { get; set; } = "text";

        /// <summary>
        /// 输入框的初始文本
        /// </summary>
        public string? InputValue { get; set; }

        /// <summary>
        /// 输入框的校验表达式
        /// </summary>
        public Regex? InputPattern { get; set; }

        /// <summary>
        /// 输入框的校验函数。可以返回布尔值或字符串，若返回一个字符串, 则返回结果会被赋值给 inputErrorMessage
        /// </summary>
        public Func<string, string?>? InputValidator { get; set; }

        /// <summary>
        /// 校验未通过时的提示文本
        /// </summary>
        public string InputErrorMessage { get; set; } = "输入的数据不合法!";

        /// <summary>
        /// 是否居中布局
        /// </summary>
        public bool Center { get; set; } = false;

        /// <summary>
        /// 是否使用圆角按钮
        /// </summary>
        public bool RoundButton { get; set; } = false;
    }

    /// <summary>
    /// MessageBox 类型
    /// </summary>
    public enum MessageBoxType
    {
        Success,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// MessageBox 结果
    /// </summary>
    public enum MessageBoxResult
    {
        Confirm,
        Cancel,
        Close
    }
}

