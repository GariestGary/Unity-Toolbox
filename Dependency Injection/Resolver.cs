using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace VolumeBox.Toolbox
{
    public class Resolver : ToolWrapper<Resolver>
    {
        [SerializeField] private GameObject instancesRoot;
        
        private static List<object> _instances;
        private static Type _injectAttributeType;
        private static List<SceneBinding> _currentSceneInstances;

        protected override void Run()
        {
            _injectAttributeType = typeof(InjectAttribute);

            _instances = new List<object>();
            _currentSceneInstances = new List<SceneBinding>();

            Messenger.SubscribeKeeping<SceneBindingMessage>(x => _currentSceneInstances = x.instances.ToList());
            Messenger.SubscribeKeeping<SceneUnloadedMessage>(x => _currentSceneInstances.RemoveAll(y => y.sceneName == x.SceneName));
            
            AddMonoInstancesFromRoot(instancesRoot);
        }

        private void AddMonoInstancesFromRoot(GameObject root)
        {
            if(root != null)
            {
                var newInstances = root.GetComponents<MonoBehaviour>().ToList();
                
                foreach (var instance in newInstances)
                {
                    _instances.Add(instance);
                }
            }
        }

        public static void AddBindingsFromObject(GameObject obj)
        {
            foreach (var cb in obj.GetComponentsInChildren<ComponentBinding>(true))
            {
                if (cb.Context != null)
                {
                    SceneBinding newBind = new SceneBinding() { Instance = cb.Context, id = cb.Id };
                    
                    if (cb.ThisSceneOnly)
                    {
                        _currentSceneInstances.Add(newBind);
                    }
                    else
                    {
                        _instances.Add(newBind);
                    }
                    
                }
            }
        }

        public static void InjectInstances()
        {
            foreach(var instance in _instances)
            {
                Inject(instance);
            }
        }

        public static void AddInstance(object instance)
        {
            if(_instances.Any(x => x.GetType() == instance.GetType())) 
            {
                Debug.LogWarning($"Resolver already contains {instance.GetType()}");
            }
            else
            {
                _instances.Add(instance);
            }
        }

        public static void RemoveInstance(object instance)
        {
            if(_instances.Contains(instance))
            {
                _instances.Remove(instance);
            }
            else
            {
                Debug.LogWarning($"Resolver doesn't contain {instance.GetType()}");
            }
        }
    
        public static void Inject(GameObject obj)
        {
            //Getting all monos from gameobject
            Component[] monosToInject = obj.GetComponentsInChildren<Component>(true);

            foreach(var mono in monosToInject)
            {
                Inject(mono);
            }
        }

        public static void Inject(object obj)
        {
            if(obj == null) return;

            if (obj is GameObject)
            {
                Inject(obj as GameObject);
                return;
            }

            var c = obj.GetType();

            if(c.IsClass)
            {
                var fields = c.GetRuntimeFields();
            
                if(fields.Count() > 0)
                {
                    fields = fields
                    .Where(f => f.GetCustomAttributes(_injectAttributeType, true).Any());
                }

                foreach(var field in fields)
                {
                    string id = "";

                    if (_currentSceneInstances.Count > 0)
                    {
                        id = field.GetCustomAttribute<InjectAttribute>(true).ID;
                    }

                    ResolveField(field, obj, id);
                }
            }
        }

        public static T GetInstance<T>()
        {
            return (T)_instances.Where(i => i is T).FirstOrDefault();
        }

        private static void ResolveField(FieldInfo fieldInfo, object owner, string id = "")
        {
            //getting instance which type equals to required
            var instance = _instances.Where(x => x.GetType() == fieldInfo.FieldType || fieldInfo.FieldType.IsInstanceOfType(x))
                .FirstOrDefault();

            if (instance == null)
            {
                SceneBinding binding;
                
                if (string.IsNullOrEmpty(id))
                {
                    binding = _currentSceneInstances.Where(x => x.Instance.GetType() == fieldInfo.FieldType).FirstOrDefault();
                }
                else
                {
                    binding = _currentSceneInstances.Where(x => x.id == id && x.Instance.GetType() == fieldInfo.FieldType).FirstOrDefault();
                }

                if (binding != null)
                {
                    instance = binding.Instance;
                }
            }

            if (instance == null)
            {
                string message = $"Instance of {fieldInfo.FieldType}";
                message += string.IsNullOrEmpty(id) ? "" : $" with tag '{id}'";
                Debug.LogError( message + " doesn't binded");
                return;
            }

            try
            {
                fieldInfo.SetValue(owner, instance);
            }
            catch
            {
                Debug.LogError($"Failed converting {instance.GetType()} to {fieldInfo.FieldType}");
            }
        }

        protected override void Clear()
        {
            _instances.Clear();
            _instances = null;
            _currentSceneInstances.Clear();
            _currentSceneInstances = null;
        }
    }

    [System.Serializable]
    public class SceneBinding
    {
        public string sceneName;
        public object Instance;
        public string id;
    }

    [Serializable]
    public class SceneBindingMessage: Message
    {
        public List<SceneBinding> instances = new List<SceneBinding>();
    }
}
