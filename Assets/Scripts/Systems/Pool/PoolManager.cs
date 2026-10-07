using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pool
{
    public class PoolManager : MonoBehaviour
    {
        [Serializable]
        private class PoolEntry
        {
            [SerializeField] private GameObject _prefab;
            [SerializeField, Min(0)] private int _prewarm;

            public GameObject Prefab => _prefab;
            public int Prewarm => _prewarm;
        }

        [SerializeField] private List<PoolEntry> _pools = new();

        private readonly Dictionary<GameObject, object> _poolsByPrefab = new();
        private readonly Dictionary<Component, object> _poolsByInstance = new();
        private readonly Dictionary<GameObject, List<GameObject>> _preloadedInstances = new();

        private Transform _container;

        public static PoolManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                throw new InvalidOperationException($"{name}: only one PoolManager is allowed in the scene.");

            Instance = this;

            _container = new GameObject("Pools").transform;
            _container.SetParent(transform);

            foreach (PoolEntry entry in _pools)
            {
                if (entry.Prefab == null)
                    throw new MissingReferenceException($"{name}: a pool entry is missing its prefab.");

                if (_preloadedInstances.ContainsKey(entry.Prefab))
                    throw new InvalidOperationException($"{name}: duplicate pool entry for '{entry.Prefab.name}'.");

                List<GameObject> instances = new(entry.Prewarm);

                for (int i = 0; i < entry.Prewarm; i++)
                {
                    GameObject instance = Instantiate(entry.Prefab, _container);
                    instance.SetActive(false);
                    instances.Add(instance);
                }

                _preloadedInstances[entry.Prefab] = instances;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            ObjectPool<T> pool = GetOrCreatePool(prefab);

            T instance = pool.Get(position, rotation);
            _poolsByInstance[instance] = pool;

            return instance;
        }

        public void Release<T>(T instance) where T : Component
        {
            if (instance == null)
                return;

            if (!_poolsByInstance.TryGetValue(instance, out object pool))
            {
                UnityEngine.Debug.LogWarning(
                    $"{name}: '{instance.name}' does not belong to any pool. Ignoring release.");
                return;
            }

            ((ObjectPool<T>)pool).Release(instance);
        }

        private ObjectPool<T> GetOrCreatePool<T>(T prefab) where T : Component
        {
            if (_poolsByPrefab.TryGetValue(prefab.gameObject, out object existing))
            {
                if (existing is ObjectPool<T> typed)
                    return typed;

                throw new InvalidOperationException(
                    $"{name}: pool for '{prefab.name}' was created as {existing.GetType()} but requested as {typeof(ObjectPool<T>)}.");
            }

            if (!_preloadedInstances.TryGetValue(prefab.gameObject, out List<GameObject> preloaded))
                throw new MissingReferenceException(
                    $"{name}: no pool entry is registered for '{prefab.name}'. Add it to the Pools list in the inspector.");

            ObjectPool<T> pool = new(prefab, _container);

            foreach (GameObject instance in preloaded)
            {
                if (!instance.TryGetComponent(out T component))
                    throw new MissingReferenceException(
                        $"{name}: preloaded instance '{instance.name}' has no {typeof(T).Name} component.");

                pool.Preload(component);
            }

            _preloadedInstances.Remove(prefab.gameObject);
            _poolsByPrefab[prefab.gameObject] = pool;

            return pool;
        }
    }
}
