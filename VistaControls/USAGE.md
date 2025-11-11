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

### VistaSelect / VistaOption / VistaOptionGroup

**命名空间**: `VistaControls`

**类型**: `VistaSelect` (继承自 `ItemsControl`)，子项使用 `VistaOption` 或分组 `VistaOptionGroup`

**状态**: ✅ 已完成

#### 基本用法（单选）

```xml
<vista:VistaSelect Placeholder="请选择">
  <vista:VistaOption Label="黄金糕" Value="选项1"/>
  <vista:VistaOption Label="双皮奶" Value="选项2"/>
</vista:VistaSelect>
```

#### 禁用选项与禁用状态
```xml
<vista:VistaSelect Placeholder="请选择" IsSelectDisabled="True">
  <vista:VistaOption Label="黄金糕" Value="选项1"/>
</vista:VistaSelect>

<vista:VistaSelect Placeholder="请选择">
  <vista:VistaOption Label="双皮奶" Value="选项2" IsOptionDisabled="True"/>
</vista:VistaSelect>
```

#### 可清空单选
```xml
<vista:VistaSelect Placeholder="请选择" Clearable="True">
  <vista:VistaOption Label="黄金糕" Value="选项1"/>
  <vista:VistaOption Label="双皮奶" Value="选项2"/>
</vista:VistaSelect>
```

#### 多选与折叠标签
```xml
<vista:VistaSelect Placeholder="请选择" Multiple="True">
  <vista:VistaOption Label="黄金糕" Value="选项1"/>
  <vista:VistaOption Label="双皮奶" Value="选项2"/>
</vista:VistaSelect>

<vista:VistaSelect Placeholder="请选择" Multiple="True" CollapseTags="True"/>
```

#### 分组
```xml
<vista:VistaSelect Placeholder="请选择">
  <vista:VistaOptionGroup Label="热门城市">
    <vista:VistaOption Label="上海" Value="Shanghai"/>
    <vista:VistaOption Label="北京" Value="Beijing"/>
  </vista:VistaOptionGroup>
</vista:VistaSelect>
```

#### 属性说明（VistaSelect）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Placeholder | string | "请选择" | 占位文本 |
| IsSelectDisabled | bool | false | 禁用选择器 |
| Clearable | bool | false | 是否可清空 |
| Multiple | bool | false | 是否多选 |
| CollapseTags | bool | false | 多选是否折叠为计数 |
| SelectedValue | object | null | 单选值（双向绑定） |
| SelectedValues | IList | null | 多选值集合（双向绑定） |

#### 属性说明（VistaOption）
| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Label | string | "" | 显示文本 |
| Value | object | null | 选项值 |
| IsOptionDisabled | bool | false | 是否禁用该选项 |

#### 事件说明（VistaSelect）
| 事件 | 说明 | 参数 |
|------|------|------|
| Change | 值变化 | 单选：object?；多选：IList |

### VistaCascader

**命名空间**: `VistaControls`

**类型**: `VistaCascader` (继承自 `Control`)，数据模型 `CascaderOption`

**状态**: ✅ 已完成

#### 基本用法（click 展开）
```xml
<vista:VistaCascader Width="360" Placeholder="请选择"
                     Options="{Binding CascaderOptions}" />
```

#### hover 展开
```xml
<vista:VistaCascader Width="360" Placeholder="请选择"
                     Options="{Binding CascaderOptions}"
                     ExpandTrigger="Hover"/>
```

#### 可清空、仅显示最后一级
```xml
<vista:VistaCascader Options="{Binding CascaderOptions}" Clearable="True"/>
<vista:VistaCascader Options="{Binding CascaderOptions}" ShowAllLevels="False"/>
```

#### 多选与折叠标签
```xml
<vista:VistaCascader Options="{Binding CascaderOptions}" Multiple="True" Clearable="True"/>
<vista:VistaCascader Options="{Binding CascaderOptions}" Multiple="True" CollapseTags="True" Clearable="True"/>
```

#### CascaderOption 模型
```csharp
public class CascaderOption
{
    public string Label { get; set; }
    public object? Value { get; set; }
    public bool Disabled { get; set; }
    public ObservableCollection<CascaderOption> Children { get; set; }
}
```

