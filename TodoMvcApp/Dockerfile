FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TodoMvcApp/TodoMvcApp.csproj", "TodoMvcApp/"]
RUN dotnet restore "TodoMvcApp/TodoMvcApp.csproj"
COPY . .
WORKDIR "/src/TodoMvcApp"
RUN dotnet build "TodoMvcApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoMvcApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# 显式复制视图和静态文件
COPY --from=build /src/TodoMvcApp/Views /app/Views
COPY --from=build /src/TodoMvcApp/wwwroot /app/wwwroot

# 添加开发环境热重载支持
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_HOSTINGSTARTUPASSEMBLIES=Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation

ENTRYPOINT ["dotnet", "TodoMvcApp.dll"] 