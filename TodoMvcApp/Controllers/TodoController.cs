using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoMvcApp.Models;

namespace TodoMvcApp.Controllers
{
    /// <summary>
    /// Todo 控制器 - 处理与待办事项相关的请求
    /// 控制器是 MVC 架构中的 "C"，负责处理用户输入并返回适当的响应
    /// </summary>
    public class TodoController : Controller
    {
        // 数据库上下文，通过依赖注入获得
        private readonly TodoDbContext _context;

        /// <summary>
        /// 构造函数 - 通过依赖注入接收数据库上下文
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public TodoController(TodoDbContext context)
        {
            _context = context; // 保存数据库上下文以便在其他方法中使用
        }

        /// <summary>
        /// 显示所有待办事项的列表
        /// GET: /Todo
        /// </summary>
        /// <returns>包含待办事项列表的视图</returns>
        public async Task<IActionResult> Index()
        {
            // 从数据库获取所有待办事项并按创建日期排序
            var todos = await _context.Todos.OrderByDescending(t => t.CreatedDate).ToListAsync();
            
            // 返回视图，并将待办事项列表作为模型传递给视图
            return View(todos);
        }

        /// <summary>
        /// 显示创建新待办事项的表单
        /// GET: /Todo/Create
        /// </summary>
        /// <returns>创建表单视图</returns>
        public IActionResult Create()
        {
            // 返回空表单视图
            return View();
        }

        /// <summary>
        /// 处理创建新待办事项的表单提交
        /// POST: /Todo/Create
        /// </summary>
        /// <param name="todo">从表单提交的待办事项数据</param>
        /// <returns>成功则重定向到索引页，失败则返回表单</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // 防止跨站请求伪造攻击
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate")] Todo todo)
        {
            // 检查模型验证是否通过（例如必填字段）
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

        /// <summary>
        /// 显示编辑待办事项的表单
        /// GET: /Todo/Edit/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>编辑表单视图</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // 检查ID是否提供
            if (id == null)
            {
                return NotFound(); // 返回404错误
            }

            // 查找指定ID的待办事项
            var todo = await _context.Todos.FindAsync(id);
            
            // 如果找不到，返回404错误
            if (todo == null)
            {
                return NotFound();
            }
            
            // 返回编辑表单视图，并传入待办事项数据
            return View(todo);
        }

        /// <summary>
        /// 处理编辑待办事项的表单提交
        /// POST: /Todo/Edit/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <param name="todo">从表单提交的待办事项数据</param>
        /// <returns>成功则重定向到索引页，失败则返回表单</returns>
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
                    // 处理并发冲突（多个用户同时编辑）
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
            
            // 如果验证失败，返回表单视图，保留用户输入的数据
            return View(todo);
        }

        /// <summary>
        /// 显示删除待办事项的确认页面
        /// GET: /Todo/Delete/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>删除确认视图</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // 检查ID是否提供
            if (id == null)
            {
                return NotFound();
            }

            // 查找指定ID的待办事项，包括详细信息
            var todo = await _context.Todos
                .FirstOrDefaultAsync(m => m.Id == id);
                
            // 如果找不到，返回404错误
            if (todo == null)
            {
                return NotFound();
            }

            // 返回删除确认视图，并传入待办事项数据
            return View(todo);
        }

        /// <summary>
        /// 处理删除待办事项的确认
        /// POST: /Todo/DeleteConfirmed/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>重定向到索引页</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 查找指定ID的待办事项
            var todo = await _context.Todos.FindAsync(id);
            
            // 从数据库中删除待办事项
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            
            // 删除成功后重定向到索引页
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// 切换待办事项的完成状态
        /// POST: /Todo/ToggleComplete/5
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>重定向到索引页</returns>
        [HttpPost]
        public async Task<IActionResult> ToggleComplete(int id)
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
            
            // 重定向到索引页
            return RedirectToAction(nameof(Index));
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