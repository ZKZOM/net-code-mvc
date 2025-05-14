# TodoMvcApp 学习文档 - 第4部分

## 7. 前端与后端交互

### 7.1 表单提交和处理

在 TodoMvcApp 中，表单是用户与后端交互的主要方式之一。以创建待办事项为例：

#### 7.1.1 前端表单（Create.cshtml）

```html
@model TodoMvcApp.Models.Todo

@{
    ViewData["Title"] = "创建待办事项";
}

<h1>创建待办事项</h1>

<div class="row">
    <div class="col-md-6">
        <form asp-action="Create">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            
            <div class="form-group mb-3">
                <label asp-for="Title" class="control-label">标题</label>
                <input asp-for="Title" class="form-control" />
                <span asp-validation-for="Title" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="Description" class="control-label">描述</label>
                <textarea asp-for="Description" class="form-control" rows="3"></textarea>
                <span asp-validation-for="Description" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="DueDate" class="control-label">截止日期</label>
                <input asp-for="DueDate" class="form-control" type="date" />
                <span asp-validation-for="DueDate" class="text-danger"></span>
            </div>
            
            <div class="form-group mt-4">
                <button type="submit" class="btn btn-primary">
                    <i class="bi bi-save"></i> 保存
                </button>
                <a asp-action="Index" class="btn btn-secondary">
                    <i class="bi bi-arrow-left"></i> 返回列表
                </a>
            </div>
        </form>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

表单处理流程：
1. 用户填写表单并点击提交按钮
2. 表单数据通过 HTTP POST 请求发送到服务器
3. 控制器的 `Create` 方法接收并处理数据
4. 如果验证通过，数据保存到数据库；否则，返回表单并显示错误信息

### 7.2 AJAX 请求和 API 调用

TodoMvcApp 还提供了 RESTful API，允许使用 AJAX 进行前端与后端的交互。这在不刷新整个页面的情况下更新数据特别有用。

#### 7.2.1 API 控制器（TodoApiController.cs）

```csharp
[Route("api/[controller]")]
[ApiController]
public class TodoApiController : ControllerBase
{
    private readonly TodoDbContext _context;

    public TodoApiController(TodoDbContext context)
    {
        _context = context;
    }

    // GET: api/todoapi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        return await _context.Todos.OrderByDescending(t => t.CreatedDate).ToListAsync();
    }

    // PATCH: api/todoapi/5/toggle
    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult<Todo>> ToggleComplete(int id)
    {
        var todo = await _context.Todos.FindAsync(id);
        
        if (todo == null)
        {
            return NotFound();
        }
        
        todo.IsComplete = !todo.IsComplete;
        await _context.SaveChangesAsync();
        
        return todo;
    }

    // 其他API方法...
}
```

#### 7.2.2 前端 AJAX 调用（site.js）

```javascript
/**
 * 使用 AJAX 切换待办事项完成状态
 * @param {number} id - 待办事项ID
 */
