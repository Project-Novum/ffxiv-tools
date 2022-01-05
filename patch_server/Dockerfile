FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["patch_server/patch_server.csproj", "patch_server/"]
COPY ["third-party/monotorrent/src/MonoTorrent.Client/MonoTorrent.Client.csproj", "MonoTorrent.Client/"]
COPY ["third-party/monotorrent/src/MonoTorrent.BEncoding/MonoTorrent.BEncoding.csproj", "MonoTorrent.BEncoding/"]
COPY ["third-party/monotorrent/src/MonoTorrent.Factories/MonoTorrent.Factories.csproj", "MonoTorrent.Factories/"]
COPY ["third-party/monotorrent/src/MonoTorrent.Connections/MonoTorrent.Connections.csproj", "MonoTorrent.Connections/"]
COPY ["third-party/monotorrent/src/MonoTorrent.Messages/MonoTorrent.Messages.csproj", "MonoTorrent.Messages/"]
COPY ["third-party/monotorrent/src/MonoTorrent/MonoTorrent.csproj", "MonoTorrent/"]
COPY ["third-party/monotorrent/src/MonoTorrent.Dht/MonoTorrent.Dht.csproj", "MonoTorrent.Dht/"]
COPY ["third-party/monotorrent/src/MonoTorrent.PiecePicking/MonoTorrent.PiecePicking.csproj", "MonoTorrent.PiecePicking/"]
COPY ["third-party/monotorrent/src/MonoTorrent.PieceWriter/MonoTorrent.PieceWriter.csproj", "MonoTorrent.PieceWriter/"]
COPY ["third-party/monotorrent/src/MonoTorrent.PortForwarding/MonoTorrent.PortForwarding.csproj", "MonoTorrent.PortForwarding/"]
COPY ["third-party/monotorrent/src/MonoTorrent.Trackers/MonoTorrent.Trackers.csproj", "MonoTorrent.Trackers/"]
RUN dotnet restore "patch_server/patch_server.csproj"
COPY . .
WORKDIR "/src/patch_server"
RUN dotnet build "patch_server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "patch_server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "patch_server.dll"]
