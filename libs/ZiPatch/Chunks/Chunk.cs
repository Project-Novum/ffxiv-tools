using Be.IO;

namespace ZiPatch.Chunks;

public enum ChunkType
{
    Aply,
    Etry,
    EtryData,
    Fhdr,
    Adir,
    Dled
}

public abstract class Chunk
{
    protected readonly BeBinaryReader Reader;
    public abstract ChunkType ChunkType { get; }
    
    protected Chunk(BeBinaryReader reader)
    {
        Reader = reader;
    }
}