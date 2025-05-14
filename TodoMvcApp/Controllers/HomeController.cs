using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoMvcApp.Models;

namespace TodoMvcApp.Controllers
{
    /// <summary>
    /// 主页控制器 - 处理网站主页和其他基本页面
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// 显示网站主页
        /// GET: /Home/Index 或 /
        /// </summary>
        /// <returns>主页视图</returns>
        public IActionResult Index()
        {
            // 返回主页视图
            return View();
        }

        /// <summary>
        /// 显示隐私政策页面
        /// GET: /Home/Privacy
        /// </summary>
        /// <returns>隐私政策视图</returns>
        public IActionResult Privacy()
        {
            // 返回隐私政策视图
            return View();
        }

        /// <summary>
        /// 显示关于页面
        /// GET: /Home/About
        /// </summary>
        /// <returns>关于页面视图</returns>
        public IActionResult About()
        {
            // 返回关于页面视图
            return View();
        }

        /// <summary>
        /// 处理错误并显示错误页面
        /// GET: /Home/Error
        /// </summary>
        /// <returns>错误页面视图</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // 创建错误视图模型，包含请求ID
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// 错误视图模型 - 用于错误页面显示
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// 请求ID，用于跟踪错误
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// 指示是否显示请求ID
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
} 