# TodoMvcApp 学习文档 - 第2部分

## 3. Entity Framework Core 基础

### 3.1 ORM 概念

ORM（对象关系映射）是一种编程技术，它将面向对象的领域模型映射到关系型数据库。Entity Framework Core 是 .NET 平台上的一个轻量级、可扩展、开源的 ORM 框架。

主要优势：
- 减少手写 SQL 代码
- 自动处理数据库连接管理
- 将数据库表映射为 C# 类
- 提供 LINQ 查询支持

### 3.2 Code First 开发模式

Code First 是 Entity Framework 的一种开发方法，开发人员先创建领域模型类（C# 类），然后 EF Core 根据这些类生成数据库表。

TodoMvcApp 项目使用 Code First 方法：

1. 定义模型类（`Todo.cs`）
2. 创建数据库上下文（`TodoDbContext.cs`）
3. 配置连接字符串（`appsettings.json`）
4. 应用迁移或自动创建数据库

### 3.3 数据库上下文

数据库上下文（`DbContext`）是 Entity Framework Core 的核心组件，它是应用程序和数据库之间的桥梁。

在 TodoMvcApp 中，`TodoDbContext` 类继承自 `DbContext`：

```csharp
public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 种子数据
        modelBuilder.Entity<Todo>().HasData(
            new Todo
            {
                Id = 1,
                Title = "学习 ASP.NET Core MVC",
                Description = "了解 MVC 模式和 ASP.NET Core 框架的基础知识",
                IsComplete = false
            },
            // 更多种子数据...
        );
    }
}
```

关键点：
- `DbSet<Todo> Todos` 属性表示数据库中的 Todos 表
- 构造函数接收配置选项，通常通过依赖注入提供
- `OnModelCreating` 方法用于配置模型和添加种子数据

### 3.4 数据库操作

Entity Framework Core 提供了多种方法来操作数据：

1. **查询数据**：
```csharp
// 获取所有待办事项
var todos = await _context.Todos.ToListAsync();

// 获取特定ID的待办事项
var todo = await _context.Todos.FindAsync(id);

// 使用LINQ查询
var activeTodos = await _context.Todos.Where(t => !t.IsComplete).ToListAsync();
```

2. **添加数据**：
```csharp
var newTodo = new Todo { Title = "新待办事项" };
_context.Todos.Add(newTodo);
await _context.SaveChangesAsync();
```

3. **更新数据**：
```csharp
var todo = await _context.Todos.FindAsync(id);
todo.Title = "已更新的标题";
_context.Update(todo);
await _context.SaveChangesAsync();
```

4. **删除数据**：
```csharp
var todo = await _context.Todos.FindAsync(id);
_context.Todos.Remove(todo);
await _context.SaveChangesAsync();
```

## 4. 项目结构详解

### 4.1 目录结构

TodoMvcApp 项目的目录结构如下：

```
TodoMvcApp/
├── Controllers/         # 控制器类
│   ├── HomeController.cs
│   ├── TodoController.cs
│   └── TodoApiController.cs
├── Models/              # 数据模型
│   ├── Todo.cs
│   └── TodoDbContext.cs
├── Views/               # Razor视图文件
│   ├── Home/
│   ├── Todo/
│   └── Shared/          # 共享视图组件
├── wwwroot/             # 静态资源文件
│   ├── css/
│   └── js/
├── Program.cs           # 应用程序入口点
├── appsettings.json     # 应用程序配置
└── Dockerfile           # Docker容器配置
```

### 4.2 核心组件和职责

#### 4.2.1 Program.cs

`Program.cs` 是应用程序的入口点，负责配置和启动 Web 应用：

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 确保数据库已创建并应用迁移
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoDbContext>();
    context.Database.EnsureCreated();
}

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

主要职责：
- 配置依赖注入服务
- 设置数据库连接
- 配置中间件管道
- 设置路由规则
- 启动应用程序

#### 4.2.2 appsettings.json

`appsettings.json` 包含应用程序的配置信息：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  
  "ConnectionStrings": {
    "DefaultConnection": "Server=db;Database=TodoMvcApp;User=sa;Password=Your_Strong_Password123!;TrustServerCertificate=True;Connection Timeout=30;ConnectRetryCount=5;ConnectRetryInterval=10;"
  },
  
  "AllowedHosts": "*"
} 