using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VistaControls
{
    /// <summary>
    /// VistaMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class VistaMessageBox : Window
    {
        private MessageBoxOptions _options;
        private MessageBoxResult _result = MessageBoxResult.Close;
        private bool _isClosing = false;

        public VistaMessageBox(MessageBoxOptions options)
        {
            InitializeComponent();
            _options = options;
            DataContext = options;
            Loaded += VistaMessageBox_Loaded;
            KeyDown += VistaMessageBox_KeyDown;
            PreviewMouseDown += VistaMessageBox_PreviewMouseDown;
        }

        private void VistaMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置标题
            if (string.IsNullOrEmpty(_options.Title))
            {
                titleText.Visibility = Visibility.Collapsed;
            }

            // 设置关闭按钮
            if (_options.ShowClose)
            {
                closeButton.Visibility = Visibility.Visible;
            }

            // 设置取消按钮
            if (_options.ShowCancelButton)
            {
                cancelButton.Visibility = Visibility.Visible;
            }

            // 设置确定按钮
            if (!_options.ShowConfirmButton)
            {
                confirmButton.Visibility = Visibility.Collapsed;
            }

            // 设置图标
            if (_options.Type.HasValue)
            {
                iconContainer.Visibility = Visibility.Visible;
                SetIcon(_options.Type.Value);
            }
            else if (!string.IsNullOrEmpty(_options.IconClass))
            {
                iconContainer.Visibility = Visibility.Visible;
                // 自定义图标类名（在 WPF 中暂不支持，使用默认图标）
                SetIcon(MessageBoxType.Info);
            }

            // 设置输入框（用于 prompt）
            if (_options.ShowInput)
            {
                inputContainer.Visibility = Visibility.Visible;
                if (!string.IsNullOrEmpty(_options.InputPlaceholder))
                {
                    // 使用 ToolTip 作为占位符提示
                    inputBox.ToolTip = _options.InputPlaceholder;
                }
                if (!string.IsNullOrEmpty(_options.InputValue))
                {
                    inputBox.Text = _options.InputValue;
                }
                else
                {
                    inputBox.Text = string.Empty;
                }
                // 初始化错误消息文本
                inputErrorText.Text = _options.InputErrorMessage;
                // 延迟设置焦点，确保窗口已完全加载
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    inputBox.Focus();
                    inputBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }

            // 设置圆角按钮（默认已启用，如果选项指定不启用则禁用）
            // 注意：默认情况下按钮已经是圆角的，这里只处理特殊情况
            if (!_options.RoundButton)
            {
                cancelButton.Round = false;
                confirmButton.Round = false;
            }
            
            // 设置按钮大小
            cancelButton.ButtonSize = ButtonSize.Small;
            confirmButton.ButtonSize = ButtonSize.Small;

            // 居中布局
            if (_options.Center)
            {
                buttonContainer.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void SetIcon(MessageBoxType type)
        {
            iconText.Text = type switch
            {
                MessageBoxType.Success => "✓",
                MessageBoxType.Info => "ℹ",
                MessageBoxType.Warning => "⚠",
                MessageBoxType.Error => "✕",
                _ => "ℹ"
            };

            iconContainer.Background = type switch
            {
                MessageBoxType.Success => new SolidColorBrush(Color.FromRgb(103, 194, 58)),
                MessageBoxType.Info => new SolidColorBrush(Color.FromRgb(64, 158, 255)),
                MessageBoxType.Warning => new SolidColorBrush(Color.FromRgb(230, 162, 60)),
                MessageBoxType.Error => new SolidColorBrush(Color.FromRgb(245, 108, 108)),
                _ => new SolidColorBrush(Color.FromRgb(64, 158, 255))
            };

            iconText.Foreground = Brushes.White;
        }

        private void VistaMessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _options.CloseOnPressEscape)
            {
                CloseWithResult(MessageBoxResult.Close);
            }
            else if (e.Key == Key.Enter && _options.ShowInput)
            {
                // 如果显示输入框，按 Enter 键触发确定
                ConfirmButton_Click(null!, null!);
            }
        }

        private void VistaMessageBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 点击遮罩层关闭
            if (e.OriginalSource == this && _options.CloseOnClickModal)
            {
                CloseWithResult(MessageBoxResult.Close);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithResult(MessageBoxResult.Close);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseWithResult(MessageBoxResult.Cancel);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // 如果显示输入框，需要验证输入
            if (_options.ShowInput)
            {
                var inputValue = inputBox.Text?.Trim() ?? string.Empty;

                // 验证正则表达式
                if (_options.InputPattern != null)
                {
                    if (!_options.InputPattern.IsMatch(inputValue))
                    {
                        inputErrorText.Text = _options.InputErrorMessage;
                        inputErrorText.Visibility = Visibility.Visible;
                        inputBox.Focus();
                        inputBox.SelectAll();
                        return;
                    }
                }

                // 验证函数
                if (_options.InputValidator != null)
                {
                    var validationResult = _options.InputValidator(inputValue);
                    if (validationResult != null)
                    {
                        // 如果返回字符串，表示验证失败，使用该字符串作为错误消息
                        inputErrorText.Text = validationResult;
                        inputErrorText.Visibility = Visibility.Visible;
                        inputBox.Focus();
                        inputBox.SelectAll();
                        return;
                    }
                }

                // 验证通过，隐藏错误消息
                inputErrorText.Visibility = Visibility.Collapsed;
                // 保存输入值
                _options.InputValue = inputValue;
            }

            CloseWithResult(MessageBoxResult.Confirm);
        }

        private void CloseWithResult(MessageBoxResult result)
        {
            if (_isClosing) return;

            _result = result;

            // 调用 BeforeClose 回调
            if (_options.BeforeClose != null)
            {
                bool shouldClose = _options.BeforeClose(result, this, () =>
                {
                    // done 回调，实际关闭窗口
                    _isClosing = true;
                    DialogResult = result == MessageBoxResult.Confirm;
                    Close();
                });

                if (!shouldClose)
                {
                    return; // BeforeClose 返回 false，不关闭
                }
            }

            _isClosing = true;
            DialogResult = result == MessageBoxResult.Confirm;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // 调用 Callback
            if (_options.Callback != null)
            {
                _options.Callback(_result, this);
            }
        }

        /// <summary>
        /// 获取输入值（用于 prompt）
        /// </summary>
        public string? GetInputValue()
        {
            return _options.InputValue;
        }
    }
}

