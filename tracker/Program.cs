// See https://aka.ms/new-console-template for more information

using System.Text;
using MonoTorrent;
using MonoTorrent.BEncoding;
using MonoTorrent.Connections.TrackerServer;
using MonoTorrent.TorrentWatcher;
using MonoTorrent.TrackerServer;
using tracker;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddSingleton<ITrackerListener>((services) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        return TrackerListenerFactory.CreateHttp($"http://+:{configuration.GetValue("ServerPort", 54997)}/");
    });

    services.AddSingleton((services) =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();

            var tracker = new TrackerServer(new BEncodedString(CreateTrackerId()));
            tracker.RegisterListener(services.GetRequiredService<ITrackerListener>());
            tracker.AllowScrape = true;
            tracker.AllowUnregisteredTorrents = false;
            tracker.MinAnnounceInterval = TimeSpan.FromSeconds(30);
            
            return tracker;
        }
    );

    services.AddSingleton<ITorrentWatcher>((services) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();
        return new TorrentFolderWatcher(configuration.GetValue("PatchData", "./"), "*.torrent");
    });
    
    services.AddHostedService<Server>();
});

var app = builder.Build();

await app.RunAsync();


string CreateTrackerId()
{
    var peerIdRandomGenerator = new Random(DateTimeOffset.UnixEpoch.Millisecond);
    
    var sb = new StringBuilder (20);
    sb.Append ("SQ0001-");
    
    while (sb.Length < 20)
        sb.Append (peerIdRandomGenerator.Next (0, 9));

    return sb.ToString();
}