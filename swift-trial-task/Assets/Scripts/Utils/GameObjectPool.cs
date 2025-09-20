using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Utils
{
    public class GameObjectPool<T> where T : Component
    {
        private readonly Stack<T> _freeStack;

        public bool HasAvailableObjects => _freeStack.Count > 0;

        public GameObjectPool(T prefab, int size, Transform parent = null)
        {
            _freeStack = new Stack<T>(size);
            for (int i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);

                _freeStack.Push(obj);
            }
        }

        public T Get()
        {
            if (_freeStack.Count == 0)
            {
                throw new System.InvalidOperationException("Pool capacity exceeded!");
            }

            var item = _freeStack.Pop();
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            if (_freeStack.Contains(item))
            {
                throw new System.InvalidOperationException($"Item {item} is already in the pool!");
            }
            
            item.gameObject.SetActive(false);
            _freeStack.Push(item);
        }
    }
}