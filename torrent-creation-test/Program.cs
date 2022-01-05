// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using MonoTorrent;
using MonoTorrent.BEncoding;
using tracker;

/*
var stockHash = "D8CC5488E9847936232AF91AC31B56306F03E88D";
var encryptedHash = "B5EB8FD62545932C1118A3432CF36DF66F03E88D";

byte[] bytes = Encoding.Default.GetBytes(stockHash[^8..]);
string hexString = BitConverter.ToString(bytes);

var bf = new BlowFish(hexString.Replace("-", ""));

var ff = new Cryptographer("krMehjLVpOso");

var z = InfoHash.FromHex(stockHash).Span.ToArray();
z = z.Take(z.Length).ToArray();
ff.Decrypt(z);

Console.WriteLine($"d: {BitConverter.ToString(z).Replace("-", "").ToUpper()}");
Console.WriteLine($"s: {stockHash}");
Console.WriteLine($"e: {encryptedHash}");

var f = StringToByteArray(stockHash);
var c = StringToByteArray(encryptedHash);

Debugger.Break();

static byte[] StringToByteArray(string hex) {
    return Enumerable.Range(0, hex.Length)
        .Where(x => x % 2 == 0)
        .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        .ToArray();
}

public class Cryptographer
{
    private byte[] Keys { get; set; }

    public Cryptographer(string password)
    {
        Keys = Encoding.ASCII.GetBytes(password);
    }

    public void Encrypt(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte) (data[i] ^ Keys[i % Keys.Length]);
        }
    }

    public void Decrypt(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte) (Keys[i % Keys.Length] ^ data[i]);
        }
    }
}

/*
var c = new TorrentCreator();
c.Announce = "http://track01.ffxiv.com:54997/announce";
c.Name = "ffxiv";
c.PieceLength = 16 * 1024;
c.CreatedBy = null;
c.StroreCreationTime = false;
c.Encoding = null;

File.Delete("fuck.torrent");

await c.CreateAsync(new TorrentFileSource("D:\\dev\\ffxiv-tools\\torrents\\PatchData\\test")
{
    TorrentName = "ffxiv"
}, "fuck.torrent");
*/

foreach (var file in Directory.GetFiles("D:\\dev\\ffxiv-tools\\torrents\\PatchData\\ffxiv\\", "*.torrent",
             SearchOption.AllDirectories))
{
    // File.Copy(file, file.Replace(".bak", ""), true);
    File.Copy(file, file + ".bak", true);

    var tr = await Torrent.LoadAsync(file);
    
    var c = new TorrentCreator();
    c.Announce = "http://127.0.0.1:54997/announce";
    c.Name = "ffxiv";
    c.PieceLength = 16 * 16 * 1024;
    c.CreatedBy = null;
    c.StroreCreationTime = false;
    c.Encoding = null;

    await c.CreateAsync(new z
    {
        Files = tr.Files.Select(x => new FileMapping($"D:\\dev\\ffxiv-tools\\torrents\\PatchData\\ffxiv\\{x.Path}", x.Path)),
        TorrentName = "ffxiv"
    }, file);
}

class z : ITorrentFileSource
{
    public IEnumerable<FileMapping> Files { get; set; }
    public string TorrentName { get; set; }
}