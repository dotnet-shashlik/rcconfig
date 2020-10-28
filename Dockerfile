FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY ./dist .
EXPOSE 80
EXPOSE 8080
ENTRYPOINT ["dotnet", "Shashlik.RC.dll"]
