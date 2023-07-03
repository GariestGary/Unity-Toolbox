using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [Serializable]
    public class PoolerDataHolder : ScriptableObject, IRunner, IClear
    {
        [SerializeField] private List<PoolData> poolsList;

        private Transform objectPoolParent;
        private List<Pool> pools = new List<Pool>();

        public void Run()
        {
            pools = new List<Pool>();

            foreach (var t in poolsList)
            {
                TryAddPool(t);
            }

            Messenger.Subscribe<SceneUnloadingMessage>(m => HandleSceneUnload(m.SceneName));
        }

        public void SetPoolParent(Transform parent)
        {
            objectPoolParent = parent;
        }

        public void TryAddPool(PoolData poolToAdd)
        {
            if (pools.Any(x => x.tag == poolToAdd.tag))
            {
                Debug.LogWarning("Pool with tag " + poolToAdd.tag + " already exist's");
                return;
            }

            LinkedList<PooledGameObject> objectPoolList = new LinkedList<PooledGameObject>();

            if(poolToAdd.initialSize <= 0)
            {
                poolToAdd.initialSize = 1;
            }

            for (int j = 0; j < poolToAdd.initialSize; j++)
            {
                CreateNewPoolObject(poolToAdd.pooledObject, objectPoolList);
            }

            pools.Add(new Pool(poolToAdd.tag, objectPoolList));

        }

        public void TryAddPool(string tag, GameObject obj, int size)
        {
            PoolData pool = new PoolData() { tag = tag, pooledObject = obj, initialSize = size };

            TryAddPool(pool);
        }

        public void TryRemovePool(string tag)
        {
            var poolToRemove = pools.FirstOrDefault(p => p.tag == tag);

            if (poolToRemove == null) return;

            foreach (var obj in poolToRemove.objects)
            {
                TryDespawn(obj);
                Destroy(obj.GameObject);
            }

            pools.Remove(poolToRemove);
        }
        
        public GameObject Spawn(string poolTag, Vector3 position, Quaternion rotation, Transform parent = null, object data = null, Action<GameObject> spawnAction = null)
        {
            //Returns null if object pool with specified tag doesn't exists
            if (!pools.Any(x => x.tag == poolTag))
            {
                Debug.LogWarning("Object pool with tag " + poolTag + " doesn't exists");
                return null;
            }

            Pool p = pools.First(x => x.tag == poolTag);

            //Create new object if last in list is active
            if (p.objects.Last.Value.Used)
            {
                CreateNewPoolObject(p.objects.Last.Value.GameObject, p.objects);
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

            //Setting transform
            obj.GameObject.transform.position = position;
            obj.GameObject.transform.rotation = rotation;
            obj.GameObject.transform.SetParent(parent);
            obj.GameObject.Enable();

            //Call all spawn methods in gameobject
            CallSpawns(obj.GameObject, data);

            //Add object back to start
            p.objects.AddFirst(obj);

            if (spawnAction != null)
            {
                spawnAction.Invoke(obj.GameObject);
            }

            obj.Used = true;
            
            return obj.GameObject;
        }
        
        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject inst = GameObject.Instantiate(prefab, position, rotation, parent);

            Updater.InitializeObject(inst);

            return inst;
        }
        
        public GameObject Instantiate(GameObject prefab, Transform parent = null)
        {
            return Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
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
            //Setting transform
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(parent);
            obj.Enable();

            //Call all spawn methods in gameobject
            CallSpawns(obj, data);

            return obj;
        }
        
        public bool TryDespawn(GameObject objectToDespawn, float delay = 0)
        {
            if (objectToDespawn == null)
            {
                return false;
            }

            Pool p = pools.FirstOrDefault(x => x.objects.Any(g => g.GameObject == objectToDespawn));

            PooledGameObject pgo = null;
            
            if (p == null)
            {
                return false;
            }
            else
            {
                pgo = p.objects.FirstOrDefault(x => x.GameObject == objectToDespawn);
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
                pgo.Used = false;
                ReturnToPool(pgo.GameObject);
            }
            else
            {
                CoroutineStarter.Instance.StartCoroutine(DespawnCoroutine(pgo, delay));
            }

            return true;
        }

        public void DespawnOrDestroy(GameObject obj, float delay = 0)
        {
            if (!TryDespawn(obj, delay))
            {
                if (delay > 0)
                {
                    CoroutineStarter.Instance.StartCoroutine(DestroyCoroutine(obj, delay));
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

            Updater.InitializeObject(poolObj);

            poolObj.Disable();

            PooledGameObject pgo = new PooledGameObject()
            {
                GameObject = poolObj,
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

        private IEnumerator DespawnCoroutine(PooledGameObject objectToDespawn, float delay)
        {
            yield return new WaitForSeconds(delay);

            objectToDespawn.Used = false;

            ReturnToPool(objectToDespawn.GameObject);
        }

        private void ReturnToPool(GameObject obj)
        {
            obj.Disable();
            obj.transform.SetParent(objectPoolParent);
        }
        
        private void HandleSceneUnload(string unloadedSceneName)
        {
            //TODO: cache objects at spawn into dictionary with scene name key and destroy from correlated list
            //what if after spawning and caching scene, i move object to other scene, object will stay in old list, correlated to old previous scene
            
            for (int i = 0; i < pools.Count; i++)
            {
                foreach (var obj in pools[i].objects)
                {
                    if (obj.GameObject.scene.name == unloadedSceneName && obj.Used)
                    {
                        TryDespawn(obj);
                    }
                }
            }
        }

        //TODO: separate pool component with self objects, that destroys when scene unloads

        private sealed class Pool
        {
            public string tag;
            public LinkedList<PooledGameObject> objects;

            public Pool(string tag, LinkedList<PooledGameObject> objects = null)
            {
                this.tag = tag;
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

        private sealed class PooledGameObject
        {
            public GameObject GameObject;
            public bool Used;
        }

        public void Clear()
        {
            objectPoolParent = null;
            pools.Clear();
            pools = null;
        }
    }

    [System.Serializable]
    public class PoolData
    {
        public string tag;
        public GameObject pooledObject;
        public int initialSize;
    }
}


