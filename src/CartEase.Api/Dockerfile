# NuGet Restore Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS nuget-restore
WORKDIR /app
COPY ["CartEase.sln", "./"]
COPY ["src/CartEase.Api/CartEase.Api.csproj", "src/CartEase.Api/"]
COPY ["src/CartEase.Application/CartEase.Application.csproj", "src/CartEase.Application/"]
COPY ["src/CartEase.Core/CartEase.Core.csproj", "src/CartEase.Core/"]
COPY ["src/CartEase.Infrastructure/CartEase.Infrastructure.csproj", "src/CartEase.Infrastructure/"]
COPY ["src/CartEase.Models/CartEase.Models.csproj", "src/CartEase.Models/"]
COPY ["src/CartEase.Tests/CartEase.Tests.csproj", "src/CartEase.Tests/"]

RUN dotnet restore "./CartEase.sln"

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY --from=nuget-restore /app/obj/ ./obj/
COPY . .
WORKDIR "/app/src/CartEase.Api"
RUN dotnet build "CartEase.Api.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "CartEase.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartEase.Api.dll"]
