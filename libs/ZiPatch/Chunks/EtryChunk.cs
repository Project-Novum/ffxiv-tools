using System.Diagnostics;
using System.IO.Compression;
using Be.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ZiPatch.Chunks;

public class EtryChunk : Chunk
{
    public override ChunkType ChunkType => ChunkType.EtryData;
    public int Mode { get; }
    public byte[] SourceFileHash { get; }
    public byte[] DestFileHash { get; }
    public int CompressMode { get; }
    public uint CompressedFileSize { get; }
    public uint PreviousFileSize { get; }
    public uint NewFileSize { get; }
    
    /// <summary>
    /// Location of the Etry block to get it's block data later
    /// </summary>
    public long Position { get; }
    
    public EtryChunk(BeBinaryReader reader) : base(reader)
    {
        Mode = BitConverter.ToInt32(reader.ReadBytes(4));
        SourceFileHash = reader.ReadBytes(0x14);
        DestFileHash = reader.ReadBytes(0x14);
        CompressMode = BitConverter.ToInt32(reader.ReadBytes(4));
        CompressedFileSize = reader.ReadUInt32();
        PreviousFileSize = reader.ReadUInt32();
        NewFileSize = reader.ReadUInt32();
        
        Position = reader.BaseStream.Position;

        reader.BaseStream.Seek(CompressedFileSize, SeekOrigin.Current);
    }

    public void Save(Stream output)
    {
        Reader.BaseStream.Seek(0, SeekOrigin.Begin);
        Reader.BaseStream.Seek(Position, SeekOrigin.Begin);
        if (CompressMode == 0x4E)
        {
            output.Write(Reader.ReadBytes((int)CompressedFileSize));
        } else if (CompressMode == 0x5A)
        {
            var compressedFileStream = new MemoryStream(Reader.ReadBytes((int) CompressedFileSize));
            using InflaterInputStream inflater = new InflaterInputStream(compressedFileStream);
            inflater.CopyTo(output);
            output.Position = 0;
        }
        else
        {
            throw new Exception($"Unknown compress type 0x{CompressMode:X}");
        }
    }
}