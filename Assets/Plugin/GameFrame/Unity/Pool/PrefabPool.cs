using System;
using System.Collections.Generic;
using System.Linq;
using GameFrame.Pool;
using UnityEditor;
using UnityEngine;

namespace GameFrame.Unity.Pool
{
    public class PrefabPool : IPool<GameObject>, IDisposable 
    {
        public bool isDisposed { get; private set; }
        private readonly Transform _stashBox;
        private readonly GameObject _prefab;
        private readonly Stack<GameObject> _stashedObjects;
        public PrefabPool(GameObject prefab, int prewarm = 0) {
            isDisposed = false;
            var stashObject = new GameObject();
            GameObject.DontDestroyOnLoad(stashObject);
            stashObject.SetActive(false);
            _stashBox = stashObject.transform;
            _prefab = prefab;
            _stashedObjects = new Stack<GameObject>();
            if (prewarm > 0)
            {
                Enumerable.Range(0, prewarm)
                    .Select(_ => Fetch())
                    .ToList()
                    .ForEach(x=>Release(x));
            }
        }

        public GameObject Fetch()
        {
            if (!isDisposed)
            {
                if (!_stashedObjects.TryPop(out var result))
                {
                    var newInstance = GameObject.Instantiate(_prefab, _stashBox);
                    newInstance.SetActive(false);
                    return newInstance;
                }
                return result;
            }
            return null;
        }

        public void Release(GameObject item)
        {
            if (!isDisposed)
            {
                item.SetActive(false);
                item.transform.SetParent(_stashBox);
                _stashedObjects.Push(item);
                return;
            }
            GameObject.Destroy(item);
        }

        public void Dispose()
        {
            isDisposed = true;
            foreach (var item in _stashedObjects)
            {
                GameObject.Destroy(item);
            }
            GameObject.Destroy(_stashBox.gameObject);
        }
    }
}