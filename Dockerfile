# Используем .NET SDK для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем .csproj из папки ConsoleApp1
COPY ConsoleApp1/ConsoleApp1.csproj ConsoleApp1/
WORKDIR /app/ConsoleApp1
RUN dotnet restore

# Копируем весь код и компилируем
COPY . .
RUN dotnet publish -c Release -o out

# Используем легкий .NET runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/ConsoleApp1/out .

# Открываем порт 8080 для внешнего доступа
EXPOSE 8080

# Запускаем бота
CMD ["dotnet", "ConsoleApp1.dll"]
