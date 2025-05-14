# TodoMvcApp 学习文档 - 第1部分

## 1. 项目概述

### 1.1 项目介绍

TodoMvcApp 是一个基于 ASP.NET Core MVC 和 Entity Framework Core 开发的待办事项管理系统。该项目展示了一个完整的 Web 应用程序开发流程，包括前端界面、后端逻辑和数据库交互。

### 1.2 功能特点

- 创建、查看、编辑和删除待办事项
- 标记待办事项为已完成/未完成
- 设置待办事项截止日期
- 搜索和筛选待办事项
- RESTful API 接口支持

### 1.3 技术栈

**后端技术：**
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server 数据库
- C# 编程语言

**前端技术：**
- HTML5 + CSS3
- Bootstrap 5 框架
- JavaScript 和 jQuery
- Razor 视图引擎

**部署技术：**
- Docker 容器化
- Docker Compose 多容器管理

### 1.4 系统架构图

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   浏览器    │     │  ASP.NET    │     │  SQL Server │
│  (前端UI)   │◄───►│  Core MVC   │◄───►│   数据库    │
└─────────────┘     └─────────────┘     └─────────────┘
                          │
                    ┌─────▼─────┐
                    │Entity Frame│
                    │work Core   │
                    └───────────┘
```

## 2. ASP.NET Core MVC 基础

### 2.1 MVC 模式简介

MVC（Model-View-Controller）是一种软件架构模式，它将应用程序分为三个主要组件：

- **模型（Model）**：表示应用程序的数据和业务逻辑
- **视图（View）**：负责数据的可视化展示
- **控制器（Controller）**：处理用户输入并协调模型和视图

这种分离可以提高代码的可维护性、可测试性和可重用性。

### 2.2 ASP.NET Core MVC 工作流程

1. **请求接收**：用户在浏览器中发起请求
2. **路由解析**：路由系统确定哪个控制器和操作方法处理请求
3. **控制器处理**：控制器执行操作方法，与模型交互获取数据
4. **视图渲染**：控制器选择视图并传递模型数据
5. **响应返回**：渲染后的HTML返回给浏览器

### 2.3 项目中的 MVC 组件

在 TodoMvcApp 项目中：

- **模型（Models 文件夹）**：
  - `Todo.cs`：定义待办事项的数据结构
  - `TodoDbContext.cs`：定义数据库上下文

- **视图（Views 文件夹）**：
  - `Todo/Index.cshtml`：显示待办事项列表
  - `Todo/Create.cshtml`：创建新待办事项的表单
  - `Todo/Edit.cshtml`：编辑待办事项的表单
  - `Todo/Delete.cshtml`：确认删除待办事项

- **控制器（Controllers 文件夹）**：
  - `TodoController.cs`：处理与待办事项相关的 MVC 请求
  - `TodoApiController.cs`：提供 RESTful API 接口

### 2.4 路由系统

ASP.NET Core MVC 使用路由系统将 URL 映射到控制器和操作方法。在 `Program.cs` 中定义了默认路由模式：

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

这个路由模式表示：
- URL 格式为 `/[控制器]/[操作]/[可选ID]`
- 如果未指定控制器，默认为 `Home`
- 如果未指定操作，默认为 `Index`
- ID 参数是可选的

例如：
- `/Todo` 会路由到 `TodoController` 的 `Index` 方法
- `/Todo/Create` 会路由到 `TodoController` 的 `Create` 方法
- `/Todo/Edit/5` 会路由到 `TodoController` 的 `Edit` 方法，并传递 ID 为 5 