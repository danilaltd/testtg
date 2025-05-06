# Используем .NET SDK для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем только .csproj, чтобы сначала установить зависимости
COPY ConsoleApp1.csproj .
RUN dotnet restore

# Копируем весь код и компилируем
COPY . .
RUN dotnet publish -c Release -o out

# Используем легкий .NET runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Запускаем бота
CMD ["dotnet", "ConsoleApp1.dll"]
