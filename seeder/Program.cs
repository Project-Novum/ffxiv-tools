// See https://aka.ms/new-console-template for more information

using System.Net;
using MonoTorrent.Client;
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

    services.AddHostedService<Server>();
});

var app = builder.Build();

await app.RunAsync();