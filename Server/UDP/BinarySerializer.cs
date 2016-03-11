using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeuroServer.Udp
{
    public class BinarySerializer : ISerializer
    {
        private static readonly BinaryFormatter Formatter = new BinaryFormatter();

        public byte[] Serialize<T>(T entity)
        {
            using (var stream = new MemoryStream())
            {
                Formatter.Serialize(stream, entity);
                return stream.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;
                return (T)Formatter.Deserialize(stream);
            }
        }
    }
}
