FROM mcr.microsoft.com/dotnet/sdk:6.0 AS src
WORKDIR /src
COPY ["/src", "."]

RUN dotnet build "Minibank.Web/Minibank.Web.csproj" -c Release
RUN dotnet test "Minibank.Core.Tests/Minibank.Core.Tests.csproj" --no-build
RUN dotnet publish "Minibank.Web/Minibank.Web.csproj" -c Release --no-build -o /dist

#Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

COPY --from=src /dist . 
ENV ASPNETCORE_URLS=http://*:5000;http://*:5001
EXPOSE 5001 5000 

ENTRYPOINT ["dotnet", "Minibank.Web.dll"]