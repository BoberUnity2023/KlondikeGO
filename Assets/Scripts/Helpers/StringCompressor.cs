using System.IO.Compression;
using System.IO;
using System.Text;
using System;

namespace BloomLines.Helpers
{
    public static class StringCompressor 
    {
        public static string CompressStringBrotli(string str)
        {
            using (var mso = new MemoryStream())
            {
                using (var bs = new BrotliStream(mso, CompressionLevel.Optimal))
                {
                    using (var writer = new StreamWriter(bs, Encoding.UTF8))
                    {
                        writer.Write(str);
                    }
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        public static string DecompressStringBrotli(string compressedStr)
        {
            var bytes = Convert.FromBase64String(compressedStr);
            using (var msi = new MemoryStream(bytes))
            {
                using (var bs = new BrotliStream(msi, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(bs, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}