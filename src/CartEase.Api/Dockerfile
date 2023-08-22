#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/CartEase.Api/CartEase.Api.csproj", "src/CartEase.Api/"]
COPY ["src/CartEase.Application/CartEase.Application.csproj", "src/CartEase.Application/"]
COPY ["src/CartEase.Core/CartEase.Core.csproj", "src/CartEase.Core/"]
COPY ["src/CartEase.Infrastructure/CartEase.Infrastructure.csproj", "src/CartEase.Infrastructure/"]
COPY ["src/CartEase.Models/CartEase.Models.csproj", "src/CartEase.Models/"]
RUN dotnet restore "src/CartEase.Api/CartEase.Api.csproj"
COPY . .
WORKDIR "/src/src/CartEase.Api"
RUN dotnet build "CartEase.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CartEase.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CartEase.Api.dll"]