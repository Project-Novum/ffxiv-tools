namespace ZiPatch.Chunks;

public enum ChunkType
{
    Aply,
    Etry,
    EtryData,
    Fhdr
}

public abstract class Chunk
{
    protected readonly EndiannessAwareBinaryReader Reader;
    public abstract ChunkType ChunkType { get; }
    
    protected Chunk(EndiannessAwareBinaryReader reader)
    {
        Reader = reader;
    }
}