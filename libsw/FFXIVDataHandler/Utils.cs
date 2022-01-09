namespace FFXIVDataHandler;

public class Utils
{
    public static byte[] Decrypt(byte[] encoded)
    {
        byte[] decoded = new byte[encoded.Length];

        for (var i = 0; i < decoded.Length; i++)
        {
            decoded[i] = (byte)(encoded[i] ^ 0x73);
        }

        return decoded;
    }

    public static byte[] Encrypt(byte[] decoded)
    {
        var encoded = new byte[decoded.Length];

        for (var i = 0; i < encoded.Length; i++)
        {
            encoded[i] = (byte)(0x73 ^ decoded[i]);
        }

        return encoded;
    }
}