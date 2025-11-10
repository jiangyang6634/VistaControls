using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VistaControls.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化 MessageManager
            VistaControls.MessageManager.Initialize(messageContainer, 20);
            
            // 初始化 checkboxGroup1 的选中值
            if (checkboxGroup1 != null)
            {
                checkboxGroup1.SelectedValues = new System.Collections.ObjectModel.ObservableCollection<object> 
                { 
                    "选中且禁用", 
                    "复选框 A" 
                };
            }
            
            // 初始化 checkboxGroup2 的选中值
            if (checkboxGroup2 != null)
            {
                checkboxGroup2.SelectedValues = new System.Collections.ObjectModel.ObservableCollection<object> 
                { 
                    "上海", 
                    "北京" 
                };
            }
            
            // 初始化 checkboxGroup3 的选中值
            if (checkboxGroup3 != null)
            {
                checkboxGroup3.SelectedValues = new System.Collections.ObjectModel.ObservableCollection<object> 
                { 
                    "上海", 
                    "北京" 
                };
            }
        }

        private void ShowBasicMessage_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Info("这是一条消息提示");
        }

        private void ShowSuccessMessage_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Success("恭喜你，这是一条成功消息");
        }

        private void ShowWarningMessage_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Warning("警告哦，这是一条警告消息");
        }

        private void ShowInfoMessage_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Info("这是一条消息提示");
        }

        private void ShowErrorMessage_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Error("错了哦，这是一条错误消息");
        }

        private void ShowClosableInfo_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Show("这是一条消息提示", VistaControls.MessageType.Info, 0, true);
        }

        private void ShowClosableSuccess_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Show("恭喜你，这是一条成功消息", VistaControls.MessageType.Success, 0, true);
        }

        private void ShowClosableWarning_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Show("警告哦，这是一条警告消息", VistaControls.MessageType.Warning, 0, true);
        }

        private void ShowClosableError_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Show("错了哦，这是一条错误消息", VistaControls.MessageType.Error, 0, true);
        }

        private void ShowCenterMessage_Click(object sender, RoutedEventArgs e)
        {
            VistaControls.MessageManager.Show("居中的文字", VistaControls.MessageType.Info, 3000, false, true);
        }

        private void RadioGroup1_SelectionChanged(object sender, object? e)
        {
            if (e != null)
            {
                VistaControls.MessageManager.Info($"选中了值: {e}");
            }
        }

        private void CheckboxGroup1_SelectionChanged(object sender, System.Collections.IList? e)
        {
            if (e != null)
            {
                var values = string.Join(", ", e.Cast<object>());
                VistaControls.MessageManager.Info($"选中的值: {values}");
            }
        }

        private string[] _cities = { "上海", "北京", "广州", "深圳" };

        private void CheckAll_CheckedChanged(object sender, object? e)
        {
            if (sender is VistaControls.VistaCheckbox checkAll && checkboxGroup2 != null)
            {
                if (checkAll.IsChecked == true)
                {
                    // 全选
                    checkboxGroup2.SelectedValues = new System.Collections.ObjectModel.ObservableCollection<object>(_cities);
                }
                else
                {
                    // 取消全选
                    checkboxGroup2.SelectedValues = new System.Collections.ObjectModel.ObservableCollection<object>();
                }
                checkAll.IsIndeterminate = false;
            }
        }

        private void CheckboxGroup2_SelectionChanged(object sender, System.Collections.IList? e)
        {
            if (e != null && checkAll != null)
            {
                int checkedCount = e.Count;
                int totalCount = _cities.Length;
                
                if (checkedCount == 0)
                {
                    checkAll.IsChecked = false;
                    checkAll.IsIndeterminate = false;
                }
                else if (checkedCount == totalCount)
                {
                    checkAll.IsChecked = true;
                    checkAll.IsIndeterminate = false;
                }
                else
                {
                    checkAll.IsChecked = null; // Indeterminate
                    checkAll.IsIndeterminate = true;
                }
            }
        }
    }
}