# Discord bot: outbound gateway connection only, so the lightweight runtime
# image is enough (no ASP.NET, no exposed ports needed).
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DiscordBotNumber23/DiscordBotNumber23.csproj", "DiscordBotNumber23/"]
RUN dotnet restore "DiscordBotNumber23/DiscordBotNumber23.csproj"
COPY . .
WORKDIR "/src/DiscordBotNumber23"
RUN dotnet build "./DiscordBotNumber23.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DiscordBotNumber23.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBotNumber23.dll"]
