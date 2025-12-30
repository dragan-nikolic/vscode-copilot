using UnityEngine;

namespace CardGame.Utilities
{
    /// <summary>
    /// Generic Singleton pattern for MonoBehaviour classes.
    /// Ensures only one instance exists and provides global access point.
    /// </summary>
    /// <typeparam name="T">Type of the singleton class</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)} already destroyed. Returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject($"{typeof(T).Name} (Singleton)");
                            _instance = singleton.AddComponent<T>();
                            DontDestroyOnLoad(singleton);

                            Debug.Log($"[Singleton] Created instance of {typeof(T)} with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log($"[Singleton] Using existing instance of {typeof(T)}.");
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} destroyed.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _applicationIsQuitting = true;
            }
        }
    }
}
