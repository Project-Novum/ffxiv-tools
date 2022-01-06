namespace ZiPatch.Chunks;

public class Aply : Chunk
{
    public override ChunkType ChunkType => ChunkType.Aply;
    public uint Unknown1 { get; }
    public uint Unknown2 { get; }
    public uint Unknown3 { get; }

    public Aply(EndiannessAwareBinaryReader reader) : base(reader)
    {
        Unknown1 = reader.ReadUInt32();
        Unknown2 = reader.ReadUInt32();
        Unknown3 = reader.ReadUInt32();
    }
    
    public override string ToString()
    {
        return
            $"Unknown1: 0x{Unknown1:X4} Unknown2: 0x{Unknown2:X4} Unknown1: 0x{Unknown3:X4}";
    }

}