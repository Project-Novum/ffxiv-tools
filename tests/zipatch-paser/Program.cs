// See https://aka.ms/new-console-template for more information

using System.Security.Cryptography;
using System.Text;
using ZiPatch;
using ZiPatch.Chunks;



var _gamePath = "D:\\dev\\ffxiv-tools\\torrents\\extracted";
var sb = new StringBuilder();

foreach (var file in Directory.GetFiles("D:\\dev\\ffxiv-tools\\torrents\\PatchData\\ffxiv", "*.patch", SearchOption.AllDirectories))
{
    using var reader = new Reader(file);
    var chunks = await reader.GetChunksAsync();
    
    Console.WriteLine($"Extracting: {Path.GetFileName(file)} Chunks: {chunks.Count}");
    
    foreach (var chunk in chunks)
    {
        switch (chunk)
        {
            case Fhdr fhdr:
            {
                sb = sb.AppendLine($"fhdr: {fhdr}");
                break;
            }

            case Aply aply:
            {
                sb = sb.AppendLine($"aply: {aply}");
                break;
            }

            case Adir adir:
            {
                var path = Path.Join(_gamePath, adir.Path);
                sb = sb.AppendLine($"Adir: {adir}");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                break;
            }

            case Dled dled:
            {
                var path = Path.Join(_gamePath, dled.Path);

                sb = sb.AppendLine($"Dled: {dled}");
                if (!Directory.Exists(path))
                    Directory.Delete(path);
                break;
            }

            case Etry etry:
            {
                var path = Path.Join(_gamePath, etry.Path);
                sb = processEtry(path, etry, sb);
                break;
            }
        }
    }
}

File.WriteAllText("D:\\dev\\ffxiv-tools\\torrents\\extracted\\output.txt", sb.ToString());

StringBuilder processEtry(string path, Etry entry, StringBuilder sb)
{
    foreach (var item in entry.Chunks)
    {
        if (item.Mode == 'D' && File.Exists(path))
        {
            sb = sb.AppendLine($"Deleted: {entry.Path} Mode:{(char) item.Mode}");
            File.Delete(path);
            continue;
        }
        
        if (item.CompressedFileSize == 0) continue;

        if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

        using var fs = new FileStream(path, FileMode.OpenOrCreate);
        item.Save(fs);
        fs.Seek(0, SeekOrigin.Begin);

        var sHash = BitConverter.ToString(item.SourceFileHash).Replace("-", "").ToUpper();
        var dHash = BitConverter.ToString(item.DestFileHash).Replace("-", "").ToUpper();
        // var hash = BitConverter.ToString(cryptoProvider.ComputeHash(fs)).Replace("-", "").ToUpper();
       sb = sb.AppendLine(string.Format("Wrote: {0} Mode:{3} SrcHash: {1} DestHash: {2}", entry.Path, sHash, dHash,
            (char) item.Mode));
    }

    return sb;
}