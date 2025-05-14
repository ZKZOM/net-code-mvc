using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TodoMvcApp.Models;

// 这是 ASP.NET Core 应用程序的入口点
// 在 .NET 6+ 中，Program.cs 使用了简化的顶级语句语法

// 创建 Web 应用程序构建器
var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
// 配置 MVC 服务，添加Razor运行时编译支持
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// 配置数据库上下文
// 使用 SQL Server 作为数据库
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 构建应用程序
var app = builder.Build();

// 确保数据库已创建并应用迁移 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<TodoDbContext>();
        context.Database.EnsureCreated();
        logger.LogInformation("数据库已成功创建或确认存在");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "初始化数据库时发生错误");
    }
}

// 配置 HTTP 请求管道
// 根据环境配置不同的中间件
if (app.Environment.IsDevelopment())
{
    // 在开发环境中显示详细错误信息
    app.UseDeveloperExceptionPage();
}
else
{
    // 在生产环境中使用错误处理页面
    app.UseExceptionHandler("/Home/Error");
    // 启用 HTTPS 重定向
    app.UseHsts();
}

// 启用 HTTPS 重定向
app.UseHttpsRedirection();
// 启用静态文件服务
app.UseStaticFiles();

// 启用路由
app.UseRouting();

// 启用授权（如果需要身份验证）
app.UseAuthorization();

// 配置默认路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 运行应用程序
app.Run(); 