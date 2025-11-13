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

            // 初始化 Tag
            InitializeTags();

            // 初始化 Progress
            InitializeProgress();

            // 初始化日期选择器
            InitializeDatePickers();

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

            // 初始化表格
            InitializeTable();
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

        private void InitializeDatePickers()
        {
            // 带快捷选项的日期选择器
            if (datePicker2 != null)
            {
                var options = new VistaControls.DatePickerOptions
                {
                    Shortcuts = new List<VistaControls.DatePickerShortcut>
                    {
                        new VistaControls.DatePickerShortcut
                        {
                            Text = "今天",
                            OnClick = () => DateTime.Now
                        },
                        new VistaControls.DatePickerShortcut
                        {
                            Text = "昨天",
                            OnClick = () => DateTime.Now.AddDays(-1)
                        },
                        new VistaControls.DatePickerShortcut
                        {
                            Text = "一周前",
                            OnClick = () => DateTime.Now.AddDays(-7)
                        }
                    }
                };
                datePicker2.PickerOptions = options;
            }

            // 禁用日期（禁用未来日期）
            if (datePickerDisabled != null)
            {
                var options = new VistaControls.DatePickerOptions
                {
                    DisabledDate = (date) => date > DateTime.Now
                };
                datePickerDisabled.PickerOptions = options;
            }

            // 日期选择器事件
            if (datePicker1 != null)
            {
                datePicker1.Change += (s, e) =>
                {
                    if (e is DateTime date)
                    {
                        VistaControls.MessageManager.Info($"选择了日期: {date:yyyy-MM-dd}");
                    }
                };
            }
        }

        // Tag 相关事件处理
        private List<string> _dynamicTags = new List<string> { "标签一", "标签二", "标签三" };

        private void InitializeTags()
        {
            // 初始化动态标签
            UpdateDynamicTags();
        }

        private void UpdateDynamicTags()
        {
            if (dynamicTagsContainer != null)
            {
                dynamicTagsContainer.Items.Clear();
                foreach (var tag in _dynamicTags)
                {
                    var tagControl = new VistaControls.VistaTag
                    {
                        Content = tag,
                        Closable = true,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    tagControl.Close += DynamicTag_Close;
                    dynamicTagsContainer.Items.Add(tagControl);
                }
            }
        }

        private void Tag_Close(object sender, RoutedEventArgs e)
        {
            if (sender is VistaControls.VistaTag tag)
            {
                VistaControls.MessageManager.Show($"标签 '{tag.Content}' 已关闭", VistaControls.MessageType.Success);
            }
        }

        private void DynamicTag_Close(object sender, RoutedEventArgs e)
        {
            if (sender is VistaControls.VistaTag tag && tag.Content is string tagText)
            {
                _dynamicTags.Remove(tagText);
                UpdateDynamicTags();
                VistaControls.MessageManager.Show($"标签 '{tagText}' 已移除", VistaControls.MessageType.Info);
            }
        }

        private void Tag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is VistaControls.VistaTag tag)
            {
                VistaControls.MessageManager.Show($"点击了标签: {tag.Content}", VistaControls.MessageType.Info);
            }
        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (tagInput != null && addTagButton != null)
            {
                tagInput.Visibility = Visibility.Visible;
                addTagButton.Visibility = Visibility.Collapsed;
                tagInput.Focus();
            }
        }

        private void TagInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && tagInput != null)
            {
                HandleTagInput();
            }
        }

        private void TagInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tagInput != null)
            {
                HandleTagInput();
            }
        }

        private void HandleTagInput()
        {
            if (tagInput != null && addTagButton != null)
            {
                var inputValue = tagInput.Text?.Trim();
                if (!string.IsNullOrEmpty(inputValue) && !_dynamicTags.Contains(inputValue))
                {
                    _dynamicTags.Add(inputValue);
                    UpdateDynamicTags();
                }
                tagInput.Text = string.Empty;
                tagInput.Visibility = Visibility.Collapsed;
                addTagButton.Visibility = Visibility.Visible;
            }
        }

        // Progress 相关事件处理
        private double _progressPercentage = 20;
        private double _dashboardPercentage = 10;

        private void InitializeProgress()
        {
            // 设置格式化函数
            if (progressFormat != null)
            {
                progressFormat.Format = (percentage) => percentage == 100 ? "满" : $"{percentage:F0}%";
            }

            // 设置自定义颜色
            if (progressCustomColor1 != null)
            {
                progressCustomColor1.Color = "#409eff";
            }

            if (progressCustomColor2 != null)
            {
                progressCustomColor2.Color = new Func<double, string>((percentage) =>
                {
                    if (percentage < 30) return "#909399";
                    if (percentage < 70) return "#e6a23c";
                    return "#67c23a";
                });
            }

            if (progressCustomColor3 != null)
            {
                var colorStops = new List<VistaControls.ProgressColorStop>
                {
                    new VistaControls.ProgressColorStop { Color = "#f56c6c", Percentage = 20 },
                    new VistaControls.ProgressColorStop { Color = "#e6a23c", Percentage = 40 },
                    new VistaControls.ProgressColorStop { Color = "#5cb87a", Percentage = 60 },
                    new VistaControls.ProgressColorStop { Color = "#1989fa", Percentage = 80 },
                    new VistaControls.ProgressColorStop { Color = "#6f7ad3", Percentage = 100 }
                };
                progressCustomColor3.Color = colorStops;
            }

            // 更新进度条百分比
            UpdateProgressBars();
        }

        private void UpdateProgressBars()
        {
            if (progressCustomColor1 != null) progressCustomColor1.Percentage = _progressPercentage;
            if (progressCustomColor2 != null) progressCustomColor2.Percentage = _progressPercentage;
            if (progressCustomColor3 != null) progressCustomColor3.Percentage = _progressPercentage;
            // if (progressDashboard != null) progressDashboard.Percentage = _dashboardPercentage;
        }

        private void ProgressIncrease_Click(object sender, RoutedEventArgs e)
        {
            _progressPercentage += 10;
            if (_progressPercentage > 100) _progressPercentage = 100;
            UpdateProgressBars();
        }

        private void ProgressDecrease_Click(object sender, RoutedEventArgs e)
        {
            _progressPercentage -= 10;
            if (_progressPercentage < 0) _progressPercentage = 0;
            UpdateProgressBars();
        }

        private void DashboardIncrease_Click(object sender, RoutedEventArgs e)
        {
            _dashboardPercentage += 10;
            if (_dashboardPercentage > 100) _dashboardPercentage = 100;
            UpdateProgressBars();
        }

        private void DashboardDecrease_Click(object sender, RoutedEventArgs e)
        {
            _dashboardPercentage -= 10;
            if (_dashboardPercentage < 0) _dashboardPercentage = 0;
            UpdateProgressBars();
        }

        private void ControlledProgressIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (progressControlled != null)
            {
                var newValue = progressControlled.Percentage + 10;
                if (newValue > 100) newValue = 100;
                progressControlled.Percentage = newValue;
            }
        }

        private void ControlledProgressDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (progressControlled != null)
            {
                var newValue = progressControlled.Percentage - 10;
                if (newValue < 0) newValue = 0;
                progressControlled.Percentage = newValue;
            }
        }

        private sealed class DemoRow
        {
            public string Date { get; set; } = "";
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
        }

        private void InitializeTable()
        {
            var list1 = new List<DemoRow>
            {
                new DemoRow { Date = "2016-05-02", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                new DemoRow { Date = "2016-05-04", Name = "王小虎", Address = "上海市普陀区金沙江路 1517 弄" },
                new DemoRow { Date = "2016-05-01", Name = "王小虎", Address = "上海市普陀区金沙江路 1519 弄" },
                new DemoRow { Date = "2016-05-03", Name = "王小虎", Address = "上海市普陀区金沙江路 1516 弄" },
            };

            if (tableBasic != null) tableBasic.ItemsSource = list1;
            var tblBordered = FindName("tableBordered") as VistaControls.VistaTable;
            if (tblBordered != null) tblBordered.ItemsSource = list1;

            if (tableStateful != null)
            {
                tableStateful.ItemsSource = list1;
                tableStateful.RowClassNameSelector = (row, index) =>
                {
                    if (index == 1) return "warning-row";
                    if (index == 3) return "success-row";
                    return string.Empty;
                };
            }

            if (tableFixedHeader != null)
            {
                var longList = new List<DemoRow>
                {
                    new DemoRow { Date = "2016-05-03", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-02", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-04", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-01", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-08", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-06", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    new DemoRow { Date = "2016-05-07", Name = "王小虎", Address = "上海市普陀区金沙江路 1518 弄" },
                    
                };
                tableFixedHeader.ItemsSource = longList;
            }
        }
    }
}