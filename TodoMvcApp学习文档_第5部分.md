# TodoMvcApp 学习文档 - 第5部分

## 9. 实用练习和扩展建议

### 9.1 添加新功能的步骤

学习完 TodoMvcApp 的基本结构后，你可以尝试添加新功能来巩固所学知识。以下是添加新功能的一般步骤：

#### 9.1.1 添加优先级功能

让我们以添加"待办事项优先级"功能为例：

1. **更新模型**：
```csharp
// Todo.cs 中添加优先级属性
public enum Priority
{
    Low,
    Medium,
    High
}

public class Todo
{
    // 现有属性...
    
    [Display(Name = "优先级")]
    public Priority Priority { get; set; } = Priority.Medium; // 默认为中等优先级
}
```

2. **更新数据库**：
```csharp
// 添加迁移
dotnet ef migrations add AddPriorityToTodo

// 更新数据库
dotnet ef database update
```

3. **更新控制器**：
```csharp
// TodoController.cs 的 Create 方法
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Priority")] Todo todo)
{
    // 现有代码...
}

// TodoController.cs 的 Edit 方法
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsComplete,DueDate,Priority")] Todo todo)
{
    // 现有代码...
}
```

4. **更新视图**：
```html
<!-- Create.cshtml 和 Edit.cshtml 中添加优先级选择 -->
<div class="form-group mb-3">
    <label asp-for="Priority" class="control-label">优先级</label>
    <select asp-for="Priority" asp-items="Html.GetEnumSelectList<TodoMvcApp.Models.Priority>()" class="form-control"></select>
    <span asp-validation-for="Priority" class="text-danger"></span>
</div>

<!-- Index.cshtml 中显示优先级 -->
<span class="badge @(item.Priority == Priority.High ? "bg-danger" : item.Priority == Priority.Medium ? "bg-warning" : "bg-info")">
    @item.Priority
</span>
```

5. **更新 API 控制器**（如果需要）：
```csharp
// TodoApiController.cs 中更新相关方法
```

### 9.2 常见问题和解决方案

#### 9.2.1 视图不显示或样式丢失

**问题**：在 Docker 容器中运行时，视图不显示或 CSS 样式丢失。

**解决方案**：
1. 确保 Dockerfile 中显式复制了视图和静态文件：
```dockerfile
COPY --from=build /src/TodoMvcApp/Views /app/Views
COPY --from=build /src/TodoMvcApp/wwwroot /app/wwwroot
```

2. 在 docker-compose.yml 中添加卷挂载：
```yaml
volumes:
  - ./TodoMvcApp/Views:/app/Views
  - ./TodoMvcApp/wwwroot:/app/wwwroot
```

#### 9.2.2 数据库连接问题

**问题**：应用程序无法连接到数据库。

**解决方案**：
1. 检查连接字符串中的服务器名称是否正确（在 Docker 环境中应该是服务名 `db`）
2. 确保数据库容器已启动并运行
3. 检查数据库用户名和密码是否正确
4. 添加连接重试逻辑：
```csharp
// Program.cs
var maxRetryCount = 5;
var retryDelay = TimeSpan.FromSeconds(10);

for (int i = 0; i < maxRetryCount; i++)
{
    try
    {
        var context = services.GetRequiredService<TodoDbContext>();
        context.Database.EnsureCreated();
        break;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"数据库连接尝试 {i+1}/{maxRetryCount} 失败");
        
        if (i < maxRetryCount - 1)
        {
            logger.LogInformation($"等待 {retryDelay.TotalSeconds} 秒后重试...");
            Thread.Sleep(retryDelay);
        }
    }
}
```

### 9.3 进阶学习路径

掌握了 TodoMvcApp 的基础知识后，你可以探索以下进阶主题：

#### 9.3.1 身份验证和授权

添加用户登录和权限控制：
1. 添加 ASP.NET Core Identity
2. 实现用户注册和登录
3. 为待办事项添加用户关联
4. 实现基于角色的授权

#### 9.3.2 高级 Entity Framework Core 功能

探索更多 EF Core 功能：
1. 复杂查询和 LINQ 表达式
2. 实体关系和导航属性
3. 并发控制
4. 性能优化

#### 9.3.3 前端框架集成

将现代前端框架集成到项目中：
1. 使用 React 或 Vue.js 构建 SPA（单页应用）
2. 实现前后端分离架构
3. 使用 TypeScript 增强 JavaScript 代码

#### 9.3.4 API 扩展和文档

增强 API 功能：
1. 实现完整的 RESTful API
2. 添加 API 版本控制
3. 使用 Swagger/OpenAPI 生成 API 文档
4. 实现 API 认证（JWT 或 OAuth）

#### 9.3.5 测试和 CI/CD

添加测试和持续集成：
1. 单元测试（使用 xUnit 或 NUnit）
2. 集成测试
3. 设置 CI/CD 管道（GitHub Actions 或 Azure DevOps）
4. 实现自动化部署

### 9.4 学习资源推荐

#### 9.4.1 官方文档

- [ASP.NET Core 文档](https://docs.microsoft.com/zh-cn/aspnet/core/)
- [Entity Framework Core 文档](https://docs.microsoft.com/zh-cn/ef/core/)
- [Docker 文档](https://docs.docker.com/)

#### 9.4.2 在线课程和教程

- Microsoft Learn：ASP.NET Core 路径
- Pluralsight：ASP.NET Core 课程
- Udemy：.NET Core MVC 和 Entity Framework Core 课程

#### 9.4.3 书籍

- 《ASP.NET Core in Action》
- 《Entity Framework Core in Action》
- 《Pro ASP.NET Core MVC》

## 10. 总结

TodoMvcApp 是一个基于 ASP.NET Core MVC 和 Entity Framework Core 的待办事项管理系统，它展示了一个完整的 Web 应用程序开发流程。通过学习这个项目，你已经了解了：

1. **ASP.NET Core MVC 架构**：模型-视图-控制器模式的实现
2. **Entity Framework Core**：对象关系映射和数据库交互
3. **Razor 视图引擎**：在 HTML 中嵌入 C# 代码
4. **前端技术**：Bootstrap、jQuery 和自定义 JavaScript
5. **RESTful API**：创建和使用 Web API
6. **Docker 容器化**：应用程序的打包和部署

这些知识和技能为你进一步学习和开发 .NET Web 应用程序奠定了坚实的基础。通过实践和扩展本项目，你可以不断提升自己的开发能力，成为一名熟练的 .NET 开发者。

祝你学习愉快，编程成功！ 