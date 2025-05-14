using Microsoft.EntityFrameworkCore;

namespace TodoMvcApp.Models
{
    /// <summary>
    /// 数据库上下文类 - Entity Framework Core 的核心组件
    /// 负责与数据库的连接和交互
    /// </summary>
    public class TodoDbContext : DbContext
    {
        /// <summary>
        /// 构造函数 - 接收数据库配置选项
        /// </summary>
        /// <param name="options">数据库配置选项</param>
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
            // 这里不需要额外代码，基类构造函数会处理配置
        }

        /// <summary>
        /// Todo 实体的 DbSet - 代表数据库中的 Todos 表
        /// 通过这个属性可以查询和修改 Todo 数据
        /// </summary>
        public DbSet<Todo> Todos { get; set; }

        /// <summary>
        /// 模型创建配置
        /// 可以在这里添加额外的模型配置，如索引、关系等
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 示例：为 Todo 表添加一些种子数据
            modelBuilder.Entity<Todo>().HasData(
                new Todo
                {
                    Id = 1,
                    Title = "学习 ASP.NET Core MVC",
                    Description = "了解 MVC 模式和 ASP.NET Core 框架的基础知识",
                    IsComplete = false
                },
                new Todo
                {
                    Id = 2,
                    Title = "学习 Entity Framework Core",
                    Description = "了解如何使用 EF Core 进行数据访问",
                    IsComplete = false
                },
                new Todo
                {
                    Id = 3,
                    Title = "构建一个完整的 Web 应用",
                    Description = "应用所学知识构建一个实际的 Web 应用",
                    IsComplete = false
                }
            );
        }
    }
} 