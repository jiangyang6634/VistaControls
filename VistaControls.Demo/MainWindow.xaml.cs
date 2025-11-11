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
        private VistaControls.CascaderOption BuildNode(string label, object value, bool disabled = false, params VistaControls.CascaderOption[] children)
        {
            return new VistaControls.CascaderOption
            {
                Label = label,
                Value = value,
                Disabled = disabled,
                Children = new System.Collections.ObjectModel.ObservableCollection<VistaControls.CascaderOption>(children ?? System.Array.Empty<VistaControls.CascaderOption>())
            };
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化 MessageManager
            VistaControls.MessageManager.Initialize(messageContainer, 20);

            // 初始化 Switch 事件
            if (switchValue != null)
            {
                switchValue.Change += SwitchValue_Change;
                switchValue.Value = 0; // 初始值
            }

            // 初始化 Slider 格式化 tooltip
            if (slider4 != null)
            {
                slider4.FormatTooltip = val => (val / 100).ToString("F2");
            }

            // 初始化 Cascader 数据
            var options = new System.Collections.ObjectModel.ObservableCollection<VistaControls.CascaderOption>
            {
                BuildNode("指南", "zhinan", false,
                    BuildNode("设计原则", "shejiyuanze", false,
                        BuildNode("一致", "yizhi"),
                        BuildNode("反馈", "fankui"),
                        BuildNode("效率", "xiaolv"),
                        BuildNode("可控", "kekong")),
                    BuildNode("导航", "daohang", false,
                        BuildNode("侧向导航", "cexiangdaohang"),
                        BuildNode("顶部导航", "dingbudaohang"))
                ),
                BuildNode("组件", "zujian", false,
                    BuildNode("Basic", "basic", false,
                        BuildNode("Layout 布局", "layout"),
                        BuildNode("Color 色彩", "color"),
                        BuildNode("Typography 字体", "typography"),
                        BuildNode("Icon 图标", "icon"),
                        BuildNode("Button 按钮", "button")),
                    BuildNode("Form", "form", false,
                        BuildNode("Radio 单选框", "radio"),
                        BuildNode("Checkbox 多选框", "checkbox"),
                        BuildNode("Input 输入框", "input"),
                        BuildNode("InputNumber 计数器", "input-number"),
                        BuildNode("Select 选择器", "select"),
                        BuildNode("Cascader 级联选择器", "cascader"))),
                BuildNode("资源", "ziyuan", false,
                    BuildNode("Axure Components", "axure"),
                    BuildNode("Sketch Templates", "sketch"),
                    BuildNode("组件交互文档", "jiaohu"))
            };

            if (cascaderClick != null)     cascaderClick.Options = options;
            if (cascaderHover != null)     cascaderHover.Options = options;
            if (cascaderClearable != null) cascaderClearable.Options = options;
            if (cascaderLast != null)      cascaderLast.Options = options;
            if (cascaderMulti != null)     cascaderMulti.Options = options;
            if (cascaderMultiCollapse != null) cascaderMultiCollapse.Options = options;
            
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

            // 初始化 TimePicker 事件
            if (timeSelectStart != null)
            {
                timeSelectStart.Change += (s, e) =>
                {
                    if (timeSelectEnd != null && e != null)
                    {
                        // 更新结束时间的最小时间限制
                        timeSelectEnd.PickerOptions.MinTime = e;
                    }
                };
            }

            if (timePicker1 != null)
            {
                timePicker1.Change += (s, e) =>
                {
                    if (e is DateTime time)
                    {
                        VistaControls.MessageManager.Info($"选择了时间: {time:HH:mm:ss}");
                    }
                };
            }

            if (timePickerRange1 != null)
            {
                timePickerRange1.Change += (s, e) =>
                {
                    if (e is DateTime[] range && range.Length == 2)
                    {
                        VistaControls.MessageManager.Info($"选择了时间范围: {range[0]:HH:mm:ss} 至 {range[1]:HH:mm:ss}");
                    }
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

        private void SwitchValue_Change(object? sender, object newValue)
        {
            if (switchValueText != null)
            {
                switchValueText.Text = $"Switch value: {newValue}";
            }
        }

        private void Slider4_FormatTooltip(double value)
        {
            // 格式化 tooltip：显示为百分比
            if (slider4 != null)
            {
                slider4.FormatTooltip = val => (val / 100).ToString("F2");
            }
        }
    }
}