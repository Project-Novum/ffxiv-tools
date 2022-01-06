// See https://aka.ms/new-console-template for more information
using ZiPatch;
using ZiPatch.Chunks;


var testPatchFile = "D:\\dev\\ffxiv-tools\\torrents\\PatchData\\ffxiv\\2d2a390f\\patch\\D2010.09.18.0000.patch";

using var reader = new Reader(testPatchFile);
var chunks = await reader.GetChunksAsync();

Console.WriteLine(chunks.Count(x => x.ChunkType == ChunkType.Etry));


foreach (var entry in chunks.OfType<Etry>().Where(x => x.Path.Contains("ffxivboot")))
{
    Console.WriteLine(entry);

    foreach (var item in entry.Chunks)
    {
        var randomFile = Path.GetRandomFileName() + ".bin";
        await using var fs = new FileStream(Path.Join("D:\\dev\\ffxiv-tools\\torrents\\extracted", randomFile),
            FileMode.OpenOrCreate);
        
        item.Save(fs);
    }
}
