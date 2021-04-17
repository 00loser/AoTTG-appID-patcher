using System;

public static class Utils
{
    public static string ToHex(this long lng) => lng.ToString("X");

    public static int FindBytes(byte[] src, byte[] find)
    {
        int index = -1;
        int matchIndex = 0;
        for (int i = 0; i < src.Length; i++)
        {
            if (src[i] == find[matchIndex])
            {
                if (matchIndex == (find.Length - 1))
                {
                    index = i - matchIndex;
                    break;
                }
                matchIndex++;
            }
            else if (src[i] == find[0])
            {
                matchIndex = 1;
            }
            else
            {
                matchIndex = 0;
            }
        }
        return index;
    }

    public static byte[] ReplaceBytes(byte[] src, byte[] search, byte[] repl)
    {
        byte[] dst = null;
        int index = FindBytes(src, search);
        if (index >= 0)
        {
            dst = new byte[src.Length - search.Length + repl.Length];
            Buffer.BlockCopy(src, 0, dst, 0, index);
            Buffer.BlockCopy(repl, 0, dst, index, repl.Length);
            Buffer.BlockCopy(src, index + search.Length, dst, index + repl.Length, src.Length - (index + search.Length));
        }
        return dst;
    }
}
