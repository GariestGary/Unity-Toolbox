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
            msg.Subscribe(Message.SCENE_UNLOADING, _ => ClearPools());
            
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

            LinkedList<GameObject> objectPoolList = new LinkedList<GameObject>();

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
                    Despawn(obj);
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
        public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null)
        {
            //Returns null if object pool with specified tag doesn't exists
            if (!pools.Any(x => x.tag == poolTag))
            {
                Debug.LogWarning("Object pool with tag " + poolTag + " doesn't exists");
                return null;
            }

            Pool p = pools.Where(x => x.tag == poolTag).First();

            //Create new object if last in list is active
            if (p.objects.Last.Value.activeSelf)
            {
                CreateNewPoolObject(p.objects.Last.Value, p.objects);
            }

            //Take last object
            GameObject obj = p.objects.Last.Value;
            p.objects.RemoveLast();

            //Return null if last object is null;
            if (obj == null)
            {
                Debug.Log("object from pool " + poolTag + " you trying to spawn is null");
                return null;
            }

            resolver?.Inject(obj);

            //Setting transform
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            EnableGameObject(obj);

            //Call all spawn methods in gameobject
            CallSpawns(obj, data);

            //Add object back to start
            p.objects.AddFirst(obj);
            return obj;
        }

        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject inst = GameObject.Instantiate(prefab, position, rotation, parent);
 
            updater.InitializeObject(inst);
            EnableGameObject(inst);

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
            EnableGameObject(obj);

            resolver.Inject(obj);

            //Setting transform
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            EnableGameObject(obj);

            //Call all spawn methods in gameobject
            CallSpawns(obj, data);

            return obj;
        }

        /// <summary>
        /// Removes GameObject from scene and returns it to pool
        /// </summary>
        /// <param name="objectToDespawn">object to despawn</param>
        /// <param name="delay">delay before despawning</param>
        public void Despawn(GameObject objectToDespawn, float delay = 0)
        {
            if (objectToDespawn == null)
            {
                return;
            }

            if (delay < 0) delay = 0;

            if (delay == 0)
            {
                ReturnToPool(objectToDespawn);
            }
            else
            {
                StartCoroutine(DespawnCoroutine(objectToDespawn, delay));
            }
        }

        /// <summary>
        /// Analogue to Unity's GameObject disable
        /// </summary>
        /// <param name="obj"></param>
        public void DisableGameObject(GameObject obj)
        {
            obj.GetComponentsInChildren<MonoCached>()
            .ToList()
            .ForEach(x => x.Deactivate());
            obj.SetActive(false);
        }

        /// <summary>
        /// Analogue to Unity's GameObject enable
        /// </summary>
        /// <param name="obj"></param>
        public void EnableGameObject(GameObject obj)
        {
            obj.GetComponentsInChildren<MonoCached>()
            .ToList()
            .ForEach(x => x.Activate());
            obj.SetActive(true);
        }

        private GameObject CreateNewPoolObject(GameObject obj, LinkedList<GameObject> pool)
        {
            GameObject poolObj = Instantiate(obj, objectPoolParent);

            updater?.InitializeObject(poolObj);
            resolver?.Inject(poolObj);

            DisableGameObject(poolObj);
            pool.AddLast(poolObj);

            return poolObj;
        }

        private IEnumerator DespawnCoroutine(GameObject objectToDespawn, float delay)
        {
            yield return new WaitForSeconds(delay);

            ReturnToPool(objectToDespawn);
        }

        private void ReturnToPool(GameObject obj)
        {
            DisableGameObject(obj);

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
                    Despawn(obj);    
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
                    Despawn(obj);
                    Destroy(obj);
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
    public LinkedList<GameObject> objects;

    public Pool(string tag, bool destroyOnLevelChange, LinkedList<GameObject> objects = null)
    {
        this.tag = tag;
        this.destroyOnLevelChange = destroyOnLevelChange;
        if(objects == null)
        {
            this.objects = new LinkedList<GameObject>();
        }
        else
        {
            this.objects = objects;
        }
    }
}
