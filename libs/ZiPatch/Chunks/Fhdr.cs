using System.Text;
using Be.IO;

namespace ZiPatch.Chunks;

public class Fhdr : Chunk
{    
    public override ChunkType ChunkType => ChunkType.Fhdr;
    public uint Version { get; }
    public string Result { get; }
    public uint NumberEntryFile { get; }
    public uint NumberAddDir { get; }
    public uint NumberDeleteDir { get; }

    public Fhdr(BeBinaryReader reader) : base(reader)
    {
        Version = reader.ReadUInt32();
        Result = Encoding.ASCII.GetString(reader.ReadBytes(4));
        NumberEntryFile = reader.ReadUInt32();
        NumberAddDir = reader.ReadUInt32();
        NumberDeleteDir = reader.ReadUInt32();
    }

    public override string ToString()
    {
        return
            $"version: 0x{Version:X4} Result: {Result} NumberEntryFile: {NumberEntryFile} NumberAddDir: {NumberAddDir} NumberDeleteDir: {NumberDeleteDir}";
    }
}