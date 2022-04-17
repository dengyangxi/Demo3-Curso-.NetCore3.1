#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Micro.Order.API.csproj", "./"]
RUN dotnet restore "./Micro.Order.API.csproj"
COPY . .
WORKDIR "/src/Micro.Order.API"
RUN dotnet build "Micro.Order.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Micro.Order.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Micro.Order.API.dll"]