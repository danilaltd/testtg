# Используем .NET SDK для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем .csproj и восстанавливаем зависимости
COPY ConsoleApp1/ConsoleApp1.csproj ConsoleApp1/
WORKDIR /app/ConsoleApp1
RUN dotnet restore ConsoleApp1.csproj

# Копируем весь проект
COPY ConsoleApp1/ .

# Компилируем проект
RUN dotnet publish ConsoleApp1.csproj -c Release -o /app/out

# Используем легкий .NET runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Render.com ожидает, что приложение слушает порт, указанный в переменной окружения PORT
# EXPOSE не обязателен, так как Render.com использует PORT, но оставим для документации
EXPOSE 8080

# Запускаем приложение
CMD ["dotnet", "ConsoleApp1.dll"]