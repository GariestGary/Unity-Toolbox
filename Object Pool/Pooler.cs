using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Pooler : Singleton<Pooler>, IRunner
    {
        [SerializeField] private List<PoolData> poolsList;

        private Transform objectPoolParent;
        private List<Pool> pools = new List<Pool>();

        [Inject] private Updater updater;
        [Inject] private Resolver resolver;
        [Inject] private Messager msg;

        public void Run()
        {
            //TODO: msg.Subscribe(Message.SCENE_UNLOADING, _ => ClearPools());
            
            pools = new List<Pool>();

            objectPoolParent = new GameObject().transform;
            objectPoolParent.name = "Pool Parent";

            foreach (var t in poolsList)
            {
                AddPool(t);
            }

            //TODO: clear pools on level change
        }

        public void AddPool(PoolData poolToAdd)
        {
            if (pools.Any(x => x.tag == poolToAdd.tag))
            {
                Debug.LogWarning("Pool with tag " + poolToAdd.tag + " already exist's");
                return;
            }

            LinkedList<PooledGameObject> objectPoolList = new LinkedList<PooledGameObject>();

            for (int j = 0; j < poolToAdd.initialSize; j++)
            {
                CreateNewPoolObject(poolToAdd.pooledObject, objectPoolList);
            }

            pools.Add(new Pool(poolToAdd.tag, poolToAdd.destroyOnLevelChange, objectPoolList));

        }

        public void ClearPools()
        {
            foreach (var pool in pools)
            {
                foreach (var obj in pool.objects)
                {
                    TryDespawn(obj);
                }
            }
        }

        public void AddPool(string tag, GameObject obj, int size, bool destroyOnLevelChange = true)
        {
            PoolData pool = new PoolData() { tag = tag, pooledObject = obj, initialSize = size, destroyOnLevelChange = destroyOnLevelChange };

            AddPool(pool);
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
        public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null, Action<GameObject> spawnAction = null)
        {
            //Returns null if object pool with specified tag doesn't exists
            if (!pools.Any(x => x.tag == poolTag))
            {
                Debug.LogWarning("Object pool with tag " + poolTag + " doesn't exists");
                return null;
            }

            Pool p = pools.Where(x => x.tag == poolTag).First();

            //Create new object if last in list is active
            if (p.objects.Last.Value.Used)
            {
                CreateNewPoolObject(p.objects.Last.Value.gameObject, p.objects);
            }

            //Take last object
            PooledGameObject obj = p.objects.Last.Value;
            p.objects.RemoveLast();

            //Return null if last object is null;
            if (obj == null)
            {
                Debug.Log("object from pool " + poolTag + " you trying to spawn is null");
                return null;
            }

            resolver?.Inject(obj);

            //Setting transform
            obj.gameObject.transform.position = position;
            obj.gameObject.transform.rotation = rotation;
            obj.gameObject.transform.SetParent(parent);
            obj.gameObject.Enable();

            //Call all spawn methods in gameobject
            CallSpawns(obj.gameObject, data);

            //Add object back to start
            p.objects.AddFirst(obj);

            if (spawnAction != null)
            {
                spawnAction.Invoke(obj.gameObject);
            }

            obj.Used = true;
            
            return obj.gameObject;
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool addToProcess = true)
        {
            GameObject inst = GameObject.Instantiate(prefab, position, rotation, parent);
        
            Resolver.Instance.SearchObjectBindings(inst);

            if (addToProcess)
            {
                updater.InitializeObject(inst);
                inst.Enable();
            }

            return inst;
        }

        private void CallSpawns(GameObject obj, object data)
        {
            IPooled[] pooled = obj.GetComponentsInChildren<IPooled>();

            foreach (var t in pooled)
            {
                if(t != null)
                {
                    t.OnSpawn(data);
                }
            }
        }

        public GameObject ManualSpawn(GameObject obj, Vector3 position, Quaternion rotation, Transform parent = null, object data = null)
        {
            resolver.Inject(obj);

            //Setting transform
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            obj.Enable();

            //Call all spawn methods in gameobject
            CallSpawns(obj, data);

            return obj;
        }

        /// <summary>
        /// Removes GameObject from scene and returns it to pool
        /// </summary>
        /// <param name="objectToDespawn">object to despawn</param>
        /// <param name="delay">delay before despawning</param>
        public bool TryDespawn(GameObject objectToDespawn, float delay = 0)
        {
            if (objectToDespawn == null)
            {
                return false;
            }

            Pool p = pools.FirstOrDefault(x => x.objects.Any(g => g.gameObject == objectToDespawn));

            PooledGameObject pgo = null;
            
            if (p == null)
            {
                return false;
            }
            else
            {
                pgo = p.objects.FirstOrDefault(x => x.gameObject == objectToDespawn);
            }

            return TryDespawn(pgo, delay);
        }

        private bool TryDespawn(PooledGameObject pgo, float delay = 0)
        {
            if (pgo == null || !pgo.Used)
            {
                return true;
            }

            if (delay < 0) delay = 0;

            if (delay == 0)
            {
                ReturnToPool(pgo.gameObject);
            }
            else
            {
                StartCoroutine(DespawnCoroutine(pgo.gameObject, delay));
            }

            return true;
        }

        public void DespawnOrDestroy(GameObject obj, float delay = 0)
        {
            if (!TryDespawn(obj, delay))
            {
                if (delay > 0)
                {
                    StartCoroutine(DestroyCoroutine(obj, delay));
                }
                else
                {
                    Destroy(obj);
                }
            }
        }

        private GameObject CreateNewPoolObject(GameObject obj, LinkedList<PooledGameObject> pool)
        {
            GameObject poolObj = Instantiate(obj, objectPoolParent);

            updater?.InitializeObject(poolObj);
            resolver?.Inject(poolObj);

            poolObj.Disable();

            PooledGameObject pgo = new PooledGameObject()
            {
                gameObject = poolObj,
                Used = false
            };
            
            pool.AddLast(pgo);

            return poolObj;
        }

        private IEnumerator DestroyCoroutine(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);

            Destroy(obj);
        }

        private IEnumerator DespawnCoroutine(GameObject objectToDespawn, float delay)
        {
            yield return new WaitForSeconds(delay);

            ReturnToPool(objectToDespawn);
        }

        private void ReturnToPool(GameObject obj)
        {
            obj.Disable();

            obj.transform.SetParent(objectPoolParent);
        }

        private void HandleLevelChange()
        {
            ResolveObjectsLinks();
            ClearTemporaryPools();
        }

        private void ResolveObjectsLinks()
        {
            //TODO: maybe it's better to simply change active pool objects parent to null, than despawning all
            foreach (var pool in pools)
            {
                foreach (var obj in pool.objects)
                {
                    TryDespawn(obj.gameObject);    
                }
            }
        }

        private void ClearTemporaryPools()
        {
            Debug.Log("Cleaning pools");

            Pool[] poolsToClear = pools.Where(x => x.destroyOnLevelChange).ToArray();

            Debug.Log("Pools to clear count: " + poolsToClear.Length);

            foreach (var pool in poolsToClear)
            {
                foreach (var obj in pool.objects)
                {
                    TryDespawn(obj.gameObject);
                    Destroy(obj.gameObject);
                }

                pools.Remove(pool);
            }
        }
    }
}

[System.Serializable]
public class PoolData
{
    public string tag;
    public GameObject pooledObject;
    public int initialSize;
    public bool destroyOnLevelChange;
}

public class Pool
{
    public string tag;
    public bool destroyOnLevelChange;
    public LinkedList<PooledGameObject> objects;

    public Pool(string tag, bool destroyOnLevelChange, LinkedList<PooledGameObject> objects = null)
    {
        this.tag = tag;
        this.destroyOnLevelChange = destroyOnLevelChange;
        if(objects == null)
        {
            this.objects = new LinkedList<PooledGameObject>();
        }
        else
        {
            this.objects = objects;
        }
    }
}

public class PooledGameObject
{
    public GameObject gameObject;
    public bool Used;
}
