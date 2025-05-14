#!/bin/bash

# 构建和启动 Docker 容器
docker-compose up --build -d

echo "应用程序正在启动..."
echo "Web 应用程序地址: http://localhost:8080"
echo "SQL Server 地址: localhost,1433"
echo "SQL Server 用户名: sa"
echo "SQL Server 密码: Your_Strong_Password123!" 