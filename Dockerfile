FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY ./dist .
ENTRYPOINT ["dotnet", "Shashlik.RC.dll"]
