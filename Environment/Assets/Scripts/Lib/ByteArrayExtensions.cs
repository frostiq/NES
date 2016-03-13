using System.IO;
using Unity.IO.Compression;

namespace Assets.Scripts.Lib
{
    public static class ByteArrayExtensions
    {
        public static byte[] CompressIntoGzip(this byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                return compressedStream.ToArray();
            }
        }
    }
}
