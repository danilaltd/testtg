# Используем .NET SDK для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем .csproj и восстанавливаем зависимости
COPY ConsoleApp1/ConsoleApp1.csproj ConsoleApp1/
RUN dotnet restore ConsoleApp1/ConsoleApp1.csproj

# Копируем только папку проекта
COPY ConsoleApp1/ ConsoleApp1/

# Компилируем конкретный проект
RUN dotnet publish ConsoleApp1/ConsoleApp1.csproj -c Release -o out

# Используем легкий .NET runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Открываем порт 8080 для внешнего доступа
EXPOSE 8080


# Запускаем приложение
CMD ["dotnet", "ConsoleApp1.dll"]