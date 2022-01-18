// Licensed to the Rapture Contributors under one or more agreements.
// The Rapture Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using MonoTorrent.Client;
using MonoTorrent.TorrentWatcher;
using seeder;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddSingleton((services) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var settingsBuilder = new EngineSettingsBuilder()
        {
            AllowLocalPeerDiscovery = false,
            AllowPortForwarding = false,
            AutoSaveLoadDhtCache = false,
            AutoSaveLoadFastResume = false,
            AutoSaveLoadMagnetLinkMetadata = false,
            DhtEndPoint = null,
            ListenEndPoint = new IPEndPoint(IPAddress.Any, configuration.GetValue<int>("ServerPort")),
        };

        return new ClientEngine(settingsBuilder.ToSettings());
    });

    services.AddSingleton<ITorrentWatcher>((services) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        return new TorrentFolderWatcher(configuration.GetValue("PatchData", "./"), "*.torrent");
    });
    
    services.AddHostedService<Server>();
});

var app = builder.Build();

await app.RunAsync();