#### 属性说明（VistaCascader）
| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Options | ObservableCollection<CascaderOption> | [] | 级联数据源 |
| ExpandTrigger | ExpandTrigger | Click | 展开触发方式：Click / Hover |
| Clearable | bool | false | 是否可清空 |
| ShowAllLevels | bool | true | 单选显示完整路径；false 显示末级 |
| Multiple | bool | false | 是否多选 |
| CollapseTags | bool | false | 多选折叠为计数 |
| SelectedPath | ObservableCollection<object> | [] | 单选路径（双向绑定） |
| SelectedPaths | ObservableCollection<IList> | [] | 多选路径集合（双向绑定） |

#### 事件说明（VistaCascader）
| 事件 | 说明 | 参数 |
|------|------|------|
| Change | 选择变化 | 单选：IList（当前路径）；多选：IEnumerable<IList>（全部路径） |

### VistaSwitch

**命名空间**: `VistaControls`

**类型**: `VistaSwitch` (继承自 `ToggleButton`)

**状态**: ✅ 已完成

#### 基本用法
```xml
<vista:VistaSwitch Width="40"/>
```

#### 自定义颜色
```xml
<vista:VistaSwitch Width="40" 
                   ActiveColor="#13ce66" 
                   InactiveColor="#ff4949"/>
```

#### 文字描述
```xml
<vista:VistaSwitch Width="60"
                   ActiveText="按月付费" 
                   InactiveText="按年付费"/>
```

文字描述会显示在开关的左右两侧，根据开关状态改变颜色：
- 关闭状态：左侧文字（InactiveText）蓝色，右侧文字（ActiveText）灰色
- 打开状态：左侧文字（InactiveText）灰色，右侧文字（ActiveText）蓝色

#### 扩展的 value 类型
```xml
<vista:VistaSwitch Width="40"
                   ActiveValue="100" 
                   InactiveValue="0"
                   ActiveColor="#13ce66" 
                   InactiveColor="#ff4949"/>
```

#### 禁用状态
```xml
<vista:VistaSwitch Width="40" IsEnabled="False" IsChecked="True"/>
<vista:VistaSwitch Width="40" IsEnabled="False" IsChecked="False"/>
```

#### 属性说明（VistaSwitch）
| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Value | object | false | 绑定值（boolean / string / number） |
| Width | double | 40 | Switch 的宽度（像素） |
| ActiveIconClass | string | "" | Switch 打开时所显示图标的类名 |
| InactiveIconClass | string | "" | Switch 关闭时所显示图标的类名 |
| ActiveText | string | "" | Switch 打开时的文字描述 |
| InactiveText | string | "" | Switch 关闭时的文字描述 |
| ActiveValue | object | true | Switch 打开时的值 |
| InactiveValue | object | false | Switch 关闭时的值 |
| ActiveColor | Brush | #409EFF | Switch 打开时的背景色 |
| InactiveColor | Brush | #C0CCDA | Switch 关闭时的背景色 |
| SwitchName | string | "" | Switch 对应的 name 属性 |
| ValidateEvent | bool | true | 改变 switch 状态时是否触发表单的校验 |
| IsEnabled | bool | true | 是否禁用（继承自 ToggleButton） |

#### 事件说明（VistaSwitch）
| 事件 | 说明 | 参数 |
|------|------|------|
| Change | Switch 状态发生变化时的回调函数 | 新状态的值（object） |

#### 方法说明（VistaSwitch）
| 方法 | 说明 | 参数 |
|------|------|------|
| Focus | 使 Switch 获取焦点 | - |

### VistaSlider

**命名空间**: `VistaControls`

**类型**: `VistaSlider` (继承自 `Slider`)

**状态**: ✅ 已完成

#### 基本用法
```xml
<vista:VistaSlider Width="400"/>
```

#### 自定义初始值
```xml
<vista:VistaSlider Width="400" Value="50"/>
```

#### 隐藏 Tooltip
```xml
<vista:VistaSlider Width="400" Value="36" ShowTooltip="False"/>
```

#### 格式化 Tooltip
```xml
<vista:VistaSlider x:Name="slider" Width="400" Value="48"/>
```

```csharp
slider.FormatTooltip = val => (val / 100).ToString("F2");
```

#### 禁用状态
```xml
<vista:VistaSlider Width="400" Value="42" IsEnabled="False"/>
```

#### 离散值
```xml
<!-- 不显示间断点 -->
<vista:VistaSlider Width="400" TickFrequency="10"/>

<!-- 显示间断点 -->
<vista:VistaSlider Width="400" TickFrequency="10" ShowStops="True"/>
```

