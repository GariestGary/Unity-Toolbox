using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T instance;
        private static object lockObject = new object();
        private static bool destroyed = false;
        private static bool reinstantiateIfDestroyed = true;
        private static bool initialized = false;

        public static bool HasInstance => initialized && instance != null;
        public static bool ReinstantiateIfDestroyed
        {
            get
            {
                return reinstantiateIfDestroyed;
            }
            set
            {
                if(value)
                {
                    destroyed = false;
                }

                reinstantiateIfDestroyed = value;
            }
        }

        public static T Instance 
        { 
            get
            {
                if (!reinstantiateIfDestroyed && destroyed) return null;
                
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        #if UNITY_6000_0_OR_NEWER
                        instance = FindFirstObjectByType<T>();
                        #else
                        instance = FindObjectOfType<T>();
                        #endif

                        if (instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
                            destroyed = false;
                            instance = singleton.AddComponent<T>();
                        }

                        Application.quitting += ClearInstance;
                        initialized = true;
                    }
                    return instance;
                }
            } 
        }

        public static void DontDestroy()
        {
            DontDestroyOnLoad(instance.gameObject);
        }

        private static void ClearInstance()
        {
            Application.quitting -= ClearInstance;
            instance = null;
            destroyed = true;
            initialized = false;
        }
        
        private void OnDestroy()
        {
            if (initialized)
            {
                ClearInstance();
            }
        }
    }

    public class CachedSingleton<T>: MonoCached where T: MonoCached
    {
        private static T instance;
        private static object lockObject = new object();
        private static bool destroyed = false;
        private static bool reinstantiateIfDestroyed = true;
        private static bool initialized;

        public static bool HasInstance => instance != null && Application.isPlaying;

        public static bool ReinstantiateIfDestroyed
        {
            get
            {
                return reinstantiateIfDestroyed;
            }
            set
            {
                reinstantiateIfDestroyed = value;
            }
        }

        public static T Instance 
        { 
            get 
            {
                if (!reinstantiateIfDestroyed && destroyed) return null;

                lock (lockObject)
                {
                    if (instance == null)
                    {
#if UNITY_6000_0_OR_NEWER
                        instance = FindFirstObjectByType<T>();
#else
                        instance = FindObjectOfType<T>();
#endif

                        if (instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
                            destroyed = false;
                            instance = singleton.AddComponent<T>();
                        }
                        
                        Application.quitting += ClearInstance;
                        initialized = true;
                    }
                    
                    return instance;
                }
            } 
        }

        public static void DontDestroy()
        {
            DontDestroyOnLoad(instance.gameObject);
        }

        private static void ClearInstance()
        {
            Application.quitting -= ClearInstance;
            instance = null;
            destroyed = true;
            initialized = false;
        }
        
        private void OnDestroy()
        {
            if (initialized)
            {
                ClearInstance();
            }
        }
    }

    public class Singleton<T> where T: class, new()
    {
        private static T instance;
        private static object lockObject = new object();

        public static bool HasInstance => instance != null && Application.isPlaying;

        public static T Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                    return instance;
                }
            }
        }

        public void ClearInstance()
        {
            instance = null;
        }
    }
}