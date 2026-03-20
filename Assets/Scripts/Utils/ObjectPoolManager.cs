using System.Collections.Generic;
using UnityEngine;
using SongShuVegetable.Utils;

namespace SongShuVegetable.Utils
{
    /// <summary>
    /// 全局对象池管理器（Singleton）。
    /// 以 Prefab 为 key，维护各自的对象池队列。
    /// 用法：
    ///   var obj = ObjectPoolManager.Instance.Get(bulletPrefab, pos, rot);
    ///   ObjectPoolManager.Instance.Release(bulletPrefab, obj);
    /// </summary>
    public class ObjectPoolManager : SingletonMonoBehaviour<ObjectPoolManager>
    {
        [SerializeField] private int _defaultCapacity = 10;

        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();
        private Transform _poolRoot;

        protected override void Awake()
        {
            base.Awake();
            _poolRoot = new GameObject("[PoolRoot]").transform;
            _poolRoot.SetParent(transform);
        }

        // ── 预热：提前创建 count 个实例 ─────────────────────────
        public void Prewarm(GameObject prefab, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = CreateInstance(prefab);
                obj.SetActive(false);
                GetQueue(prefab).Enqueue(obj);
            }
            Log.Info("Pool", $"预热 {prefab.name} x{count}");
        }

        // ── 获取对象 ─────────────────────────────────────────────
        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var queue = GetQueue(prefab);
            GameObject obj;

            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
            }
            else
            {
                obj = CreateInstance(prefab);
                Log.Info("Pool", $"{prefab.name} 池已空，扩容创建新实例");
            }

            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
            return obj;
        }

        public GameObject Get(GameObject prefab, Vector3 position)
            => Get(prefab, position, Quaternion.identity);

        // ── 归还对象 ─────────────────────────────────────────────
        public void Release(GameObject obj)
        {
            if (obj == null) return;

            if (!_instanceToPrefab.TryGetValue(obj, out var prefab))
            {
                Log.Warning("Pool", $"{obj.name} 不属于任何对象池，直接销毁");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(_poolRoot);
            GetQueue(prefab).Enqueue(obj);
        }

        // ── 清空指定 prefab 的池 ──────────────────────────────────
        public void Clear(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var queue)) return;
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                _instanceToPrefab.Remove(obj);
                Destroy(obj);
            }
            _pools.Remove(prefab);
            Log.Info("Pool", $"已清空 {prefab.name} 对象池");
        }

        // ── 内部工具 ──────────────────────────────────────────────
        private Queue<GameObject> GetQueue(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>(_defaultCapacity);
                _pools[prefab] = queue;
            }
            return queue;
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            var obj = Instantiate(prefab, _poolRoot);
            _instanceToPrefab[obj] = prefab;
            return obj;
        }
    }
}