function toggleTodoStatus(id) {
    // 检查是否有 jQuery
    if (typeof $ !== 'undefined' && typeof $.ajax !== 'undefined') {
        $.ajax({
            url: '/api/todoapi/' + id + '/toggle',
            type: 'PATCH',
            success: function (data) {
                // 更新 UI
                var todoItem = document.querySelector('.todo-item[data-id="' + id + '"]');
                if (todoItem) {
                    if (data.isComplete) {
                        todoItem.classList.remove('active');
                        todoItem.classList.add('completed');
                    } else {
                        todoItem.classList.remove('completed');
                        todoItem.classList.add('active');
                    }
                    
                    // 更新复选框状态
                    var checkbox = todoItem.querySelector('.todo-checkbox');
                    if (checkbox) {
                        checkbox.checked = data.isComplete;
                    }
                    
                    // 更新标题样式
                    var title = todoItem.querySelector('.todo-title');
                    if (title) {
                        if (data.isComplete) {
                            title.classList.add('text-decoration-line-through');
                        } else {
                            title.classList.remove('text-decoration-line-through');
                        }
                    }
                }
            },
            error: function (error) {
                console.error('切换待办事项状态失败:', error);
            }
        });
    }
}
```

AJAX 交互流程：
1. 用户点击待办事项的完成状态按钮
2. JavaScript 函数发送 AJAX 请求到 API
3. API 控制器处理请求并更新数据库
4. 控制器返回更新后的数据
5. JavaScript 根据返回的数据更新 UI

### 7.3 JavaScript 功能和交互

TodoMvcApp 使用 JavaScript 增强用户体验，例如实现待办事项的过滤和搜索功能：

```javascript
$(document).ready(function() {
    // 过滤按钮点击事件
    $('.filter-btn').click(function() {
        const filter = $(this).data('filter');
        
        // 更新按钮样式
        $('.filter-btn').removeClass('active');
        $(this).addClass('active');
        
        // 过滤待办事项
        if (filter === 'all') {
            $('.todo-item').show();
        } else {
            $('.todo-item').hide();
            $('.todo-item.' + filter).show();
        }
    });
    
    // 搜索功能
    $('#searchButton').click(function() {
        const searchText = $('#searchInput').val().toLowerCase();
        
        $('.todo-item').each(function() {
            const title = $(this).find('h5').text().toLowerCase();
            const description = $(this).find('p').text().toLowerCase();
            
            if (title.includes(searchText) || description.includes(searchText)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
});
```

## 8. Docker 容器化部署

### 8.1 Docker 基础概念

Docker 是一个开源的容器化平台，它可以将应用程序及其依赖项打包到一个可移植的容器中，确保在任何环境中都能一致地运行。

核心概念：
- **容器（Container）**：轻量级、可执行的软件包，包含运行应用程序所需的一切
- **镜像（Image）**：容器的只读模板，用于创建容器
- **Dockerfile**：构建 Docker 镜像的脚本
- **Docker Compose**：定义和运行多容器 Docker 应用程序的工具

### 8.2 TodoMvcApp 的 Docker 配置

#### 8.2.1 Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TodoMvcApp/TodoMvcApp.csproj", "TodoMvcApp/"]
RUN dotnet restore "TodoMvcApp/TodoMvcApp.csproj"
COPY . .
WORKDIR "/src/TodoMvcApp"
RUN dotnet build "TodoMvcApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoMvcApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# 显式复制视图和静态文件
COPY --from=build /src/TodoMvcApp/Views /app/Views
COPY --from=build /src/TodoMvcApp/wwwroot /app/wwwroot

# 添加开发环境热重载支持
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation

ENTRYPOINT ["dotnet", "TodoMvcApp.dll"]
```

Dockerfile 解析：
1. 使用 .NET 8.0 SDK 镜像构建应用程序
2. 复制项目文件并还原依赖项
3. 构建和发布应用程序
4. 使用 .NET 8.0 运行时镜像作为最终镜像
5. 复制发布的应用程序文件到最终镜像
6. 显式复制视图和静态文件（解决容器中缺少这些文件的问题）
7. 设置环境变量和入口点

#### 8.2.2 docker-compose.yml

```yaml
version: '3.8'

services:
  web:
    build:
      context: .
      dockerfile: TodoMvcApp/Dockerfile
    ports:
      - "8080:80"
      - "8081:443"
    depends_on:
      - db
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:80
      - DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true
      - ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
    networks:
      - todo-network
    restart: always
    volumes:
      - ./TodoMvcApp:/app/src
      - ./TodoMvcApp/Views:/app/Views
      - ./TodoMvcApp/wwwroot:/app/wwwroot

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_Strong_Password123!
      - MSSQL_PID=Express
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - todo-network
    restart: always

networks:
  todo-network:
    driver: bridge

volumes:
  sqlserver-data:
```

docker-compose.yml 解析：
1. 定义两个服务：web（ASP.NET Core 应用）和 db（SQL Server 数据库）
2. 配置端口映射、环境变量和网络
3. 设置卷挂载，确保视图和静态文件可用
4. 配置数据库的持久化存储

### 8.3 容器配置和网络

TodoMvcApp 使用 Docker Compose 创建一个包含 Web 应用和数据库的网络：

1. **网络配置**：
   - 创建名为 `todo-network` 的桥接网络
   - Web 和数据库容器都连接到这个网络
   - 容器可以通过服务名相互访问（例如，Web 应用通过 `db` 主机名访问数据库）

2. **端口映射**：
   - Web 应用：8080 -> 80（HTTP）和 8081 -> 443（HTTPS）
   - 数据库：1433 -> 1433（SQL Server）

### 8.4 数据持久化

Docker 容器本身是临时的，容器重启后数据会丢失。为了保持数据的持久性，TodoMvcApp 使用 Docker 卷：

```yaml
volumes:
  sqlserver-data:
```

这个卷挂载到 SQL Server 容器的数据目录：

```yaml
volumes:
  - sqlserver-data:/var/opt/mssql
```

这确保了即使容器重启或重建，数据库数据也不会丢失。 