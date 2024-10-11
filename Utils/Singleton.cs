using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MonoSingleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T instance;
        private static object lockObject = new object();
        private static bool destroyed = false;
        private static bool reinstantiateIfDestroyed = true;

        public static bool HasInstance => instance != null;
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
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
                            destroyed = false;
                            instance = singleton.AddComponent<T>();
                        }
                    }
                    return instance;
                }
            } 
        }

        public static void DontDestroy()
        {
            DontDestroyOnLoad(Instance.gameObject);
        }

        private void OnDestroy()
        {
            instance = null;
            destroyed = true;
        }
    }

    public class CachedSingleton<T>: MonoCached where T: MonoCached
    {
        private static T instance;
        private static object lockObject = new object();
        private static bool destroyed = false;
        private static bool reinstantiateIfDestroyed = true;

        public static bool HasInstance
        {
            get
            {
                return FindObjectOfType<T>() != null;
            }
            private set { }
        }

        public static bool ReinstantiateIfDestroyed
        {
            get
            {
                return reinstantiateIfDestroyed;
            }
            set
            {
                if (value)
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
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
                            destroyed = false;
                            instance = singleton.AddComponent<T>();
                        }
                    }
                    
                    return instance;
                }
            } 
        }

        public static void DontDestroy()
        {
            DontDestroyOnLoad(Instance.gameObject);
        }

        private void OnDestroy()
        {
            instance = null;
            destroyed = true;
        }
    }

    public class Singleton<T> where T: class
    {
        private static T instance;
        private static object lockObject = new object();

        public static bool HasInstance => instance != null;

        public static T Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = default(T);
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