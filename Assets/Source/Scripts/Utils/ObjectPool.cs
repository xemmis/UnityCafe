using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public sealed class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> _pool = new();
        private readonly HashSet<T> _inPool = new();   
        private readonly T _prefab;
        private readonly Transform _parent;          
        private readonly int _maxSize;               

        public int CountInactive => _pool.Count;

        public ObjectPool(T prefab, int initialSize, int maxSize = int.MaxValue, Transform parent = null)
        {
            _prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
            _maxSize = maxSize > 0 ? maxSize : throw new ArgumentOutOfRangeException(nameof(maxSize));
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
                AddToPool(CreateInstance());
        }

        /// <summary>Возвращает активный объект из пула (или создаёт новый).</summary>
        public T Get()
        {
            T obj = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
            _inPool.Remove(obj);
            obj.gameObject.SetActive(true);  // <-- была забыта активация
            return obj;
        }

        /// <summary>Возвращает объект в пул. Повторный возврат игнорируется.</summary>
        public bool Return(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            // защита от двойного возврата
            if (_inPool.Contains(obj))
            {
                Debug.LogWarning($"[ObjectPool] Object '{obj.name}' returned twice — ignoring.", obj);
                return false;
            }

            // если пул уже заполнен — уничтожаем лишний объект
            if (_pool.Count >= _maxSize)
            {
                UnityEngine.Object.Destroy(obj.gameObject);
                return false;
            }

            AddToPool(obj);
            return true;
        }

        /// <summary>Уничтожает все объекты в пуле и очищает его.</summary>
        public void Dispose()
        {
            foreach (T obj in _pool)
                if (obj != null)
                    UnityEngine.Object.Destroy(obj.gameObject);

            _pool.Clear();
            _inPool.Clear();
        }

        private T CreateInstance()
        {
            T obj = UnityEngine.Object.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            return obj;
        }

        private void AddToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
            _inPool.Add(obj);
        }
    }
}