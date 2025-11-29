using UnityEngine;

namespace GameFrame.Savable
{
    public interface IDataSerializer
    {
        byte[] Serialize(object source);
        bool TryDesealize<T>(byte[] data, out T result);
    }
}