#!/bin/bash
# 开发环境启动脚本

# 停止并移除现有容器
docker-compose down

# 使用override文件重新构建并启动容器
docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build

# 使用Ctrl+C停止 