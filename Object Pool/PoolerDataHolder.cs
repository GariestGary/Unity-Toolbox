using Cysharp.Threading.Tasks;
using NaughtyAttributes;
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
        private GameObjectRemovedMessage _removeMessage;

        public void Run()
        {
#pragma warning disable
            RunAsync();
#pragma warning enable
        }

        private async UniTask RunAsync()
        {
            objectPoolParent = new GameObject("Pool Parent").transform;

            pools = new List<Pool>();

            foreach (var t in poolsList)
            {
                TryAddPool(t);
            }

            Messenger.Subscribe<SceneUnloadingMessage>(m => HandleSceneUnload(m.SceneName), null, true);

            _removeMessage = new GameObjectRemovedMessage();
        }

        public void TryAddPool(PoolData poolToAdd)
        {
            if(poolToAdd.pooledObject == null)
            {
                Debug.LogWarning($"Pool with tag {poolToAdd.tag} has no prefab setted");
                return;
            }

            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].tag == poolToAdd.tag)
                {
                    Debug.LogWarning("Pool with tag " + poolToAdd.tag + " already exist's");
                    return;
                }
            }

            List<PooledGameObject> objectPoolList = new List<PooledGameObject>();

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
            Pool poolToRemove = null;// pools.FirstOrDefault(p => p.tag == tag);

            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].tag == tag)
                {
                    poolToRemove = pools[i];
                    break;
                }
            }

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
            Pool poolToUse = null;

            for (int i = 0; i < pools.Count; i++)
            {
                if (pools[i].tag.Equals(poolTag))
                {
                    poolToUse = pools[i];
                    break;
                }
            }

            if(poolToUse == null)
            {
                Debug.LogWarning("Object pool with tag " + poolTag + " doesn't exists");
                return null;
            }
            
            var objToSpawn = poolToUse.objects.Where(o => !o.Used).FirstOrDefault();

            //Create new object if last in list is active
            if (objToSpawn is null)
            {
                CreateNewPoolObject(poolToUse.objects[0].GameObject, poolToUse.objects);
                objToSpawn = poolToUse.objects.Where(o => !o.Used).FirstOrDefault();
            }

            //Return null if last object is null;
            if (objToSpawn == null)
            {
                Debug.Log("object from pool " + poolTag + " you trying to spawn is null");
                return null;
            }

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

            //Add object back to start
            poolToUse.objects.Add(objToSpawn);

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
                    var objToCheck = pools[i].objects.ElementAt(j);

                    if (objToCheck.GameObject == objectToDespawn)
                    {
                        pgo = objToCheck;
                        found = true;
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

        private bool TryDespawn(PooledGameObject pgo)
        {
            if (pgo == null || !pgo.Used)
            {
                return true;
            }

            pgo.Used = false;
            ReturnToPool(pgo.GameObject);

            _removeMessage.Obj = pgo.GameObject;
            _removeMessage.RemoveType = GameObjectRemoveType.Despawned;
            Messenger.Send(_removeMessage);

            return true;
        }

        public void DespawnOrDestroy(GameObject obj)
        {
            if (!TryDespawn(obj))
            {
                Destroy(obj);

                _removeMessage.Obj = obj;
                _removeMessage.RemoveType = GameObjectRemoveType.Destroyed;
                Messenger.Send(_removeMessage);
            }
        }

        private GameObject CreateNewPoolObject(GameObject obj, List<PooledGameObject> pool)
        {
            GameObject poolObj = Instantiate(obj, objectPoolParent);

            Updater.InitializeObject(poolObj);

            poolObj.Disable();

            PooledGameObject pgo = new PooledGameObject()
            {
                GameObject = poolObj,
                Used = false
            };
            
            pool.Add(pgo);

            return poolObj;
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

        private sealed class Pool
        {
            public string tag;
            public List<PooledGameObject> objects;

            public Pool(string tag, List<PooledGameObject> objects = null)
            {
                this.tag = tag;
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



