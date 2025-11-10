# VistaControls

一个基于 C# 和 WPF 的自定义控件库，提供美观、易用的 Windows 桌面应用程序控件组件。

## 项目简介

VistaControls 是一个 WPF 自定义控件库项目，旨在为开发者提供一套功能完善、样式美观的自定义控件，帮助快速构建现代化的 Windows 桌面应用程序。

## 项目结构

```
VistaControls/
├── VistaControls/              # 自定义控件库主项目
│   ├── Themes/                 # 控件样式和主题定义
│   │   └── Generic.xaml        # 默认样式资源字典
│   ├── icons/                  # 图标资源文件夹
│   └── CustomControl1.cs       # 示例自定义控件
│
├── VistaControls.Demo/         # 演示程序
│   ├── MainWindow.xaml         # 主窗口
│   └── App.xaml               # 应用程序入口
│
└── VistaControls.sln          # 解决方案文件
```

## 技术栈

- **.NET 6.0** - 目标框架
- **WPF (Windows Presentation Foundation)** - UI 框架
- **C#** - 编程语言

## 环境要求

- .NET 6.0 SDK 或更高版本
- Visual Studio 2022 或 JetBrains Rider（推荐）
- Windows 操作系统

## 快速开始

### 1. 克隆或下载项目

```bash
git clone <repository-url>
cd VistaControls
```

### 2. 还原依赖

```bash
dotnet restore
```

### 3. 构建项目

```bash
dotnet build
```

### 4. 运行演示程序

```bash
cd VistaControls.Demo
dotnet run
```

## 在您的项目中使用 VistaControls

### 方法一：项目引用（开发阶段）

