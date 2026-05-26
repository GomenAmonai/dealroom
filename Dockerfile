FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY DealRoom.sln global.json ./
COPY src/DealRoom.Core/DealRoom.Core.csproj src/DealRoom.Core/
COPY src/DealRoom.Infrastructure/DealRoom.Infrastructure.csproj src/DealRoom.Infrastructure/
COPY src/DealRoom.Api/DealRoom.Api.csproj src/DealRoom.Api/
COPY tests/DealRoom.Tests/DealRoom.Tests.csproj tests/DealRoom.Tests/
RUN dotnet restore

COPY . .
RUN dotnet publish src/DealRoom.Api/DealRoom.Api.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "DealRoom.Api.dll"]