#### 带有输入框
```xml
<vista:VistaSlider Width="400" Value="13" ShowInput="True"/>
```

#### 属性说明（VistaSlider）
| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Value | double | 0 | 绑定值（双向绑定） |
| Minimum | double | 0 | 最小值 |
| Maximum | double | 100 | 最大值 |
| Step | double | 1 | 步长（通过 TickFrequency 设置） |
| ShowInput | bool | false | 是否显示输入框 |
| ShowInputControls | bool | true | 在显示输入框的情况下，是否显示输入框的控制按钮 |
| InputSize | InputSize | Small | 输入框的尺寸：Default, Medium, Small, Mini |
| ShowStops | bool | false | 是否显示间断点 |
| ShowTooltip | bool | true | 是否显示 tooltip |
| FormatTooltip | Func<double, string> | null | 格式化 tooltip message 的委托 |
| Range | bool | false | 是否为范围选择（待完善） |
| RangeValue | double[] | [0, 100] | 范围选择的值（待完善） |
| Vertical | bool | false | 是否竖向模式（待完善） |
| SliderHeight | double | 200 | Slider 高度，竖向模式时必填（待完善） |
| Marks | Dictionary<double, object> | null | 标记字典（待完善） |
| Debounce | int | 300 | 输入时的去抖延迟，毫秒 |
| TooltipClass | string | "" | tooltip 的自定义类名 |
| IsEnabled | bool | true | 是否禁用（继承自 Slider） |

#### 事件说明（VistaSlider）
| 事件 | 说明 | 参数 |
|------|------|------|
| Change | 值改变时触发（使用鼠标拖曳时，只在松开鼠标后触发） | RoutedPropertyChangedEventArgs<double> |
| Input | 数据改变时触发（使用鼠标拖曳时，活动过程实时触发） | RoutedPropertyChangedEventArgs<double> |

### VistaTimeSelect

**命名空间**: `VistaControls`

**类型**: `VistaTimeSelect` (继承自 `Control`)

**状态**: ✅ 已完成

#### 基本用法（固定时间点）

```xml
<vista:VistaTimeSelect Width="200" Placeholder="选择时间">
    <vista:VistaTimeSelect.PickerOptions>
        <vista:TimeSelectOptions Start="08:30" Step="00:15" End="18:30"/>
    </vista:VistaTimeSelect.PickerOptions>
</vista:VistaTimeSelect>
```

#### 固定时间范围

```xml
<vista:VistaTimeSelect x:Name="startTime" Width="200" Placeholder="起始时间">
    <vista:VistaTimeSelect.PickerOptions>
        <vista:TimeSelectOptions Start="08:30" Step="00:15" End="18:30"/>
    </vista:VistaTimeSelect.PickerOptions>
</vista:VistaTimeSelect>
<vista:VistaTimeSelect x:Name="endTime" Width="200" Placeholder="结束时间">
    <vista:VistaTimeSelect.PickerOptions>
        <vista:TimeSelectOptions Start="08:30" Step="00:15" End="18:30"/>
    </vista:VistaTimeSelect.PickerOptions>
</vista:VistaTimeSelect>
```

在代码中设置 `MinTime`：

```csharp
startTime.Change += (s, e) =>
{
    if (endTime != null && e != null)
    {
        endTime.PickerOptions.MinTime = e;
    }
};
```

#### 属性说明（VistaTimeSelect）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Value | string | null | 绑定值（双向绑定），格式：HH:mm |
| Placeholder | string | "选择时间" | 占位文本 |
| Readonly | bool | false | 完全只读 |
| IsDisabled | bool | false | 禁用 |
| Editable | bool | true | 文本框可输入 |
| Clearable | bool | true | 是否显示清除按钮 |
| InputSize | InputSize | Default | 输入框尺寸：Default, Medium, Small, Mini |
| PickerOptions | TimeSelectOptions | new TimeSelectOptions() | 时间选择器配置选项 |
| DefaultValue | string | null | 选择器打开时默认显示的时间 |

#### TimeSelectOptions 属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Start | string | "09:00" | 开始时间，格式：HH:mm |
| End | string | "18:00" | 结束时间，格式：HH:mm |
| Step | string | "00:30" | 间隔时间，格式：HH:mm |
| MinTime | string? | null | 最小时间，小于该时间的时间段将被禁用 |
| MaxTime | string? | null | 最大时间，大于该时间的时间段将被禁用 |

