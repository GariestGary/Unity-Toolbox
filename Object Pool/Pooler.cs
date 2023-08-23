using System;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Pooler: ResourcesToolWrapper<Pooler, PoolerDataHolder>
    {
        public override string GetDataPath()
        {
            return SettingsData.poolerResourcesDataPath;
        }

        protected override void PostLoadRun()
        {
            Data.Run();
        }

        protected override void Clear()
        {
            Data?.Clear();
        }

        public static void TryAddPool(PoolData poolToAdd)
        {
            Instance.Data.TryAddPool(poolToAdd);
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
        /// Spawns GameObject from pool with specified tag, then calls all OnSpawn methods in it
        /// </summary>
        /// <param name="poolTag">pool tag with necessary object</param>
        /// <param name="position">initial position</param>
        /// <param name="rotation">initial rotation</param>
        /// <param name="parent">parent transform for GameObject</param>
        /// <param name="data">data to provide in GameObject</param>
        /// <returns>GameObject from pool</returns>
        public static GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null, Action<GameObject> spawnAction = null)
        {
            return Instance.Data.Spawn(poolTag, position, rotation, parent, data, spawnAction);
        }

        /// <summary>
        /// Alternative to Unity's Instantiate method, that automatically injects object
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <returns>Instantiated GameObject</returns>
        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Instance.Data.Instantiate(prefab, position, rotation, parent);
        }
        
        /// <summary>
        /// Alternative to Unity's Instantiate method, which uses Vector3.zero, Quaternion.identity and automatically injects object
        /// </summary>
        /// <param name="prefab">Prefab to Instantiate</param>
        /// <param name="parent">Parent object to instantiated GameObject</param>
        /// <returns>Instantiated GameObject</returns>
        public static GameObject Instantiate(GameObject prefab, Transform parent = null)
        {
            return Instance.Data.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
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