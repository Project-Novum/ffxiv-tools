using FFXIVDataHandler.Structs;

namespace FFXIVDataHandler.IO;

public class Lpb : IDisposable
{
    private readonly Header _header;

    private readonly BinaryReader _binaryReader;

    public Lpb(string file) : this(new FileStream(file, FileMode.Open, FileAccess.Read))
    {}

    public Lpb(Stream stream)
    {
        _binaryReader = new BinaryReader(stream);

        _header = new Header(_binaryReader);
    }

    public Task SaveAsync(string file) => Task.Run(() => Save(file));

    public Task SaveAsync(Stream output) => Task.Run(() => Save(output));

    public void Save(string file)
    {
        using var fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        Save(fs);
    }

    public void Save(Stream output)
    {
        var data = Read();
        output.Write(data);
        output.Flush();
    }

    public Task<byte[]> ReadAsync()
    {
        return Task.Run(Read);
    }

    public byte[] Read()
    {
        var data = _binaryReader.ReadBytes(_header.PayloadSize);

        return Utils.Decrypt(data);
    }

    public void Dispose()
    {
        _binaryReader.Dispose();
    }
}