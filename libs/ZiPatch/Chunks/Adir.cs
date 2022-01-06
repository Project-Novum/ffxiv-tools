using System.Text;
using Be.IO;

namespace ZiPatch.Chunks;

public class Adir : Chunk
{
    public override ChunkType ChunkType { get; } = ChunkType.Adir;
    
    public string Path { get; set; }

    public Adir(BeBinaryReader reader) : base(reader)
    {
        Path = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt32()));
    }

    public override string ToString()
    {
        return Path;
    }
}