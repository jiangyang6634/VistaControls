using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VistaControls
{
    /// <summary>
    /// VistaInput - 输入框控件
    /// </summary>
    public class VistaInput : TextBox
    {
        private Button? _clearButton;
        private Button? _passwordToggleButton;
        private bool _isPasswordVisible = false;
        private string _actualPasswordText = string.Empty;
        private bool _suppressTextChanged;

        static VistaInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaInput),
                new FrameworkPropertyMetadata(typeof(VistaInput)));
        }

        public VistaInput()
        {
            TextChanged += VistaInput_TextChanged;
            Loaded += VistaInput_Loaded;
            PreviewTextInput += VistaInput_PreviewTextInput;
            PreviewKeyDown += VistaInput_PreviewKeyDown;
            DataObject.AddPastingHandler(this, OnPaste);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 获取清空按钮
            _clearButton = GetTemplateChild("PART_ClearButton") as Button;
            if (_clearButton != null)
            {
                _clearButton.Click += ClearButton_Click;
            }

            // 获取密码切换按钮
            _passwordToggleButton = GetTemplateChild("PART_PasswordToggleButton") as Button;
            if (_passwordToggleButton != null)
            {
                _passwordToggleButton.Click += PasswordToggleButton_Click;
            }

            UpdateClearButtonVisibility();
            UpdatePasswordToggleButtonVisibility();
        }

        #region 依赖属性

        /// <summary>
        /// 是否可清空
        /// </summary>
        public static readonly DependencyProperty ClearableProperty =
            DependencyProperty.Register(nameof(Clearable), typeof(bool), typeof(VistaInput),
                new PropertyMetadata(false, OnClearableChanged));

        /// <summary>
        /// 是否显示切换密码图标
        /// </summary>
        public static readonly DependencyProperty ShowPasswordProperty =
            DependencyProperty.Register(nameof(ShowPassword), typeof(bool), typeof(VistaInput),
                new PropertyMetadata(false, OnShowPasswordChanged));

        /// <summary>
        /// 密码是否可见（内部使用）
        /// </summary>
        private static readonly DependencyProperty IsPasswordVisibleProperty =
            DependencyProperty.Register(nameof(IsPasswordVisible), typeof(bool), typeof(VistaInput),
                new PropertyMetadata(false, OnIsPasswordVisibleChanged));

        /// <summary>
        /// 是否显示输入字数统计
        /// </summary>
        public static readonly DependencyProperty ShowWordLimitProperty =
            DependencyProperty.Register(nameof(ShowWordLimit), typeof(bool), typeof(VistaInput),
                new PropertyMetadata(false));

        /// <summary>
        /// 输入框关联的label文字
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(VistaInput),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// 输入框尺寸
        /// </summary>
        public static readonly DependencyProperty InputSizeProperty =
            DependencyProperty.Register(nameof(InputSize), typeof(InputSize), typeof(VistaInput),
                new PropertyMetadata(InputSize.Default));

        /// <summary>
        /// 输入框占位文本
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(VistaInput),
                new PropertyMetadata(string.Empty));

        #endregion

        #region 属性

        public bool Clearable
        {
            get => (bool)GetValue(ClearableProperty);
            set => SetValue(ClearableProperty, value);
        }

        public bool ShowPassword
        {
            get => (bool)GetValue(ShowPasswordProperty);
            set => SetValue(ShowPasswordProperty, value);
        }

        public bool ShowWordLimit
        {
            get => (bool)GetValue(ShowWordLimitProperty);
            set => SetValue(ShowWordLimitProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public InputSize InputSize
        {
            get => (InputSize)GetValue(InputSizeProperty);
            set => SetValue(InputSizeProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        private bool IsPasswordVisible
        {
            get => (bool)GetValue(IsPasswordVisibleProperty);
            set => SetValue(IsPasswordVisibleProperty, value);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 在点击清空按钮时触发
        /// </summary>
        public event EventHandler? Cleared;

        #endregion

        #region 事件处理

        private void VistaInput_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateClearButtonVisibility();
            UpdatePasswordToggleButtonVisibility();
        }

        private void VistaInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateClearButtonVisibility();

            // 在“显示密码”状态下，同步实际文本
            if (ShowPassword && _isPasswordVisible && !_suppressTextChanged)
            {
                _actualPasswordText = Text ?? string.Empty;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Text = string.Empty;
            Focus();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        private void PasswordToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // 切换密码显示/隐藏
            IsPasswordVisible = !IsPasswordVisible;
            UpdatePasswordDisplay();
        }

        private void UpdatePasswordDisplay()
        {
            if (!ShowPassword)
                return;

            _suppressTextChanged = true;
            try
            {
                if (_isPasswordVisible)
                {
                    // 显示真实密码
                    Text = _actualPasswordText;
                    CaretIndex = Text.Length;
                }
                else
                {
                    // 显示为点
                    Text = new string('●', _actualPasswordText.Length);
                    CaretIndex = Text.Length;
                }
            }
            finally
            {
                _suppressTextChanged = false;
            }
        }

        private static void OnIsPasswordVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInput input)
            {
                input._isPasswordVisible = (bool)e.NewValue;
                input.UpdatePasswordDisplay();
            }
        }

        private static void OnClearableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInput input)
            {
                input.UpdateClearButtonVisibility();
            }
        }

        private static void OnShowPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VistaInput input)
            {
                input.UpdatePasswordToggleButtonVisibility();
            }
        }

        private void UpdateClearButtonVisibility()
        {
            if (_clearButton != null)
            {
                _clearButton.Visibility = (Clearable && !string.IsNullOrEmpty(Text) && IsEnabled) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
        }

        private void UpdatePasswordToggleButtonVisibility()
        {
            if (_passwordToggleButton != null)
            {
                _passwordToggleButton.Visibility = ShowPassword 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;

                // 初始化显示为点
                if (ShowPassword && !_isPasswordVisible)
                {
                    UpdatePasswordDisplay();
                }
            }
        }

        private void VistaInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(ShowPassword && !_isPasswordVisible))
                return;

            // 密码隐藏状态：拦截输入，写入实际文本，刷新点显示
            e.Handled = true;
            ReplaceSelectionWith(e.Text);
            MaskToDots();
        }

        private void VistaInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(ShowPassword && !_isPasswordVisible))
                return;

            if (e.Key == Key.Back)
            {
                e.Handled = true;
                if (SelectionLength > 0)
                {
                    DeleteSelection();
                }
                else if (CaretIndex > 0 && _actualPasswordText.Length > 0)
                {
                    var index = CaretIndex - 1;
                    if (index >= 0 && index < _actualPasswordText.Length)
                    {
                        _actualPasswordText = _actualPasswordText.Remove(index, 1);
                        CaretIndex = Math.Max(0, CaretIndex - 1);
                    }
                }
                MaskToDots();
            }
            else if (e.Key == Key.Delete)
            {
                e.Handled = true;
                if (SelectionLength > 0)
                {
                    DeleteSelection();
                }
                else if (CaretIndex >= 0 && CaretIndex < _actualPasswordText.Length)
                {
                    _actualPasswordText = _actualPasswordText.Remove(CaretIndex, 1);
                }
                MaskToDots();
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!(ShowPassword && !_isPasswordVisible))
                return;

            if (e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                var pasteText = e.DataObject.GetData(DataFormats.UnicodeText) as string ?? string.Empty;
                e.CancelCommand();
                ReplaceSelectionWith(pasteText);
                MaskToDots();
            }
        }

        private void ReplaceSelectionWith(string text)
        {
            var start = CaretIndex;
            if (SelectionLength > 0)
            {
                var selStart = SelectionStart;
                _actualPasswordText = _actualPasswordText.Remove(selStart, SelectionLength);
                start = selStart;
            }
            _actualPasswordText = _actualPasswordText.Insert(start, text);
            CaretIndex = start + text.Length;
        }

        private void DeleteSelection()
        {
            if (SelectionLength <= 0)
                return;
            var selStart = SelectionStart;
            _actualPasswordText = _actualPasswordText.Remove(selStart, SelectionLength);
            CaretIndex = selStart;
        }

        private void MaskToDots()
        {
            _suppressTextChanged = true;
            try
            {
                var caret = CaretIndex;
                Text = new string('●', _actualPasswordText.Length);
                CaretIndex = Math.Min(caret, Text.Length);
            }
            finally
            {
                _suppressTextChanged = false;
            }
        }

        #endregion

        #region 方法

        /// <summary>
        /// 使 input 获取焦点
        /// </summary>
        public new void Focus()
        {
            base.Focus();
        }

        /// <summary>
        /// 使 input 失去焦点
        /// </summary>
        public void Blur()
        {
            // WPF 中没有直接的 Blur 方法，需要通过移动焦点来实现
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// 选中 input 中的文字
        /// </summary>
        public new void SelectAll()
        {
            base.SelectAll();
        }

        #endregion
    }

    /// <summary>
    /// Input 尺寸枚举
    /// </summary>
    public enum InputSize
    {
        Default,
        Medium,
        Small,
        Mini
    }
}

