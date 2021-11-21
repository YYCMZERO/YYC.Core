#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Demo/Demo.csproj", "Demo/"]
COPY ["AntJoin.Log/AntJoin.Log.csproj", "AntJoin.Log/"]
COPY ["AntJoin.Core/AntJoin.Core.csproj", "AntJoin.Core/"]
COPY ["AntJoin.Redis/AntJoin.Redis.csproj", "AntJoin.Redis/"]
COPY ["AntJoin.MQ/AntJoin.MQ.csproj", "AntJoin.MQ/"]
COPY ["Demo.EventHandlers/Demo.EventHandlers.csproj", "Demo.EventHandlers/"]
RUN dotnet restore "Demo/Demo.csproj"
COPY . .
WORKDIR "/src/Demo"
RUN dotnet build "Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.dll"]