using UnityEngine;

namespace SongShuVegetable.Utils
{
    /// <summary>
    /// 泛型单例基类，挂载到 GameObject 上自动注册。
    /// 场景切换时不销毁（DontDestroyOnLoad），重复实例自动销毁。
    /// </summary>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                        Debug.LogWarning($"[Singleton] {typeof(T).Name} 实例未找到，请在场景中添加对应 GameObject。");
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
