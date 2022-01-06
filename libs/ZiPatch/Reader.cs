using System.Text;
using ZiPatch.Chunks;

namespace ZiPatch;

public class Reader : IDisposable
{
    private readonly EndiannessAwareBinaryReader _binaryReader;
    public Reader(string file)
    {
        _binaryReader =
            new EndiannessAwareBinaryReader(new FileStream(file, FileMode.Open), EndiannessAwareBinaryReader.Endianness.Big);
    }

    public Task<List<Chunk>> GetChunksAsync()
    {
        return GetChunksAsync(CancellationToken.None);
    }
    
    public Task<List<Chunk>> GetChunksAsync(CancellationToken cancellationToken)
    {
        return Task.Run(GetChunks, cancellationToken);
    }

    public List<Chunk> GetChunks()
    {
        var chunks = new List<Chunk>();
        
        while (_binaryReader.BaseStream.Position < _binaryReader.BaseStream.Length)
        {
            var type = Encoding.ASCII.GetString(_binaryReader.ReadBytes(4));
            switch (type)
            {
                case "FHDR":
                    chunks.Add(new Fhdr(_binaryReader));
                    break;
                case "APLY":
                    chunks.Add(new Aply(_binaryReader));
                    break;
                case "ADIR":
                    break;
                case "ETRY":
                    chunks.Add(new Etry(_binaryReader));
                    break;
            }
        }
        _binaryReader.BaseStream.Seek(0, SeekOrigin.Begin);
        
        return chunks;
    }

    public void Dispose() => _binaryReader.Dispose();
}