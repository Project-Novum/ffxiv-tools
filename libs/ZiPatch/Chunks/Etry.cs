using System.Text;

namespace ZiPatch.Chunks;

public class Etry : Chunk
{
    public override ChunkType ChunkType => ChunkType.Etry;
    public string Path { get; }
    public uint Count { get; }
    public EtryChunk[] Chunks { get; }
    
    public Etry(EndiannessAwareBinaryReader reader) : base(reader)
    {
        Path = Encoding.ASCII.GetString(reader.ReadBytes(reader.ReadInt32()));
        Count = reader.ReadUInt32();
        Chunks = new EtryChunk[Count];

        for (var i = 0; i < Chunks.Length; i++)
        {
            Chunks[i] = new EtryChunk(reader);
        }
        
        reader.BaseStream.Seek(0x08, SeekOrigin.Current);
    }

    public override string ToString()
    {
        return $"Path: {Path} Count: {Count}";
    }
}