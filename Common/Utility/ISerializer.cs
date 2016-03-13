namespace Common.Utility
{
    public interface ISerializer<T>
    {
        byte[] Serialize(T entity);
        T Deserialize(byte[] bytes);
    }
}
