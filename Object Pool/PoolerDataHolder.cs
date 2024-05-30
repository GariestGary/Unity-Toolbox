using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [Serializable]
    public class PoolerDataHolder : ScriptableObject, IRunner, IClear
    {
        [SerializeField] private float m_GarbageCollectorWorkInterval = 10;
        [SerializeField] private List<PoolData> poolsList;

        private Transform objectPoolParent;
        private List<Pool> pools = new List<Pool>();
        private GameObjectRemovedMessage _removeMessage;

        public List<PoolData> PoolsList => poolsList;

        private CancellationTokenSource m_GCTokenSource = new CancellationTokenSource();

        public void Run()
        {
            objectPoolParent = new GameObject("Pool Parent").transform;

            pools = new List<Pool>();

            for (int i = 0; i < poolsList.Count; i++)
            {
                TryAddPool(poolsList[i]);
            }

            Messenger.Subscribe<SceneUnloadingMessage>(m => HandleSceneUnload(m.SceneName), null, true);

            _removeMessage = new GameObjectRemovedMessage();

            EnableGC();
        }

        #region Garbage Collector

        private async UniTask GCWorkerStart(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(m_GarbageCollectorWorkInterval)).AttachExternalCancellation(token);

                if(token.IsCancellationRequested)
                {
                    return;
                }

                ForceGarbageCollector();
            }
        }
        private void GCWorkerStop()
        {
            m_GCTokenSource = m_GCTokenSource.CancelAndCreate();
        }

        public void EnableGC()
        {
            GCWorkerStart(m_GCTokenSource.Token).Forget();
        }

        public void DisableGC()
        {
            GCWorkerStop();
        }

        public void ForceGarbageCollector()
        {
            if(pools != null)
            {
                for (int i = 0; i < pools.Count; i++)
                {
                    ClearPoolGarbage(pools[i]);
                }
            }
        }
        private void ClearPoolGarbage(Pool pool)
        {
            var unusedObjects = pool.objects.Where(o => !o.Used).ToList();

            if(unusedObjects.Count <= 0)
            {
                return;
            }

            var allObjectsCount = pool.objects.Count;
            var excessObjectsCount = allObjectsCount - pool.size;
            var canBeCleared = excessObjectsCount - (unusedObjects.Count - pool.size) >= 0;
            var usedObjects = allObjectsCount - unusedObjects.Count;
            excessObjectsCount -= usedObjects;

            if (canBeCleared)
            {
                for(int i = 0; i < excessObjectsCount; i++) 
                {
                    var unused = unusedObjects[i];
                    pool.objects.Remove(unused);
                    Destroy(unused.GameObject);
                    _removeMessage.Obj = unused.GameObject;
                    _removeMessage.RemoveType = GameObjectRemoveType.Destroyed;
                    Messenger.Send(_removeMessage);
                }
            }
        }

        #endregion

        public bool TryRemovePool(Pool pool)
        {
            if (pools == null)
            {
                return false;
            }

            if(!pools.Contains(pool))
            {
                return false;
            }

            for (int i = 0; i < pool.objects.Count; i++)
            {
                var obj = pool.objects[i];

                TryDespawn(obj);
                Destroy(obj.GameObject);
            }

            pools.Remove(pool);
            return true;
        }

        public bool TryRemovePool(string tag)
        {
            Pool poolToRemove = null;

            for (int i = 0; i < pools.Count; i++)
            {
                var pool = pools[i];

                if (pool.tag == tag)
                {
                    poolToRemove = pool;
                    break;
                }
            }

            if (poolToRemove == null)
            {
                Debug.LogWarning($"There is no pool named {tag}");
                return false;
            }

            return TryRemovePool(poolToRemove);
        }
        
        public Pool TryAddPool(PoolData poolToAdd)
        {
            if(poolToAdd.pooledObject == null)
            {
                Debug.LogWarning($"Pool with tag {poolToAdd.tag} has no prefab setted");
                return null;
            }

            if(pools == null)
            {
                pools = new List<Pool>();
            }

            List<PooledGameObject> objectPoolList = new List<PooledGameObject>();

            if(poolToAdd.size <= 0)
            {
                poolToAdd.size = 1;
            }

            for (int j = 0; j < poolToAdd.size; j++)
            {
                CreateNewPoolObject(poolToAdd.pooledObject, objectPoolList);
            }

            var pool = new Pool(poolToAdd.tag, poolToAdd.pooledObject, poolToAdd.size, objectPoolList);
            pools.Add(pool);
            return pool;
        }

        public Pool TryAddPool(string tag, GameObject obj, int size)
        {
            PoolData pool = new PoolData() { tag = tag, pooledObject = obj, size = size };
            return TryAddPool(pool);
        }

        #region Instantiating
        public GameObject Spawn(string poolTag, Transform parent = null, object data = null, Action<GameObject> spawnAction = null)
        {
            return Spawn(poolTag, Vector3.zero, Quaternion.identity, parent, data, spawnAction);
        }
        
        public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null, Action<GameObject> spawnAction = null)
        {
            //Returns null if object pool with specified tag doesn't exists
            var poolsToUse = pools.Where(p => p.tag == poolTag).ToArray();

            if(poolsToUse.Length <= 0)
            {
                Debug.LogWarning($"Object pool with tag '{poolTag}' doesn't exists");
                return null;
            }

            //get first unused obj
            var poolToUse = poolsToUse[UnityEngine.Random.Range(0, poolsToUse.Length)];
            var objToSpawn = poolToUse.objects.FirstOrDefault(o => !o.Used);

            //Create new object if last in list is active
            if (objToSpawn is null)
            {
                CreateNewPoolObject(poolToUse.referenceObject, poolToUse.objects);
                objToSpawn = poolToUse.objects.FirstOrDefault(o => !o.Used);
            }

            //Return null if last object is null;
            if (objToSpawn == null)
            {
                Debug.Log($"object from pool '{poolTag}' you trying to spawn is null");
                return null;
            }

            //Setting initial name
            objToSpawn.GameObject.name = poolToUse.referenceObject.name;

            //Setting transform
            var t = objToSpawn.GameObject.transform;
            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.position = position;
            t.rotation = rotation;
            objToSpawn.GameObject.Enable();

            //Call all spawn methods in gameobject
            CallSpawns(objToSpawn.GameObject, data);

            if (spawnAction != null)
            {
                spawnAction.Invoke(objToSpawn.GameObject);
            }

            objToSpawn.Used = true;
            
            return objToSpawn.GameObject;
        }
        
        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject inst = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);

            Updater.InitializeObject(inst);

            return inst;
        }
        
        public GameObject Instantiate(GameObject prefab, Transform parent = null)
        {
            return Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        }
        
        private GameObject CreateNewPoolObject(GameObject obj, List<PooledGameObject> poolQueue, bool addToPoolParent = true)
        {
            Transform poolParent = null;

            if(addToPoolParent)
            {
                poolParent = objectPoolParent;
            }

            GameObject poolObj = Instantiate(obj, poolParent);

            poolObj.name = obj.name;

            Updater.InitializeObject(poolObj);

            poolObj.Disable();

            PooledGameObject pgo = new PooledGameObject()
            {
                GameObject = poolObj,
                Used = false
            };
            
            poolQueue.Add(pgo);

            return poolObj;
        }

        #endregion

        #region Reflection

        private void CallSpawns(GameObject obj, object data) 
        {
            MonoCached[] pooledMono = obj.GetComponentsInChildren<MonoCached>(true).Where(o => o is IPooledBase).ToArray();

            for (int i = 0; i < pooledMono.Length; i++)
            {
                var mono = pooledMono[i];

                if (mono == null)
                {
                    continue;
                }

                var type = mono.GetType();
                var interfaces = type.GetInterfaces();

                for (int j = 0; j < interfaces.Length; j++)
                {
                    var inter = interfaces[j];

                    if (inter.GetInterface("IPooledBase") != null)
                    {
                        var generic = inter.GetGenericArguments()[0];
                        var onSpawnMethod = inter.GetMethod("OnSpawn");
                        onSpawnMethod.Invoke(mono, new object[] { Convert.ChangeType(data, generic) });
                    }
                }
            }
        }

        private void CallDespawns(GameObject obj)
        {
            IDespawn[] despawns = obj.GetComponentsInChildren<IDespawn>(true);

            for(int i = 0; i < despawns.Length; i++)
            {
                var despawnable = despawns[i];

                if(despawnable != null)
                {
                    despawnable.OnDespawn();
                }
            }
        }

        #endregion

        #region Destroying

        public bool TryDespawn(GameObject objectToDespawn)
        {
            if (objectToDespawn == null)
            {
                return false;
            }

            Pool p = null;
            PooledGameObject pgo = null;

            for (int i = 0; i < pools.Count; i++)
            {
                bool found = false;

                for (int j = 0; j < pools[i].objects.Count; j++)
                {
                    var objToCheck = pools[i].objects[j];

                    if (objToCheck.GameObject == objectToDespawn)
                    {
                        pgo = objToCheck;
                        found = true;
                        TraversePooledObjectAndDespawnOtherPooledObjects(objectToDespawn.transform);
                        break;
                    }
                }

                if(found)
                {
                    p = pools[i];
                    break;
                }
            }

            
            if (p == null)
            {
                return false;
            }

            return TryDespawn(pgo);
        }

        private void TraversePooledObjectAndDespawnOtherPooledObjects(Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                TraversePooledObjectAndDespawnOtherPooledObjects(transform.GetChild(i));
                TryDespawn(transform.GetChild(i).gameObject);
            }
        }

        private bool TryDespawn(PooledGameObject pgo)
        {
            if (pgo == null || !pgo.Used)
            {
                return true;
            }

            CallDespawns(pgo.GameObject);

            pgo.Used = false;
            ReturnToPool(pgo.GameObject);

            _removeMessage.Obj = pgo.GameObject;
            _removeMessage.RemoveType = GameObjectRemoveType.Despawned;
            Messenger.Send(_removeMessage);

            return true;
        }

        public bool DespawnOrDestroy(GameObject obj)
        {
            var despawned = TryDespawn(obj);

            if (!despawned)
            {
                Destroy(obj);

                _removeMessage.Obj = obj;
                _removeMessage.RemoveType = GameObjectRemoveType.Destroyed;
                Messenger.Send(_removeMessage);

                return true;
            }

            return despawned;
        }
        
        private void HandleSceneUnload(string unloadedSceneName)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                for (int j = 0; j < pools[i].objects.Count; j++)
                {
                    var obj = pools[i].objects[j];

                    if (obj.GameObject.scene.name == unloadedSceneName && obj.Used)
                    {
                        TryDespawn(obj);
                    }
                }
            }
        }
        
        private void ReturnToPool(GameObject obj)
        {
            obj.Disable();
            obj.transform.SetParent(objectPoolParent);
        }
        
        #endregion

        public int GetPoolObjectsCount(string poolTag)
        {
            var allPools = pools.Where(p => p.tag == poolTag);

            if(allPools.Count() <= 0)
            {
                Debug.LogWarning($"There is no pool named {poolTag}");
                return -1;
            }

            return allPools.Sum(p => p.CurrentObjectsCount);
        }

        public void Clear()
        {
            objectPoolParent = null;
            pools?.Clear();
            pools = null;
        }
    }

    [System.Serializable]
    public class PoolData
    {
        public string tag;
        public GameObject pooledObject;
        public int size;
    }

    public sealed class Pool
    {
        public string tag;
        public int size;
        public GameObject referenceObject;
        public List<PooledGameObject> objects;

        public int CurrentObjectsCount => objects.Count;

        public Pool(string tag, GameObject referenceObject, int size, List<PooledGameObject> objects = null)
        {
            this.tag = tag;
            this.size = size;
            this.referenceObject = referenceObject;

            if(objects == null)
            {
                this.objects = new List<PooledGameObject>();
            }
            else
            {
                this.objects = objects;
            }
        }
    }

    public sealed class PooledGameObject
        {
            public GameObject GameObject;
            public bool Used;
        }
    
    public class GameObjectRemovedMessage: Message
    {
        public GameObject Obj;
        public GameObjectRemoveType RemoveType;
    }

    public enum GameObjectRemoveType
    {
        Destroyed,
        Despawned
    }
}



