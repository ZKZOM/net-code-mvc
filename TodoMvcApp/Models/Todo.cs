using System;
using System.ComponentModel.DataAnnotations;

namespace TodoMvcApp.Models
{
    /// <summary>
    /// Todo 模型类 - 表示一个待办事项
    /// 这个类将被映射到数据库中的一个表
    /// </summary>
    public class Todo
    {
        /// <summary>
        /// 待办事项的唯一标识符
        /// 这个属性被标记为主键，数据库会自动为其生成值
        /// </summary>
        [Key] // 数据注解，标记这个属性为主键
        public int Id { get; set; }

        /// <summary>
        /// 待办事项的标题
        /// 使用 Required 特性确保该字段不能为空
        /// </summary>
        [Required(ErrorMessage = "标题是必填项")] // 数据验证，确保用户输入值
        [StringLength(100, ErrorMessage = "标题不能超过100个字符")] // 限制字符串长度
        public string Title { get; set; }

        /// <summary>
        /// 待办事项的详细描述（可选）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 待办事项是否已完成的标志
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// 待办事项的创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now; // 默认值为当前时间

        /// <summary>
        /// 待办事项的截止日期（可选）
        /// </summary>
        [Display(Name = "截止日期")] // 显示名称，用于表单标签
        [DataType(DataType.Date)] // 指定这是一个日期类型，影响UI渲染
        public DateTime? DueDate { get; set; } // 问号表示这是一个可空类型
    }
} 