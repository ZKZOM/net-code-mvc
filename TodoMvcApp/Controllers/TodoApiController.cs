using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoMvcApp.Models;

namespace TodoMvcApp.Controllers
{
    /// <summary>
    /// Todo API 控制器 - 提供 RESTful API 接口
    /// 用于前端 JavaScript 与后端进行数据交互
    /// [Route("api/[controller]")] 特性定义了API的路由前缀为 "api/todoapi"
    /// </summary>
    [Route("api/[controller]")]
    [ApiController] // 标记为API控制器，启用API特定功能
    public class TodoApiController : ControllerBase // 注意这里使用ControllerBase而不是Controller，因为API不需要视图支持
    {
        private readonly TodoDbContext _context;

        /// <summary>
        /// 构造函数 - 通过依赖注入接收数据库上下文
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public TodoApiController(TodoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有待办事项
        /// GET: api/todoapi
        /// </summary>
        /// <returns>待办事项列表</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            // 从数据库获取所有待办事项并按创建日期排序
            return await _context.Todos.OrderByDescending(t => t.CreatedDate).ToListAsync();
        }

        /// <summary>
        /// 获取指定ID的待办事项
        /// GET: api/todoapi/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>找到的待办事项，或404错误</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            // 查找指定ID的待办事项
            var todo = await _context.Todos.FindAsync(id);

            // 如果找不到，返回404错误
            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        /// <summary>
        /// 更新指定ID的待办事项
        /// PUT: api/todoapi/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="todo">更新后的待办事项数据</param>
        /// <returns>无内容（成功）或错误</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo(int id, Todo todo)
        {
            // 检查ID是否匹配
            if (id != todo.Id)
            {
                return BadRequest();
            }

            // 标记实体为已修改
            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                // 保存更改到数据库
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // 处理并发冲突
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // 返回204 No Content状态码，表示成功但无返回内容
            return NoContent();
        }

        /// <summary>
        /// 创建新的待办事项
        /// POST: api/todoapi
        /// </summary>
        /// <param name="todo">新待办事项数据</param>
        /// <returns>创建的待办事项</returns>
        [HttpPost]
        public async Task<ActionResult<Todo>> PostTodo(Todo todo)
        {
            // 将新待办事项添加到数据库
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // 返回201 Created状态码，并包含新创建的资源
            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }

        /// <summary>
        /// 删除指定ID的待办事项
        /// DELETE: api/todoapi/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>无内容（成功）或错误</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            // 查找指定ID的待办事项
            var todo = await _context.Todos.FindAsync(id);
            
            // 如果找不到，返回404错误
            if (todo == null)
            {
                return NotFound();
            }

            // 从数据库中删除待办事项
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            // 返回204 No Content状态码，表示成功但无返回内容
            return NoContent();
        }

        /// <summary>
        /// 切换待办事项的完成状态
        /// PATCH: api/todoapi/5/toggle
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>更新后的待办事项</returns>
        [HttpPatch("{id}/toggle")]
        public async Task<ActionResult<Todo>> ToggleComplete(int id)
        {
            // 查找指定ID的待办事项
            var todo = await _context.Todos.FindAsync(id);
            
            // 如果找不到，返回404错误
            if (todo == null)
            {
                return NotFound();
            }
            
            // 切换完成状态
            todo.IsComplete = !todo.IsComplete;
            
            // 保存更改到数据库
            await _context.SaveChangesAsync();
            
            // 返回更新后的待办事项
            return todo;
        }

        /// <summary>
        /// 检查指定ID的待办事项是否存在
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>如果存在返回true，否则返回false</returns>
        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
} 