#### 事件说明（VistaTimeSelect）

| 事件 | 说明 | 参数 |
|------|------|------|
| Change | 用户确认选定的值时触发 | string?（选中的时间） |
| Blur | 当 input 失去焦点时触发 | EventArgs |
| OnFocus | 当 input 获得焦点时触发 | EventArgs |

#### 方法（VistaTimeSelect）

| 方法 | 说明 |
|------|------|
| Focus() | 使 input 获取焦点 |

### VistaTimePicker

**命名空间**: `VistaControls`

**类型**: `VistaTimePicker` (继承自 `Control`)

**状态**: ✅ 已完成

#### 基本用法（任意时间点）

```xml
<vista:VistaTimePicker Width="200" Placeholder="任意时间点">
    <vista:VistaTimePicker.PickerOptions>
        <vista:TimePickerOptions SelectableRange="18:30:00 - 20:30:00"/>
    </vista:VistaTimePicker.PickerOptions>
</vista:VistaTimePicker>
```

#### 箭头控制模式

```xml
<vista:VistaTimePicker Width="200" Placeholder="任意时间点" ArrowControl="True">
    <vista:VistaTimePicker.PickerOptions>
        <vista:TimePickerOptions SelectableRange="18:30:00 - 20:30:00"/>
    </vista:VistaTimePicker.PickerOptions>
</vista:VistaTimePicker>
```

#### 任意时间范围

```xml
<vista:VistaTimePicker Width="300"
                       IsRange="True"
                       StartPlaceholder="开始时间"
                       EndPlaceholder="结束时间"
                       Placeholder="选择时间范围">
</vista:VistaTimePicker>
```

#### 可清空

```xml
<vista:VistaTimePicker Width="200" Placeholder="任意时间点" Clearable="True">
    <vista:VistaTimePicker.PickerOptions>
        <vista:TimePickerOptions SelectableRange="18:30:00 - 20:30:00"/>
    </vista:VistaTimePicker.PickerOptions>
</vista:VistaTimePicker>
```

#### 属性说明（VistaTimePicker）

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| Value | DateTime? | null | 绑定值（双向绑定），非范围模式 |
| ValueRange | DateTime[] | null | 绑定值（双向绑定），范围模式，长度为 2 的数组 |
| Placeholder | string | "选择时间" | 非范围选择时的占位内容 |
| StartPlaceholder | string | "开始时间" | 范围选择时开始日期的占位内容 |
| EndPlaceholder | string | "结束时间" | 范围选择时结束日期的占位内容 |
| Readonly | bool | false | 完全只读 |
| IsDisabled | bool | false | 禁用 |
| Editable | bool | true | 文本框可输入 |
| Clearable | bool | true | 是否显示清除按钮 |
| InputSize | InputSize | Default | 输入框尺寸：Default, Medium, Small, Mini |
| IsRange | bool | false | 是否为时间范围选择 |
| ArrowControl | bool | false | 是否使用箭头进行时间选择 |
| RangeSeparator | string | "至" | 选择范围时的分隔符 |
| PickerOptions | TimePickerOptions | new TimePickerOptions() | 时间选择器配置选项 |
| DefaultValue | DateTime? | null | 选择器打开时默认显示的时间 |
| ValueFormat | string? | null | 绑定值的格式，不指定则使用 PickerOptions.Format |

#### TimePickerOptions 属性

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| SelectableRange | object? | null | 可选时间段，例如'18:30:00 - 20:30:00'或者传入数组['09:30:00 - 12:00:00', '14:30:00 - 18:30:00'] |
| Format | string | "HH:mm:ss" | 时间格式化，小时：HH，分：mm，秒：ss，AM/PM A |

#### 事件说明（VistaTimePicker）

| 事件 | 说明 | 参数 |
|------|------|------|
| Change | 用户确认选定的值时触发 | object?（DateTime? 或 DateTime[]?） |
| Blur | 当 input 失去焦点时触发 | EventArgs |
| OnFocus | 当 input 获得焦点时触发 | EventArgs |

#### 方法（VistaTimePicker）

| 方法 | 说明 |
|------|------|
| Focus() | 使 input 获取焦点 |

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
