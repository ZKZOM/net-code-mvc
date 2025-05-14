// 网站全局 JavaScript 文件

// 等待文档加载完成
document.addEventListener('DOMContentLoaded', function () {
    // 初始化工具提示
    initializeTooltips();
    
    // 添加表单验证样式
    setupFormValidation();
    
    // 设置待办事项状态切换动画
    setupTodoToggleAnimation();
});

/**
 * 初始化 Bootstrap 工具提示
 */
function initializeTooltips() {
    // 检查是否有 Bootstrap 的 tooltip 函数
    if (typeof bootstrap !== 'undefined' && typeof bootstrap.Tooltip !== 'undefined') {
        // 初始化所有带有 data-bs-toggle="tooltip" 属性的元素
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

/**
 * 设置表单验证样式
 */
function setupFormValidation() {
    // 为所有表单添加提交事件监听器
    var forms = document.querySelectorAll('form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function (event) {
            // 如果表单验证失败，阻止提交
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            
            // 添加验证样式类
            form.classList.add('was-validated');
        });
    });
}

/**
 * 设置待办事项状态切换动画
 */
function setupTodoToggleAnimation() {
    // 使用事件委托监听待办事项完成状态切换
    document.addEventListener('click', function (event) {
        // 检查点击的元素是否是待办事项完成状态切换按钮
        if (event.target.matches('.todo-toggle-btn') || event.target.closest('.todo-toggle-btn')) {
            // 找到待办事项卡片
            var todoItem = event.target.closest('.todo-item');
            if (todoItem) {
                // 添加动画类
                todoItem.classList.add('animate__animated', 'animate__pulse');
                
                // 动画结束后移除动画类
                todoItem.addEventListener('animationend', function () {
                    todoItem.classList.remove('animate__animated', 'animate__pulse');
                }, { once: true });
            }
        }
    });
}

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