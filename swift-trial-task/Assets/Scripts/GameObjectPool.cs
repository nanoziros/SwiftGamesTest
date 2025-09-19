using UnityEngine;

namespace Scripts
{
    public class GameObjectPool<T> where T : Component
    {
        private readonly T[] _pool;
        private int _poolIndex = 0;
        private readonly Transform _parent;

        public int Count => _poolIndex;
        public int Capacity => _pool.Length;
        
        public GameObjectPool(T prefab, int size, Transform parent = null)
        {
            _parent = parent;
            _pool = new T[size];

            for (int i = 0; i < size; i++)
            {
                var component = Object.Instantiate(prefab, parent);
                component.gameObject.SetActive(false);
                _pool[i] = component;
            }
        }

        public T Get()
        {
            if (_poolIndex >= _pool.Length)
            {
                Debug.LogWarning($"We have reached the maximum pooling capacity {_pool.Length} for {this}");
                return null;
            }

            T item = _pool[_poolIndex];
            _poolIndex++;
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);

            _poolIndex--;
            _pool[_poolIndex] = item;
        }
    }
}