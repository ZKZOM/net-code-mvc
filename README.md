# Todo MVC 应用程序

这是一个使用 .NET 8.0 MVC 和 Entity Framework Core 构建的简单待办事项应用程序。

## 项目概述

这个项目是一个完整的待办事项管理系统，包含以下功能：
- 查看所有待办事项
- 添加新的待办事项
- 编辑现有待办事项
- 标记待办事项为已完成
- 删除待办事项

项目使用 MVC (Model-View-Controller) 架构模式和 Entity Framework Core 进行数据访问。

## 使用 Docker 运行

本项目已配置为使用 Docker 容器运行，包含 Web 应用和 SQL Server 数据库。

### 前提条件

- 安装 [Docker](https://www.docker.com/products/docker-desktop)
- 安装 [Docker Compose](https://docs.docker.com/compose/install/)（Docker Desktop for Windows/Mac 已包含）

### 启动应用程序

#### Windows

```
start.bat
```

#### Linux/Mac

```
chmod +x start.sh
./start.sh
```

### 访问应用程序

启动后，可以通过以下地址访问应用程序：

- Web 应用程序：http://localhost:8080
- SQL Server：
  - 服务器：localhost,1433
  - 用户名：sa
  - 密码：Your_Strong_Password123!

## 手动安装步骤

如果不使用 Docker，可以按照以下步骤安装：

1. 安装 .NET SDK 8.0 (https://dotnet.microsoft.com/download)
2. 安装 SQL Server (https://www.microsoft.com/sql-server/sql-server-downloads)
3. 克隆此仓库
4. 修改 `TodoMvcApp/appsettings.json` 中的连接字符串以匹配您的 SQL Server 配置
5. 在项目根目录运行以下命令：
   ```
   dotnet restore
   dotnet ef database update --project TodoMvcApp
   dotnet run --project TodoMvcApp
   ```
6. 在浏览器中访问 https://localhost:5001

## 项目结构

```
TodoMvcApp/
├── Controllers/         # 控制器类
│   ├── HomeController.cs
│   ├── TodoController.cs
│   └── TodoApiController.cs
├── Models/              # 数据模型类
│   ├── ErrorViewModel.cs
│   ├── Todo.cs
│   └── TodoDbContext.cs
├── Views/               # 视图文件
│   ├── Home/
│   ├── Todo/
│   └── Shared/
├── wwwroot/             # 静态文件 (CSS, JS, 图片)
│   ├── css/
│   ├── js/
│   └── lib/
├── Dockerfile           # Docker 配置文件
├── appsettings.json     # 应用程序配置
└── Program.cs           # 应用程序入口点
```

## 技术栈

- .NET 8.0
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- HTML/CSS/JavaScript
- Bootstrap
- Docker

## 学习要点

通过本项目，您将学习：
1. MVC 架构模式
2. Entity Framework Core 基础
3. CRUD 操作实现
4. 前后端交互
5. 数据验证
6. 依赖注入
7. RESTful API 设计
8. Docker 容器化 