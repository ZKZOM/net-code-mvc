# Windows PowerShell 重启开发环境脚本

# 停止容器
docker-compose stop web

# 重新启动容器
docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build -d web

# 显示日志
docker-compose logs -f web 