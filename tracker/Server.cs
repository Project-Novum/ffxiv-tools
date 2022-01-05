using MonoTorrent;
using MonoTorrent.Connections.TrackerServer;
using MonoTorrent.TorrentWatcher;
using MonoTorrent.TrackerServer;

namespace tracker;

public class Server : BackgroundService
{
    private readonly ILogger<Server> _logger;
    private readonly ITrackerListener _listener;
    private readonly TrackerServer _trackerServer;
    private readonly ITorrentWatcher _torrentWatcher;

    public Server(ILogger<Server> logger, ITrackerListener listener, TrackerServer trackerServer, ITorrentWatcher torrentWatcher)
    {
        _logger = logger;
        _listener = listener;
        _trackerServer = trackerServer;
        _torrentWatcher = torrentWatcher;

        _listener.AnnounceReceived += ListenerOnAnnounceReceived;
        _torrentWatcher.TorrentFound += TorrentWatcherOnTorrentFound;
        _torrentWatcher.TorrentLost += TorrentWatcherOnTorrentLost;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _torrentWatcher.Start();
        _torrentWatcher.ForceScan();

        await Task.Run(() => { _listener.Start(); }, stoppingToken);
    }

    private void ListenerOnAnnounceReceived(object? sender, AnnounceRequest e)
    {
        var f = _trackerServer.GetTrackerItems()
            .FirstOrDefault(x => x.Trackable.InfoHash.ToHex()[^8..] == e.InfoHash.ToHex()[^8..]);
        
        _logger.LogInformation($"AnnounceReceived: {e.InfoHash.ToHex()}:{f?.Trackable.InfoHash.ToHex() ?? "N/A"} -> {e.PeerId.Text}");
    }

    private void TorrentWatcherOnTorrentLost(object? sender, TorrentWatcherEventArgs e)
    {
    }

    private void TorrentWatcherOnTorrentFound(object? sender, TorrentWatcherEventArgs e)
    {
        var t = Torrent.Load(e.TorrentPath);
        _logger.LogInformation($"Found: {t.InfoHash.ToHex()} -> {e.TorrentPath.Split(Path.DirectorySeparatorChar).Last()}");

        lock (_trackerServer)
            _trackerServer.Add(new CustomITrackable(t));
    }
}