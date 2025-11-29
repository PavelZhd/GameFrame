using System.Text;
using UnityEngine;

namespace GameFrame.Savable
{
    public class JSONDataSerializer : IDataSerializer
    {
        public byte[] Serialize(object source)
        {
            return Encoding.UTF32.GetBytes(JsonUtility.ToJson(source));
        }

        public bool TryDesealize<T>(byte[] data, out T result)
        {
            try
            {
                result = (T)JsonUtility.FromJson(Encoding.UTF32.GetString(data), typeof(T));
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }
    }
}