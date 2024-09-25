using System;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Pooler: ResourcesToolWrapper<Pooler, PoolerDataHolder>
    {
        [SerializeField] private Transform _PredefinedRoot;

        public override string GetDataPath()
        {
            return SettingsData.poolerResourcesDataPath;
        }

        protected override void PostLoadRun()
        {
            Data.Run();
            Data.SetCustomRoot(_PredefinedRoot);
        }

        protected override void Clear()
        {
            Data?.Clear();
        }

        public static void ForceGarbageCollectorWork()
        {
            Instance.Data.ForceGarbageCollector();
        }

        public static int GetPoolObjectsCount(string poolTag)
        {
            return Instance.Data.GetPoolObjectsCount(poolTag);
        }

        public static Pool TryAddPool(PoolData poolToAdd)
        {
            return Instance.Data.TryAddPool(poolToAdd);
        }

        public static void TryRemovePool(Pool pool)
        {
            Instance.Data.TryRemovePool(pool);
        }

        public static void TryAddPool(string tag, GameObject prefab, int capacity)
        {
            Instance.Data.TryAddPool(tag, prefab, capacity);
        }

        public static void TryRemovePool(string tag)
        {
            Instance.Data.TryRemovePool(tag);
        }

        /// <summary>
        /// Spawns GameObject from pool with specified tag
        /// </summary>
        /// <param name="poolTag">pool tag with necessary object</param>
        /// <param name="position">initial position</param>
        /// <param name="rotation">initial rotation</param>
        /// <param name="parent">parent transform for GameObject</param>
        /// <param name="data">data to provide in GameObject</param>
        /// <returns>GameObject from pool</returns>
        public static GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null)
        {
            return Instance.Data.Spawn(poolTag, position, rotation, parent, data);
        }

        /// <summary>
        /// Spawns GameObject from pool with specified tag
        /// </summary>
        /// <param name="poolTag">pool tag with necessary object</param>
        /// <param name="parent">parent transform for GameObject</param>
        /// <param name="data">data to provide in GameObject</param>
        /// <returns>GameObject from pool</returns>
        public static GameObject Spawn(string poolTag, object data = null, Transform parent = null)
        {
            return Instance.Data.Spawn(poolTag, parent, data);
        }

        /// <summary>
        /// Spawns GameObject from pool with specified tag and returns specified component from it
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <param name="poolTag">pool tag with necessary object</param>
        /// <param name="position">initial position</param>
        /// <param name="rotation">initial rotation</param>
        /// <param name="parent">parent transform for GameObject</param>
        /// <param name="data">data to provide in GameObject</param>
        /// <returns>Component from spawned GameObject</returns>
        public static T Spawn<T>(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null) where T: Component
        {
            return Spawn(poolTag, position, rotation, parent, data).GetComponent<T>();
        }

        /// <summary>
        /// Spawns GameObject from pool with specified tag and returns specified component from it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolTag"></param>
        /// <param name="data"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T Spawn<T>(string poolTag, object data = null, Transform parent = null)
        {
            return Spawn(poolTag, data, parent).GetComponent<T>();
        }

        /// <summary>
        /// Alternative to Unity's Instantiate method, that automatically injects object
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <returns>Instantiated GameObject</returns>
        public static GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Instance.Data.Create(prefab, position, rotation, parent);
        }
        
        /// <summary>
        /// Alternative to Unity's Instantiate method, which uses Vector3.zero, Quaternion.identity and automatically injects object
        /// </summary>
        /// <param name="prefab">Prefab to Instantiate</param>
        /// <param name="parent">Parent object to instantiated GameObject</param>
        /// <returns>Instantiated GameObject</returns>
        public static GameObject Create(GameObject prefab, Transform parent = null)
        {
            return Instance.Data.Create(prefab, Vector3.zero, Quaternion.identity, parent);
        }

        /// <summary>
        /// Removes GameObject from scene and returns it to pool
        /// </summary>
        /// <param name="objectToDespawn">object to despawn</param>
        /// <param name="delay">delay before despawning</param>
        public static bool TryDespawn(GameObject objectToDespawn)
        {
            return Instance.Data.TryDespawn(objectToDespawn);
        }

        /// <summary>
        /// Decides to despawn GameObject if it in pools list or destroy if not
        /// </summary>
        /// <param name="obj">GameObject to despawn or destroy</param>
        /// <param name="delay">Delay before despawning or destroying</param>
        public static void DespawnOrDestroy(GameObject obj)
        {
            Instance.Data.DespawnOrDestroy(obj);
        }
    }
}