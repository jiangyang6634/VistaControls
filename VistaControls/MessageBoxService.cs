using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace VistaControls
{
    /// <summary>
    /// MessageBoxService - 消息框服务，用于显示消息提示、确认消息和提交内容
    /// </summary>
    public static class MessageBoxService
    {
        /// <summary>
        /// 显示消息提示（Alert）
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="options">配置选项</param>
        /// <returns>Task，完成后返回 MessageBoxResult</returns>
        public static Task<MessageBoxResult> Alert(string message, string? title = null, MessageBoxOptions? options = null)
        {
            options ??= new MessageBoxOptions();
            options.Message = message;
            options.Title = title ?? "提示";
            options.ShowCancelButton = false;
            options.CloseOnClickModal = false;
            options.CloseOnPressEscape = false;

            return ShowMessageBox(options);
        }

        /// <summary>
        /// 显示消息提示（Alert），使用回调
        /// </summary>
        public static void Alert(string message, string? title, Action<MessageBoxResult>? callback, MessageBoxOptions? options = null)
        {
            options ??= new MessageBoxOptions();
            options.Message = message;
            options.Title = title ?? "提示";
            options.ShowCancelButton = false;
            options.CloseOnClickModal = false;
            options.CloseOnPressEscape = false;

            if (callback != null)
            {
                options.Callback = (result, instance) => callback(result);
            }

            _ = ShowMessageBox(options);
        }

        /// <summary>
        /// 显示确认消息（Confirm）
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="options">配置选项</param>
        /// <returns>Task，完成后返回 MessageBoxResult</returns>
        public static Task<MessageBoxResult> Confirm(string message, string? title = null, MessageBoxOptions? options = null)
        {
            options ??= new MessageBoxOptions();
            options.Message = message;
            options.Title = title ?? "提示";
            options.ShowCancelButton = true;
            options.Type = MessageBoxType.Warning;

            return ShowMessageBox(options);
        }

        /// <summary>
        /// 显示确认消息（Confirm），使用回调
        /// </summary>
        public static void Confirm(string message, string? title, Action<MessageBoxResult>? callback, MessageBoxOptions? options = null)
        {
            options ??= new MessageBoxOptions();
            options.Message = message;
            options.Title = title ?? "提示";
            options.ShowCancelButton = true;
            options.Type = MessageBoxType.Warning;

            if (callback != null)
            {
                options.Callback = (result, instance) => callback(result);
            }

            _ = ShowMessageBox(options);
        }

        /// <summary>
        /// 显示输入提示（Prompt）
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="options">配置选项</param>
        /// <returns>Task，完成后返回输入值，如果取消则返回 null</returns>
        public static Task<string?> Prompt(string message, string? title = null, MessageBoxOptions? options = null)
        {
            options ??= new MessageBoxOptions();
            options.Message = message;
            options.Title = title ?? "提示";
            options.ShowCancelButton = true;
            options.ShowInput = true;

            return ShowPrompt(options);
        }

        /// <summary>
        /// 显示输入提示（Prompt），使用回调
        /// </summary>
        public static void Prompt(string message, string? title, Action<string?>? callback, MessageBoxOptions? options = null)
        {
            options ??= new MessageBoxOptions();
            options.Message = message;
            options.Title = title ?? "提示";
            options.ShowCancelButton = true;
            options.ShowInput = true;

            if (callback != null)
            {
                options.Callback = (result, instance) =>
                {
                    if (result == MessageBoxResult.Confirm)
                    {
                        callback(instance.GetInputValue());
                    }
                    else
                    {
                        callback(null);
                    }
                };
            }

            _ = ShowPrompt(options);
        }

        /// <summary>
        /// 显示消息框
        /// </summary>
        private static Task<MessageBoxResult> ShowMessageBox(MessageBoxOptions options)
        {
            var tcs = new TaskCompletionSource<MessageBoxResult>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var messageBox = new VistaMessageBox(options);
                
                // 在显示对话框之前设置 Owner
                if (Application.Current.MainWindow != null && Application.Current.MainWindow != messageBox)
                {
                    messageBox.Owner = Application.Current.MainWindow;
                }
                
                messageBox.Closed += (sender, e) =>
                {
                    var mb = sender as VistaMessageBox;
                    if (mb != null)
                    {
                        var result = mb.DialogResult == true ? MessageBoxResult.Confirm :
                                     mb.DialogResult == false ? MessageBoxResult.Cancel : MessageBoxResult.Close;
                        tcs.SetResult(result);
                    }
                };

                messageBox.ShowDialog();
            });

            return tcs.Task;
        }

        /// <summary>
        /// 显示输入提示框
        /// </summary>
        private static Task<string?> ShowPrompt(MessageBoxOptions options)
        {
            var tcs = new TaskCompletionSource<string?>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                var messageBox = new VistaMessageBox(options);
                
                // 在显示对话框之前设置 Owner
                if (Application.Current.MainWindow != null && Application.Current.MainWindow != messageBox)
                {
                    messageBox.Owner = Application.Current.MainWindow;
                }
                
                messageBox.Closed += (sender, e) =>
                {
                    var mb = sender as VistaMessageBox;
                    if (mb != null)
                    {
                        if (mb.DialogResult == true)
                        {
                            tcs.SetResult(mb.GetInputValue());
                        }
                        else
                        {
                            tcs.SetResult(null);
                        }
                    }
                };

                messageBox.ShowDialog();
            });

            return tcs.Task;
        }
    }
}

