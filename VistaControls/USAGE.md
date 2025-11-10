# VistaControls 使用文档

本文档详细说明如何使用 VistaControls 控件库中的各个控件。

## 目录

- [快速开始](#快速开始)
- [控件列表](#控件列表)
- [详细使用说明](#详细使用说明)
- [常见问题](#常见问题)
- [最佳实践](#最佳实践)

## 快速开始

### 1. 添加项目引用

在您的 WPF 项目文件中添加对 VistaControls 的引用：

```xml
<ItemGroup>
  <ProjectReference Include="..\VistaControls\VistaControls.csproj" />
</ItemGroup>
```

### 2. 在 XAML 中引入命名空间

在需要使用控件的 XAML 文件顶部添加命名空间声明：

```xml
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vista="clr-namespace:VistaControls;assembly=VistaControls"
        ...>
```

### 3. 使用控件

在 XAML 中直接使用控件：

```xml
<vista:VistaButton Content="点击我" ButtonType="Primary" />
```

## 控件列表

### VistaButton

**命名空间**: `VistaControls`

**类型**: `Button` (继承自 `System.Windows.Controls.Button`)

**状态**: ✅ 已完成

#### 基本用法

```xml
<!-- 基础按钮 -->
<vista:VistaButton Content="默认按钮" />

<!-- 主要按钮 -->
<vista:VistaButton Content="主要按钮" ButtonType="Primary" />

<!-- 成功按钮 -->
<vista:VistaButton Content="成功按钮" ButtonType="Success" />
```

#### 属性说明

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| ButtonType | ButtonType | Default | 按钮类型：Default, Primary, Success, Info, Warning, Danger, Text |
| Plain | bool | false | 是否朴素按钮（白色背景，彩色边框） |
| Round | bool | false | 是否圆角按钮 |
| Circle | bool | false | 是否圆形按钮（仅显示图标） |
| Loading | bool | false | 是否加载中状态 |
| Icon | string | "" | 图标文件名（位于 icons 文件夹） |
| ButtonSize | ButtonSize | Default | 按钮尺寸：Default, Medium, Small, Mini |
| NativeType | string | "button" | 原生 type 属性：button, submit, reset |
| IsEnabled | bool | true | 是否启用（继承自 Button） |

#### 按钮类型示例

```xml
<!-- 默认按钮 -->
<vista:VistaButton Content="默认按钮" />

<!-- 主要按钮 -->
<vista:VistaButton Content="主要按钮" ButtonType="Primary" />

<!-- 成功按钮 -->
<vista:VistaButton Content="成功按钮" ButtonType="Success" />

<!-- 信息按钮 -->
<vista:VistaButton Content="信息按钮" ButtonType="Info" />

<!-- 警告按钮 -->
<vista:VistaButton Content="警告按钮" ButtonType="Warning" />

<!-- 危险按钮 -->
<vista:VistaButton Content="危险按钮" ButtonType="Danger" />

<!-- 文字按钮 -->
<vista:VistaButton Content="文字按钮" ButtonType="Text" />
```

#### 朴素按钮

```xml
<vista:VistaButton Content="朴素按钮" Plain="True" />
<vista:VistaButton Content="主要按钮" ButtonType="Primary" Plain="True" />
<vista:VistaButton Content="成功按钮" ButtonType="Success" Plain="True" />
```

#### 圆角按钮

```xml
<vista:VistaButton Content="圆角按钮" Round="True" />
<vista:VistaButton Content="主要按钮" ButtonType="Primary" Round="True" />
```

#### 圆形按钮

```xml
<vista:VistaButton Icon="el-icon-search.png" Circle="True" Height="40" Width="40" />
<vista:VistaButton Icon="el-icon-edit.png" ButtonType="Primary" Circle="True" Height="40" Width="40" />
```

#### 图标按钮

```xml
<!-- 仅图标 -->
<vista:VistaButton Icon="el-icon-edit.png" ButtonType="Primary" />

<!-- 图标 + 文字 -->
<vista:VistaButton Icon="el-icon-search.png" ButtonType="Primary" Content="搜索" />
```

#### 禁用状态

```xml
<vista:VistaButton Content="禁用按钮" IsEnabled="False" />
<vista:VistaButton Content="禁用按钮" ButtonType="Primary" IsEnabled="False" />
<vista:VistaButton Content="禁用按钮" ButtonType="Primary" Plain="True" IsEnabled="False" />
```

#### 加载状态

```xml
<vista:VistaButton Content="加载中" ButtonType="Primary" Loading="True" />
```

#### 按钮尺寸

```xml
<vista:VistaButton Content="默认按钮" />
<vista:VistaButton Content="中等按钮" ButtonSize="Medium" />
<vista:VistaButton Content="小型按钮" ButtonSize="Small" />
<vista:VistaButton Content="超小按钮" ButtonSize="Mini" />
```

#### 事件说明

VistaButton 继承自 `System.Windows.Controls.Button`，支持所有标准按钮事件：

- `Click` - 点击事件
- `MouseEnter` / `MouseLeave` - 鼠标进入/离开事件
- `MouseDown` / `MouseUp` - 鼠标按下/释放事件

#### 代码示例

**XAML:**
```xml
<vista:VistaButton x:Name="myButton"
                   Content="点击我"
                   ButtonType="Primary"
                   Icon="el-icon-search.png"
                   Click="MyButton_Click" />
```

**C# 代码后台:**
```csharp
private void MyButton_Click(object sender, RoutedEventArgs e)
{
    // 设置加载状态
    myButton.Loading = true;
    
    // 执行异步操作
    Task.Run(async () =>
    {
        await Task.Delay(2000);
        
        // 恢复按钮状态
        Dispatcher.Invoke(() =>
        {
            myButton.Loading = false;
            MessageBox.Show("操作完成！");
        });
    });
}
```

### ButtonGroup

**命名空间**: `VistaControls`

**类型**: `ItemsControl` (继承自 `System.Windows.Controls.ItemsControl`)

**状态**: ✅ 已完成

#### 基本用法

```xml
<vista:ButtonGroup>
    <vista:VistaButton Content="上一页" ButtonType="Primary" Icon="el-icon-search.png" />
    <vista:VistaButton Content="下一页" ButtonType="Primary" />
</vista:ButtonGroup>
```

#### 属性说明

ButtonGroup 是一个容器控件，会自动将子按钮水平排列。它继承自 `ItemsControl`，支持所有标准的 ItemsControl 属性。

#### 代码示例

```xml
<vista:ButtonGroup>
    <vista:VistaButton Icon="el-icon-edit.png" ButtonType="Primary" />
    <vista:VistaButton Icon="el-icon-search.png" ButtonType="Primary" />
    <vista:VistaButton Icon="el-icon-delete.png" ButtonType="Primary" />
</vista:ButtonGroup>
```

### VistaMessage

**命名空间**: `VistaControls`

**类型**: `Control` (继承自 `System.Windows.Controls.Control`)

**状态**: ✅ 已完成

#### 基本用法

首先需要在应用程序中初始化 MessageManager：

```csharp
// 在 MainWindow.xaml.cs 的 Loaded 事件中
private void MainWindow_Loaded(object sender, RoutedEventArgs e)
{
    VistaControls.MessageManager.Initialize(messageContainer, 20);
}
```

在 XAML 中添加消息容器：

```xml
<Window>
    <Grid>
        <Canvas x:Name="messageContainer" IsHitTestVisible="False"/>
        <!-- 其他内容 -->
    </Grid>
</Window>
```

然后就可以使用 MessageManager 显示消息：

```csharp
// 基础用法
VistaControls.MessageManager.Info("这是一条消息提示");

// 成功消息
VistaControls.MessageManager.Success("恭喜你，这是一条成功消息");

// 警告消息
VistaControls.MessageManager.Warning("警告哦，这是一条警告消息");

// 错误消息
VistaControls.MessageManager.Error("错了哦，这是一条错误消息");
```

#### 属性说明

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Message | string | "" | 消息文字 |
| MessageType | MessageType | Info | 消息类型：Success, Warning, Info, Error |
| ShowClose | bool | false | 是否显示关闭按钮 |
| Center | bool | false | 文字是否居中 |
| Duration | int | 3000 | 显示时间（毫秒），0 表示不自动关闭 |
| DangerouslyUseHTMLString | bool | false | 是否将 message 作为 HTML 片段处理 |
| OnClose | Action<VistaMessage> | null | 关闭时的回调函数 |

#### 不同状态示例

```csharp
// 成功消息
VistaControls.MessageManager.Success("恭喜你，这是一条成功消息");

// 警告消息
VistaControls.MessageManager.Warning("警告哦，这是一条警告消息");

// 信息消息
VistaControls.MessageManager.Info("这是一条消息提示");

// 错误消息
VistaControls.MessageManager.Error("错了哦，这是一条错误消息");
```

#### 可关闭消息

```csharp
// 显示可关闭的消息（duration=0 表示不自动关闭）
VistaControls.MessageManager.Show(
    "这是一条消息提示", 
    VistaControls.MessageType.Info, 
    duration: 0, 
    showClose: true
);
```

#### 文字居中

```csharp
VistaControls.MessageManager.Show(
    "居中的文字", 
    VistaControls.MessageType.Info, 
    duration: 3000, 
    showClose: false, 
    center: true
);
```

#### MessageManager 方法

| 方法 | 说明 | 参数 |
|------|------|------|
| `Initialize` | 初始化消息容器 | container: Panel, offset: double = 20 |
| `Show` | 显示消息 | message, type, duration, showClose, center, onClose |
| `Success` | 显示成功消息 | message, duration, showClose |
| `Warning` | 显示警告消息 | message, duration, showClose |
| `Info` | 显示信息消息 | message, duration, showClose |
| `Error` | 显示错误消息 | message, duration, showClose |

#### 代码示例

**C# 代码:**
```csharp
// 基础用法
VistaControls.MessageManager.Info("操作成功！");

// 带关闭按钮
VistaControls.MessageManager.Show(
    "这条消息可以手动关闭", 
    VistaControls.MessageType.Success, 
    duration: 0, 
    showClose: true
);

// 带回调
VistaControls.MessageManager.Show(
    "消息关闭后会执行回调", 
    VistaControls.MessageType.Info, 
    onClose: (msg) => 
    {
        MessageBox.Show("消息已关闭");
    }
);

// 手动关闭消息
var message = VistaControls.MessageManager.Info("这条消息可以手动关闭");
// 稍后关闭
message.Close();
```

### VistaRadio

**命名空间**: `VistaControls`

**类型**: `CheckBox` (继承自 `System.Windows.Controls.CheckBox`)

**状态**: ✅ 已完成

#### 基本用法

```xml
<vista:VistaRadio Content="备选项1" Label="1"/>
<vista:VistaRadio Content="备选项2" Label="2"/>
```

#### 属性说明

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Label | object | null | Radio 的值，用于标识和绑定 |
| Border | bool | false | 是否显示边框 |
| RadioSize | RadioSize | Default | Radio 的尺寸：Default, Medium, Small, Mini |
| IsEnabled | bool | true | 是否启用（继承自 CheckBox） |
| IsChecked | bool? | false | 是否选中（继承自 CheckBox） |

#### 禁用状态

```xml
<vista:VistaRadio Content="禁用" Label="disabled1" IsEnabled="False"/>
<vista:VistaRadio Content="选中且禁用" Label="disabled2" IsEnabled="False" IsChecked="True"/>
```

#### 单选框组

```xml
<vista:RadioGroup x:Name="radioGroup" SelectedValue="3" SelectionChanged="RadioGroup_SelectionChanged">
    <vista:VistaRadio Content="备选项" Label="3"/>
    <vista:VistaRadio Content="备选项" Label="6"/>
    <vista:VistaRadio Content="备选项" Label="9"/>
</vista:RadioGroup>
```

#### 带边框样式

```xml
<vista:VistaRadio Content="备选项1" Label="1" Border="True"/>
<vista:VistaRadio Content="备选项2" Label="2" Border="True"/>
```

#### 不同尺寸（仅对带边框的 Radio 有效）

```xml
<vista:VistaRadio Content="备选项1" Label="1" Border="True" RadioSize="Medium"/>
<vista:VistaRadio Content="备选项2" Label="2" Border="True" RadioSize="Small"/>
<vista:VistaRadio Content="备选项3" Label="3" Border="True" RadioSize="Mini"/>
```

#### 代码示例

**XAML:**
```xml
<vista:RadioGroup x:Name="myRadioGroup" 
                  SelectedValue="{Binding SelectedOption}"
                  SelectionChanged="MyRadioGroup_SelectionChanged">
    <vista:VistaRadio Content="选项1" Label="option1"/>
    <vista:VistaRadio Content="选项2" Label="option2"/>
    <vista:VistaRadio Content="选项3" Label="option3"/>
</vista:RadioGroup>
```

**C# 代码后台:**
```csharp
private void MyRadioGroup_SelectionChanged(object sender, object? e)
{
    if (e != null)
    {
        MessageBox.Show($"选中了: {e}");
    }
}

// 程序化设置选中值
myRadioGroup.SelectedValue = "option2";

// 获取当前选中值
var selected = myRadioGroup.SelectedValue;
```

### RadioGroup

**命名空间**: `VistaControls`

**类型**: `Panel` (继承自 `System.Windows.Controls.Panel`)

**状态**: ✅ 已完成

#### 属性说明

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| SelectedValue | object | null | 当前选中的值 |
| RadioSize | RadioSize | Default | 单选框组尺寸（会应用到所有子单选框） |
| IsGroupDisabled | bool | false | 是否禁用整个组 |

#### 事件说明

- `SelectionChanged` - 选中值改变时触发，参数为新的选中值

### VistaInputNumber

**命名空间**: `VistaControls`

**类型**: `TextBox` (继承自 `System.Windows.Controls.TextBox`)

**状态**: ✅ 已完成

#### 基本用法

```xml
<vista:VistaInputNumber Width="160" Value="1" Min="1" Max="10" />
```

#### 禁用状态

```xml
<vista:VistaInputNumber Width="160" Value="1" IsEnabled="False"/>
```

#### 步数与严格步数

```xml
<vista:VistaInputNumber Width="160" Value="5" Step="2"/>
<vista:VistaInputNumber Width="160" Value="2" Step="2" StepStrictly="True"/>
```

#### 精度（不补零显示）

```xml
<vista:VistaInputNumber Width="160" Value="1" Precision="2" Step="0.1" Max="10"/>
```

> 说明：`Precision` 会与步长的小数位数对齐并限制在 0..15；显示会四舍五入且不补 0（例如 `1.4`）。

#### 尺寸与按钮位置

```xml
<vista:VistaInputNumber Width="160" Value="1" />
<vista:VistaInputNumber Width="160" Value="1" NumberSize="Medium"/>
<vista:VistaInputNumber Width="160" Value="1" NumberSize="Small"/>
<vista:VistaInputNumber Width="160" Value="1" NumberSize="Mini"/>

<vista:VistaInputNumber Width="200" Value="1" ControlsPosition="right"/>
```

#### 属性说明（VistaInputNumber）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Value | double | 0 | 当前值（双向绑定） |
| Min | double | -Infinity | 最小值 |
| Max | double | +Infinity | 最大值 |
| Step | double | 1 | 步长 |
| StepStrictly | bool | false | 是否只能输入 step 的倍数（相对 Min 对齐） |
| Precision | int? | null | 数值精度；自动不小于步长小数位，范围 0..15；显示不补零 |
| NumberSize | InputSize | Default | 尺寸：Default, Medium, Small, Mini |
| Controls | bool | true | 是否显示左右控制按钮 |
| ControlsPosition | string | "default" | 按钮位置："default" 或 "right" |
| Placeholder | string | "" | 占位文本 |

#### 事件说明（VistaInputNumber）

| 事件 | 说明 | 参数 |
|------|------|------|
| Changed | 绑定值被改变时触发 | (oldValue: double, newValue: double) |

#### 方法（VistaInputNumber）

| 方法 | 说明 |
|------|------|
| Focus() | 获取焦点 |
| SelectAll() | 选中全部文字 |

### VistaCheckbox

**命名空间**: `VistaControls`

**类型**: `CheckBox` (继承自 `System.Windows.Controls.CheckBox`)

**状态**: ✅ 已完成

#### 基本用法

```xml
<!-- 单个复选框，绑定布尔值 -->
<vista:VistaCheckbox Content="备选项" IsChecked="{Binding IsAgreed}" />
```

#### 禁用状态

```xml
<vista:VistaCheckbox Content="禁用" IsEnabled="False" />
<vista:VistaCheckbox Content="选中且禁用" IsEnabled="False" IsChecked="True" />
```

#### 多选框组

将多个 Checkbox 放入 `CheckboxGroup` 中，使用 `SelectedValues` 绑定数组类型集合：

```xml
<vista:CheckboxGroup x:Name="checkboxGroup" SelectionChanged="CheckboxGroup_SelectionChanged">
    <vista:VistaCheckbox Content="复选框 A" Label="A"/>
    <vista:VistaCheckbox Content="复选框 B" Label="B"/>
    <vista:VistaCheckbox Content="复选框 C" Label="C"/>
    <vista:VistaCheckbox Content="禁用" Label="D" IsEnabled="False"/>
    <vista:VistaCheckbox Content="选中且禁用" Label="E" IsEnabled="False" IsChecked="True"/>
</vista:CheckboxGroup>
```

> 提示：`Label` 表示该 Checkbox 对应的值，`SelectedValues` 中包含该值即为选中。

#### indeterminate（不确定态）与全选示例

```xml
<StackPanel>
    <vista:VistaCheckbox Content="全选"
                         x:Name="checkAll"
                         IsIndeterminate="True"
                         CheckedChanged="CheckAll_CheckedChanged"/>
    <vista:CheckboxGroup x:Name="cityGroup" SelectionChanged="CityGroup_SelectionChanged">
        <vista:VistaCheckbox Content="上海" Label="上海"/>
        <vista:VistaCheckbox Content="北京" Label="北京"/>
        <vista:VistaCheckbox Content="广州" Label="广州"/>
        <vista:VistaCheckbox Content="深圳" Label="深圳"/>
    </vista:CheckboxGroup>
</StackPanel>
```

#### 可选数量限制（Min/Max）

```xml
<vista:CheckboxGroup Min="1" Max="2">
    <vista:VistaCheckbox Content="上海" Label="上海"/>
    <vista:VistaCheckbox Content="北京" Label="北京"/>
    <vista:VistaCheckbox Content="广州" Label="广州"/>
    <vista:VistaCheckbox Content="深圳" Label="深圳"/>
</vista:CheckboxGroup>
```

#### 带边框与尺寸

```xml
<!-- 单项带边框 -->
<vista:VistaCheckbox Content="备选项1" Border="True"/>

<!-- 组 + 尺寸（仅对带边框样式有效） -->
<vista:CheckboxGroup CheckboxSize="Small">
    <vista:VistaCheckbox Content="备选项1" Label="1" Border="True"/>
    <vista:VistaCheckbox Content="备选项2" Label="2" Border="True" IsEnabled="False"/>
</vista:CheckboxGroup>
```

#### 属性说明（VistaCheckbox）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Label | object | null | 选中状态的值（在 `CheckboxGroup` 或数组绑定时有效） |
| TrueLabel | object | null | 选中时的值（单项独立使用时） |
| FalseLabel | object | null | 未选中时的值（单项独立使用时） |
| Border | bool | false | 是否显示边框样式 |
| CheckboxSize | CheckboxSize | Default | 尺寸：Default, Medium, Small, Mini（仅 `Border=True` 时有效） |
| IsEnabled | bool | true | 是否启用（继承） |
| IsChecked | bool? | false | 是否选中（继承） |
| IsIndeterminate | bool | false | 不确定态（仅样式控制） |

#### 事件说明（VistaCheckbox）

| 事件 | 说明 | 参数 |
|------|------|------|
| CheckedChanged | 当绑定值变化时触发 | value: object?（TrueLabel/FalseLabel 或布尔值） |

#### 属性说明（CheckboxGroup）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| SelectedValues | IList | null | 当前选中的值集合（Label 集合） |
| CheckboxSize | CheckboxSize | Default | 组内复选框尺寸（会同步到子项） |
| IsGroupDisabled | bool | false | 是否禁用整个组（子项不可交互） |
| Min | int | 0 | 最小可选数量 |
| Max | int | int.MaxValue | 最大可选数量 |
| TextColor | string | #ffffff | 按钮形态激活时文本颜色（预留） |
| Fill | string | #409EFF | 按钮形态激活时填充/边框色（预留） |

#### 事件说明（CheckboxGroup）

| 事件 | 说明 | 参数 |
|------|------|------|
| SelectionChanged | 选中集合改变时触发 | IList（新的集合） |

### VistaInput

**命名空间**: `VistaControls`

**类型**: `TextBox` (继承自 `System.Windows.Controls.TextBox`)

**状态**: ✅ 已完成

#### 基本用法

```xml
<vista:VistaInput Width="200" Placeholder="请输入内容"/>
```

#### 禁用状态

```xml
<vista:VistaInput Width="200" Placeholder="请输入内容" IsEnabled="False"/>
```

#### 可清空

```xml
<vista:VistaInput Width="200" Placeholder="请输入内容" Clearable="True"/>
```

#### 密码框（默认显示点，点击图标可切换为明文查看）

```xml
<vista:VistaInput Width="200" Placeholder="请输入密码" ShowPassword="True"/>
```

#### 字数统计

```xml
<vista:VistaInput Width="200"
                  Placeholder="最多 20 个字符"
                  MaxLength="20"
                  ShowWordLimit="True"/>
```

#### 不同尺寸

```xml
<vista:VistaInput Width="200" Placeholder="默认尺寸"/>
<vista:VistaInput Width="200" Placeholder="中等尺寸" InputSize="Medium"/>
<vista:VistaInput Width="200" Placeholder="小型尺寸" InputSize="Small"/>
<vista:VistaInput Width="200" Placeholder="超小尺寸" InputSize="Mini"/>
```

#### 属性说明（VistaInput）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Placeholder | string | "" | 占位文本（文本为空时显示） |
| Clearable | bool | false | 是否显示清空按钮 |
| ShowPassword | bool | false | 是否显示密码切换按钮（默认显示为点） |
| ShowWordLimit | bool | false | 是否显示字数统计（结合 `MaxLength` 使用） |
| InputSize | InputSize | Default | 尺寸：Default, Medium, Small, Mini |
| IsEnabled | bool | true | 是否启用（继承） |
| Text | string | "" | 输入框文本（继承） |
| MaxLength | int | 0 | 最大输入长度，0 表示不限制（继承） |

#### 事件说明（VistaInput）

| 事件 | 说明 | 参数 |
|------|------|------|
| Cleared | 点击清空按钮时触发 | 无 |
| TextChanged | 文本变化时触发（继承） | TextChangedEventArgs |
| GotFocus/LostFocus | 获取/失去焦点（继承） | RoutedEventArgs |

#### 方法（VistaInput）

| 方法 | 说明 |
|------|------|
| Focus() | 使输入框获取焦点 |
| Blur() | 使输入框失去焦点（通过清除键盘焦点实现） |
| SelectAll() | 选中输入框中全部文字 |

## 详细使用说明

### 样式自定义

所有控件都支持通过样式进行自定义。您可以在应用程序的资源字典中覆盖默认样式：

```xml
<Application.Resources>
    <Style TargetType="{x:Type vista:VistaButton}">
        <Setter Property="FontSize" Value="16"/>
        <Setter Property="Padding" Value="25,15"/>
    </Style>
</Application.Resources>
```

### 数据绑定

控件支持标准 WPF 数据绑定：

```xml
<vista:VistaButton Content="{Binding ButtonText}"
                   ButtonType="{Binding ButtonType}"
                   IsEnabled="{Binding IsButtonEnabled}"
                   Click="{Binding ButtonClickCommand}" />
```

### MVVM 模式支持

VistaButton 完全支持 MVVM 模式：

```xml
<vista:VistaButton Content="保存"
                   ButtonType="Primary"
                   Command="{Binding SaveCommand}"
                   CommandParameter="{Binding SelectedItem}" />
```

## 常见问题

### Q: 图标不显示？

**A**: 确保：
1. 图标文件位于 `icons` 文件夹中
2. 图标文件名正确（包括扩展名，如 `el-icon-search.png`）
3. 图标文件已设置为资源（在项目文件中）

### Q: 按钮样式不正确？

**A**: 确保在应用程序的 `App.xaml` 中正确引用了 VistaControls 的资源字典：

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/VistaControls;component/Themes/Generic.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

### Q: 如何在代码中动态创建按钮？

**A**: 
```csharp
var button = new VistaControls.VistaButton
{
    Content = "动态按钮",
    ButtonType = VistaControls.ButtonType.Primary,
    Icon = "el-icon-search.png",
    Width = 120,
    Height = 40
};
button.Click += (s, e) => MessageBox.Show("点击了按钮");
container.Children.Add(button);
```

### Q: Loading 状态如何控制？

**A**: 
```csharp
// 开始加载
myButton.Loading = true;
myButton.IsEnabled = false; // Loading 会自动禁用按钮

// 完成加载
myButton.Loading = false;
```

### Q: 按钮支持哪些图标格式？

**A**: 目前支持 PNG 格式的图标文件。图标文件应放在 `icons` 文件夹中，并在项目文件中设置为资源。

## 最佳实践

1. **命名空间别名**: 使用简短的命名空间别名（如 `vista`）以提高 XAML 可读性。

2. **样式管理**: 建议在应用程序级别定义控件样式，而不是在每个使用控件的地方重复设置属性。

3. **图标管理**: 
   - 将图标文件统一放在 `icons` 文件夹中
   - 使用有意义的文件名
   - 确保图标尺寸适中（推荐 16x16 或 24x24 像素）

4. **性能优化**: 
   - 避免在大量按钮上使用复杂的绑定
   - 合理使用 Loading 状态来防止重复点击

5. **可访问性**: 
   - 为按钮设置有意义的 Content
   - 使用 ToolTip 提供额外信息
   - 确保按钮在禁用状态下有清晰的视觉反馈

6. **错误处理**: 在使用按钮时，注意处理可能的异常情况，特别是在异步操作时。

## 示例项目

完整的示例代码请参考 `VistaControls.Demo` 项目，其中包含了所有按钮类型和功能的演示。

## 更新说明

本文档会随着新控件的添加和现有控件的更新而持续更新。请定期查看以获取最新信息。

---

**最后更新**: 2024年12月（VistaButton 控件完成）
