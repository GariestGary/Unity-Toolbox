using System.Globalization;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T instance;
        private static object lockObject = new object();

        public static T Instance { get 
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
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
    }

    public class CachedSingleton<T>: MonoCached where T: MonoCached
    {
        private static T instance;
        private static object lockObject = new object();

        public static T Instance { get 
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindObjectOfType<T>();

                        if (instance == null)
                        {
                            var singleton = new GameObject("[SINGLETON] " + typeof(T));
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
    }
}