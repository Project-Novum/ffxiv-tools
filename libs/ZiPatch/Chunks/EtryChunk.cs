using System.IO.Compression;

namespace ZiPatch.Chunks;

public class EtryChunk : Chunk
{
    public override ChunkType ChunkType => ChunkType.EtryData;
    public uint HashMode { get; }
    public byte[] SourceFileHash { get; }
    public byte[] DestFileHash { get; }
    public uint CompressMode { get; }
    public uint CompressedFileSize { get; }
    public uint PreviousFileSize { get; }
    public uint NewFileSize { get; }
    
    /// <summary>
    /// Location of the Etry block to get it's block data later
    /// </summary>
    public long Position { get; }
    
    public EtryChunk(EndiannessAwareBinaryReader reader) : base(reader)
    {
        HashMode = reader.ReadUInt32();
        SourceFileHash = reader.ReadBytes(0x14);
        DestFileHash = reader.ReadBytes(0x14);
        CompressMode = reader.ReadUInt32();
        CompressedFileSize = reader.ReadUInt32();
        PreviousFileSize = reader.ReadUInt32();
        NewFileSize = reader.ReadUInt32();
        
        Position = reader.BaseStream.Position;

        reader.BaseStream.Seek(CompressedFileSize, SeekOrigin.Current);
    }

    public void Save(Stream output)
    {
        var size = CompressedFileSize;
        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
        Reader.BaseStream.Seek(Position, SeekOrigin.Begin);
        if (CompressMode == 0x4E)
        {
            output.Write(Reader.ReadBytes((int)CompressedFileSize));
        } else if (CompressMode == 0x5A)
        {
            var compressedFileStream = new MemoryStream(Reader.ReadBytes((int) CompressedFileSize));
            using var compressor = new DeflateStream(compressedFileStream, CompressionMode.Decompress);
            compressor.CopyTo(output);
        }
        else
        {
            throw new Exception($"Unknown compress type 0x{CompressMode:X}");
        }
    }
}