1. 在您的 WPF 项目中添加项目引用：
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\VistaControls\VistaControls.csproj" />
   </ItemGroup>
   ```

2. 在 XAML 文件中添加命名空间：
   ```xml
   xmlns:vista="clr-namespace:VistaControls;assembly=VistaControls"
   ```

3. 使用控件：
   ```xml
   <vista:CustomControl1 />
   ```

### 方法二：NuGet 包（发布后）

待项目完善后，将提供 NuGet 包供直接安装使用。

## 已实现的控件

### VistaButton
- **状态**: ✅ 已完成
- **描述**: 仿 Element UI 风格的按钮控件，支持多种类型、尺寸和状态
- **功能特性**:
  - 支持 6 种按钮类型：Default、Primary、Success、Info、Warning、Danger、Text
  - 支持朴素按钮（Plain）、圆角按钮（Round）、圆形按钮（Circle）
  - 支持图标显示、加载状态、禁用状态
  - 支持 4 种尺寸：Default、Medium、Small、Mini
- **详细说明**: 参见 [使用文档](USAGE.md)

### ButtonGroup
- **状态**: ✅ 已完成
- **描述**: 按钮组控件，用于将多个按钮组合在一起
- **详细说明**: 参见 [使用文档](USAGE.md)

### VistaMessage
- **状态**: ✅ 已完成
- **描述**: 消息提示控件，常用于主动操作后的反馈提示
- **功能特性**:
  - 支持 4 种消息类型：Success、Warning、Info、Error
  - 支持自动关闭（可配置时长）
  - 支持手动关闭（显示关闭按钮）
  - 支持文字居中
  - 支持淡入淡出动画效果
- **详细说明**: 参见 [使用文档](USAGE.md)

### VistaRadio
- **状态**: ✅ 已完成
- **描述**: 单选框控件，用于在一组备选项中进行单选
- **功能特性**:
  - 支持基础单选框和单选框组
  - 支持禁用状态
  - 支持带边框样式
  - 支持 4 种尺寸：Default、Medium、Small、Mini
  - 支持数据绑定和事件处理
- **详细说明**: 参见 [使用文档](USAGE.md)

### RadioGroup
- **状态**: ✅ 已完成
- **描述**: 单选框组控件，用于管理一组互斥的单选框
- **详细说明**: 参见 [使用文档](USAGE.md)

### VistaCheckbox
- **状态**: ✅ 已完成
- **描述**: 多选框控件，支持基础、多选组、禁用、带边框和尺寸
- **功能特性**:
  - 单个复选框与布尔值绑定
  - 复选框组与数组绑定
  - 支持禁用、带边框样式与多尺寸
  - 支持 `indeterminate`（样式控制）与“全选”示例
- **详细说明**: 参见 [使用文档](USAGE.md)

### CheckboxGroup
- **状态**: ✅ 已完成
- **描述**: 管理一组 Checkbox 的容器，支持最小/最大可选数量限制
- **功能特性**:
  - 绑定选中项集合
  - `Min`/`Max` 限制可勾选数量
  - 组禁用与尺寸联动
- **详细说明**: 参见 [使用文档](USAGE.md)

### VistaInput
- **状态**: ✅ 已完成
- **描述**: 输入框控件，支持占位符、可清空、密码显示切换、字数统计与尺寸
- **功能特性**:
  - 占位符显示（文本为空时）
  - 可清空按钮（`Clearable`）
  - 密码模式（默认显示为点，支持切换明文查看）
  - 字数统计（`ShowWordLimit`）和多尺寸
- **详细说明**: 参见 [使用文档](USAGE.md)

### VistaInputNumber
- **状态**: ✅ 已完成
- **描述**: 数字计数器输入框，支持范围、步长、精度、严格步长、尺寸和按钮布局
- **功能特性**:
  - `Min/Max` 范围限制与键盘上下键步进
  - `Step` 步长与 `StepStrictly` 严格步长（仅允许步长倍数）
  - `Precision` 精度（自动不小于步长小数位，四舍五入且不补零）
  - `Controls` 控制按钮与 `ControlsPosition="right"` 右侧按钮布局
  - 文本水平居中显示
- **详细说明**: 参见 [使用文档](USAGE.md)
### CustomControl1
- **状态**: 示例控件（待完善）
- **描述**: 基础自定义控件模板
- **详细说明**: 参见 [使用文档](USAGE.md)

## 开发计划

- [ ] 完善现有控件功能
- [ ] 添加更多实用控件
- [ ] 完善样式和主题系统
- [ ] 编写单元测试
- [ ] 发布 NuGet 包

## 贡献指南

欢迎提交 Issue 和 Pull Request 来帮助改进这个项目。

## 许可证

[待定]

## 更新日志

### 2024-12 - VistaRadio 控件
- ✅ 实现 VistaRadio 控件，支持 Element UI 风格的所有功能
- ✅ 实现 RadioGroup 控件，用于管理单选框组
- ✅ 支持禁用状态、带边框样式、多种尺寸
- ✅ 支持数据绑定和事件处理
- ✅ 创建完整的演示程序

### 2024-12 - VistaMessage 控件
- ✅ 实现 VistaMessage 控件，支持 Element UI 风格的所有功能
- ✅ 实现 MessageManager 服务类，用于管理消息显示
- ✅ 支持自动关闭、手动关闭、文字居中等功能
- ✅ 添加淡入淡出动画效果
- ✅ 创建完整的演示程序

### 2025-01 - VistaCheckbox / CheckboxGroup / VistaInput
- ✅ 实现 VistaCheckbox 控件及 CheckboxGroup 组容器
- ✅ 支持单项、多选组、禁用、带边框、尺寸、`Min/Max` 限制与 `indeterminate` 样式
- ✅ 实现 VistaInput 控件，支持占位符、可清空、密码显示切换、字数统计、尺寸
- ✅ 在 Demo 中添加完整示例

### 2024-12 - VistaButton 控件
- ✅ 实现 VistaButton 控件，支持 Element UI 风格的所有功能
- ✅ 实现 ButtonGroup 控件
- ✅ 添加图标支持
- ✅ 完善样式系统
- ✅ 创建完整的演示程序

### 2024 - 项目初始化
- 创建项目基础结构
- 添加示例控件 CustomControl1
- 创建项目文档和使用文档

---

**注意**: 本项目正在积极开发中，API 可能会发生变化。建议在生产环境使用前进行充分测试。

