namespace FFXIVDataHandler.Structs;

public class Header
{
    public char[] Type { get; }
    public byte[] Unknown { get; }
    public int PayloadSize { get; }

    public Header(BinaryReader reader)
    {
        Type = reader.ReadChars(4);
        Unknown = reader.ReadBytes(4);
        PayloadSize = reader.ReadInt32();

        reader.BaseStream.Position += 1;
    }
}