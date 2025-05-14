# TodoMvcApp 学习文档 - 第3部分

## 5. 数据流程和请求生命周期

### 5.1 HTTP 请求处理流程

当用户在浏览器中访问 TodoMvcApp 应用时，请求的处理流程如下：

1. **请求发送**：浏览器发送 HTTP 请求到服务器
2. **中间件处理**：请求通过一系列中间件（如路由、静态文件、认证等）
3. **路由匹配**：路由系统确定处理请求的控制器和操作方法
4. **控制器执行**：执行对应的控制器操作方法
5. **数据库交互**：控制器通过 Entity Framework Core 与数据库交互
6. **视图渲染**：控制器选择视图并传递模型数据
7. **响应返回**：渲染后的 HTML 返回给浏览器

### 5.2 示例：创建待办事项的完整流程

以创建新待办事项为例，完整的数据流程如下：

1. **用户操作**：
   - 用户访问 `/Todo/Create` 页面
   - 填写表单并点击提交按钮

2. **前端处理**：
   - 表单通过 HTTP POST 请求提交到服务器
   - 请求包含表单数据（标题、描述、截止日期等）

3. **后端处理**：
   - 路由系统将请求路由到 `TodoController` 的 `Create` 方法
   - 控制器验证表单数据
   - 创建新的 `Todo` 对象
   - 将对象添加到数据库
   - 重定向用户到待办事项列表页面

4. **数据库操作**：
   - Entity Framework Core 将 `Todo` 对象转换为 SQL INSERT 语句
   - SQL Server 执行插入操作
   - 返回新创建记录的 ID

### 5.3 异步操作和任务处理

ASP.NET Core 大量使用异步编程模型，这有助于提高应用程序的可伸缩性和性能。在 TodoMvcApp 中，大多数数据库操作都是异步执行的：

```csharp
// 异步方法使用 async 关键字标记
public async Task<IActionResult> Index()
{
    // 异步操作使用 await 关键字
    var todos = await _context.Todos.OrderByDescending(t => t.CreatedDate).ToListAsync();
    return View(todos);
}
```

异步编程的优势：
- 提高服务器资源利用率
- 避免线程阻塞
- 增强应用程序的响应能力
- 支持更多并发连接

## 6. 关键代码解析

### 6.1 模型定义和验证

`Todo.cs` 模型类定义了待办事项的数据结构和验证规则：

```csharp
public class Todo
{
    [Key] // 标记为主键
    public int Id { get; set; }

    [Required(ErrorMessage = "标题是必填项")] // 验证规则：必填
    [StringLength(100, ErrorMessage = "标题不能超过100个字符")] // 验证规则：最大长度
    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsComplete { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now; // 默认值

    [Display(Name = "截止日期")] // 显示名称
    [DataType(DataType.Date)] // 数据类型
    public DateTime? DueDate { get; set; } // 可空类型
}
```

数据注解的作用：
- `[Key]`：标识属性为主键
- `[Required]`：指定属性为必填项
- `[StringLength]`：限制字符串长度
- `[Display]`：设置显示名称
- `[DataType]`：指定数据类型

### 6.2 控制器操作和业务逻辑

`TodoController.cs` 包含处理用户请求的操作方法：

#### 6.2.1 显示待办事项列表

```csharp
public async Task<IActionResult> Index()
{
    // 从数据库获取所有待办事项并按创建日期排序
    var todos = await _context.Todos.OrderByDescending(t => t.CreatedDate).ToListAsync();
    
    // 返回视图，并将待办事项列表作为模型传递给视图
    return View(todos);
}
```

#### 6.2.2 创建新待办事项

```csharp
[HttpPost]
[ValidateAntiForgeryToken] // 防止跨站请求伪造攻击
public async Task<IActionResult> Create([Bind("Title,Description,DueDate")] Todo todo)
{
    // 检查模型验证是否通过
    if (ModelState.IsValid)
    {
        // 设置创建日期为当前时间
        todo.CreatedDate = DateTime.Now;
        
        // 将新待办事项添加到数据库
        _context.Add(todo);
        await _context.SaveChangesAsync();
        
        // 添加成功后重定向到索引页
        return RedirectToAction(nameof(Index));
    }
    
    // 如果验证失败，返回表单视图，保留用户输入的数据
    return View(todo);
}
```

