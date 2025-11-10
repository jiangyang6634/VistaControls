using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VistaControls
{
    /// <summary>
    /// MessageManager - 消息管理器，用于显示和管理消息提示
    /// </summary>
    public static class MessageManager
    {
        private static Panel? _messageContainer;
        private static double _offset = 20;
        private static bool _sizeChangedHandlerAttached = false;

        /// <summary>
        /// 初始化消息容器（通常在 App.xaml.cs 或主窗口的 Loaded 事件中调用）
        /// </summary>
        /// <param name="container">用于显示消息的容器（通常是主窗口的某个 Panel）</param>
        /// <param name="offset">距离顶部的偏移量，默认 20</param>
        public static void Initialize(Panel container, double offset = 20)
        {
            _messageContainer = container;
            _offset = offset;
            
            // 如果是 Canvas，添加 SizeChanged 事件处理器（只添加一次）
            if (container is Canvas canvas && !_sizeChangedHandlerAttached)
            {
                canvas.SizeChanged += (sender, args) =>
                {
                    UpdateAllMessagesPosition(canvas);
                };
                _sizeChangedHandlerAttached = true;
            }
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="type">消息类型</param>
        /// <param name="duration">显示时长（毫秒），0 表示不自动关闭</param>
        /// <param name="showClose">是否显示关闭按钮</param>
        /// <param name="center">文字是否居中</param>
        /// <param name="onClose">关闭回调</param>
        /// <returns>消息实例</returns>
        public static VistaMessage Show(string message, MessageType type = MessageType.Info, 
            int duration = 3000, bool showClose = false, bool center = false, 
            Action<VistaMessage>? onClose = null)
        {
            if (_messageContainer == null)
            {
                throw new InvalidOperationException("MessageManager 未初始化。请在 App.xaml.cs 或主窗口的 Loaded 事件中调用 MessageManager.Initialize()。");
            }

            var messageControl = new VistaMessage
            {
                Message = message,
                MessageType = type,
                Duration = duration,
                ShowClose = showClose,
                Center = center,
                OnClose = onClose,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = true  // 确保消息框可以接收鼠标事件
            };

            _messageContainer.Children.Add(messageControl);
            
            // 如果是 Canvas，需要设置水平居中
            if (_messageContainer is Canvas canvas)
            {
                // 等待布局完成后再设置位置
                messageControl.Loaded += (s, e) =>
                {
                    UpdateMessagePosition(messageControl, canvas);
                };
            }
            else
            {
                // 其他容器类型使用 Margin
                messageControl.Margin = new Thickness(0, GetNextTopOffset(), 0, 0);
            }
            
            return messageControl;
        }

        /// <summary>
        /// 显示成功消息
        /// </summary>
        public static VistaMessage Success(string message, int duration = 3000, bool showClose = false)
        {
            return Show(message, MessageType.Success, duration, showClose);
        }

        /// <summary>
        /// 显示警告消息
        /// </summary>
        public static VistaMessage Warning(string message, int duration = 3000, bool showClose = false)
        {
            return Show(message, MessageType.Warning, duration, showClose);
        }

        /// <summary>
        /// 显示信息消息
        /// </summary>
        public static VistaMessage Info(string message, int duration = 3000, bool showClose = false)
        {
            return Show(message, MessageType.Info, duration, showClose);
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        public static VistaMessage Error(string message, int duration = 3000, bool showClose = false)
        {
            return Show(message, MessageType.Error, duration, showClose);
        }

        /// <summary>
        /// 计算下一个消息的顶部偏移量
        /// </summary>
        private static double GetNextTopOffset()
        {
            if (_messageContainer == null)
                return _offset;

            double currentOffset = _offset;
            
            if (_messageContainer is Canvas canvas)
            {
                // Canvas 容器：从 Canvas.GetTop 获取位置
                foreach (var child in _messageContainer.Children)
                {
                    if (child is VistaMessage msg && msg.Opacity > 0)
                    {
                        var top = Canvas.GetTop(msg);
                        var height = msg.ActualHeight > 0 ? msg.ActualHeight : 50; // 默认高度
                        currentOffset = Math.Max(currentOffset, top + height + 10); // 10px 间距
                    }
                }
            }
            else
            {
                // 其他容器：从 Margin 获取位置
                foreach (var child in _messageContainer.Children)
                {
                    if (child is VistaMessage msg && msg.Opacity > 0)
                    {
                        msg.UpdateLayout();
                        var top = msg.Margin.Top;
                        var height = msg.ActualHeight > 0 ? msg.ActualHeight : 50; // 默认高度
                        currentOffset = Math.Max(currentOffset, top + height + 10); // 10px 间距
                    }
                }
            }

            return currentOffset;
        }

        /// <summary>
        /// 更新单个消息的位置（水平居中）
        /// </summary>
        private static void UpdateMessagePosition(VistaMessage message, Canvas canvas)
        {
            if (message.Parent != canvas)
                return;

            var topOffset = GetNextTopOffset();
            Canvas.SetTop(message, topOffset);
            
            // 计算水平居中位置
            if (canvas.ActualWidth > 0)
            {
                message.UpdateLayout();
                var left = (canvas.ActualWidth - message.ActualWidth) / 2;
                Canvas.SetLeft(message, left);
            }
        }

        /// <summary>
        /// 更新所有消息的位置（水平居中）
        /// </summary>
        private static void UpdateAllMessagesPosition(Canvas canvas)
        {
            if (canvas.ActualWidth <= 0)
                return;

            double currentTop = _offset;
            foreach (var child in canvas.Children)
            {
                if (child is VistaMessage msg && msg.Opacity > 0)
                {
                    msg.UpdateLayout();
                    Canvas.SetTop(msg, currentTop);
                    var left = (canvas.ActualWidth - msg.ActualWidth) / 2;
                    Canvas.SetLeft(msg, left);
                    currentTop += msg.ActualHeight + 10; // 10px 间距
                }
            }
        }
    }
}

