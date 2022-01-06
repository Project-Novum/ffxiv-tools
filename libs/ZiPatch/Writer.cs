using Be.IO;

namespace ZiPatch;

public class Writer : IDisposable
{
    private readonly BeBinaryWriter _binaryWriter;
    
    public Writer(string file)
    {
        _binaryWriter =
            new BeBinaryWriter(new FileStream(file, FileMode.Create));
        
        _binaryWriter.Write(Constants.ZiPatchHeader);
    }

    public void Save()
    {
        _binaryWriter.Flush();
    }

    public void Dispose()
    {
        _binaryWriter.Dispose();
    }
}