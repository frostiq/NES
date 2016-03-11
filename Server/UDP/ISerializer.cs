namespace NeuroServer.Udp
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T entity);
        T Deserialize<T>(byte[] bytes);
    }
}
