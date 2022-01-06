using System.Text;
using Be.IO;

namespace ZiPatch.Chunks;

public class Dled : Chunk
{
    public override ChunkType ChunkType { get; } = ChunkType.Dled;
    
    public string Path { get; set; }

    public Dled(BeBinaryReader reader) : base(reader)
    {
        Path = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt32()));
    }

    public override string ToString()
    {
        return Path;
    }
}