#### 6.2.3 编辑待办事项

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsComplete,DueDate")] Todo todo)
{
    // 检查ID是否匹配
    if (id != todo.Id)
    {
        return NotFound();
    }

    // 检查模型验证是否通过
    if (ModelState.IsValid)
    {
        try
        {
            // 保留原始创建日期
            var originalTodo = await _context.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            todo.CreatedDate = originalTodo.CreatedDate;
            
            // 更新数据库中的待办事项
            _context.Update(todo);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // 处理并发冲突
            if (!TodoExists(todo.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        
        // 更新成功后重定向到索引页
        return RedirectToAction(nameof(Index));
    }
    
    // 如果验证失败，返回表单视图
    return View(todo);
}
```

### 6.3 视图渲染和 Razor 语法

Razor 是 ASP.NET Core 的视图引擎，它允许在 HTML 中嵌入 C# 代码。以下是 `Todo/Index.cshtml` 视图的部分代码：

```html
@model IEnumerable<TodoMvcApp.Models.Todo>

@{
    ViewData["Title"] = "待办事项列表";
}

<div class="d-flex justify-content-between align-items-center mb-4">
    <h1>待办事项列表</h1>
    <a asp-action="Create" class="btn btn-primary">
        <i class="bi bi-plus-circle"></i> 新建待办事项
    </a>
</div>

<!-- 待办事项列表 -->
<div class="todo-list">
    @if (Model.Any())
    {
        @foreach (var item in Model)
        {
            <div class="card mb-3 todo-item @(item.IsComplete ? "completed" : "active")">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                        <div class="d-flex align-items-center">
                            <!-- 完成状态切换按钮 -->
                            <form asp-action="ToggleComplete" asp-route-id="@item.Id" method="post" class="me-3">
                                <button type="submit" class="btn btn-sm @(item.IsComplete ? "btn-success" : "btn-outline-success")">
                                    <i class="bi @(item.IsComplete ? "bi-check-circle-fill" : "bi-circle")"></i>
                                </button>
                            </form>
                            
                            <!-- 待办事项标题和描述 -->
                            <div>
                                <h5 class="mb-1 @(item.IsComplete ? "text-decoration-line-through" : "")">@item.Title</h5>
                                @if (!string.IsNullOrEmpty(item.Description))
                                {
                                    <p class="mb-1 text-muted">@item.Description</p>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        }
    }
    else
    {
        <div class="alert alert-info">
            <p class="mb-0">暂无待办事项。点击"新建待办事项"按钮创建一个新的待办事项。</p>
        </div>
    }
</div>
```

Razor 语法特点：
- `@` 符号用于转换到 C# 代码
- `@{ ... }` 用于多行 C# 代码块
- `@foreach`, `@if` 等用于控制流
- `@model` 指令定义视图的模型类型
- `@item.Title` 用于输出表达式的值

### 6.4 Tag Helpers

ASP.NET Core MVC 提供了 Tag Helpers，它们可以扩展 HTML 元素的功能：

```html
<!-- asp-controller 和 asp-action 指定控制器和操作方法 -->
<a asp-controller="Todo" asp-action="Create" class="btn btn-primary">新建待办事项</a>

<!-- asp-for 将表单字段绑定到模型属性 -->
<input asp-for="Title" class="form-control" />

<!-- asp-validation-for 显示特定字段的验证消息 -->
<span asp-validation-for="Title" class="text-danger"></span>
```

常用的 Tag Helpers：
- `asp-controller`, `asp-action`: 生成链接到控制器操作的 URL
- `asp-for`: 绑定表单元素到模型属性
- `asp-validation-for`: 显示模型验证错误
- `asp-items`: 为选择列表提供选项 