using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pool
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Stack<T> _available = new();
        private readonly HashSet<T> _inUse = new();

        public int CountInactive => _available.Count;
        public int CountInUse => _inUse.Count;

        public ObjectPool(T prefab, Transform container)
        {
            if (prefab == null)
                throw new System.ArgumentNullException(nameof(prefab));

            if (container == null)
                throw new System.ArgumentNullException(nameof(container));

            _prefab = prefab;
            _container = container;
        }

        public void Preload(T instance)
        {
            if (instance == null)
                throw new System.ArgumentNullException(nameof(instance));

            instance.transform.SetParent(_container);
            instance.gameObject.SetActive(false);
            _available.Push(instance);
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            T instance = PopAvailable();

            if (instance == null)
            {
                instance = Object.Instantiate(_prefab, _container);
                instance.gameObject.SetActive(false);
            }

            instance.transform.SetPositionAndRotation(position, rotation);

            if (instance is IPoolable poolable)
                poolable.OnGetFromPool();

            instance.gameObject.SetActive(true);

            _inUse.Add(instance);

            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null)
                return;

            if (!_inUse.Remove(instance))
            {
                UnityEngine.Debug.LogWarning(
                    $"{nameof(ObjectPool<T>)}: '{instance.name}' was released but is not active in this pool. Ignoring double return.");
                return;
            }

            if (instance is IPoolable poolable)
                poolable.OnReturnToPool();

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_container);
            _available.Push(instance);
        }

        private T PopAvailable()
        {
            while (_available.Count > 0)
            {
                T instance = _available.Pop();

                if (instance != null)
                    return instance;
            }

            return null;
        }
    }
}
