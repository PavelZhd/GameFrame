using UnityEngine;

namespace GameFrame.Pool
{
    public interface IPool<T>
    {
        public T Fetch();
        public void Release(T item);
    }
}