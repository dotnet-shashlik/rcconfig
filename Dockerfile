FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY ./dist .
ENTRYPOINT ["dotnet", "Shashlik.RC.dll"]
