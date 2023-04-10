using System.Reflection;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace VolumeBox.Toolbox
{
    public class Resolver : Singleton<Resolver>, IRunner
    {
        [Required]
        [SerializeField] private GameObject instancesRoot;

        private List<object> instances;
        private Type injectAttributeType;
        private List<SceneBinding> currentSceneInstances = new List<SceneBinding>();

        public void Run()
        {
            injectAttributeType = typeof(InjectAttribute);

            instances = new List<object>();

            if(instancesRoot != null)
            {
                var newInstances = instancesRoot.GetComponents<MonoBehaviour>().ToList();
                
                foreach (var instance in newInstances)
                {
                    instances.Add(instance);
                }
            }

            Messager.Instance.SubscribeKeeping<SceneBindingMessage>(x => currentSceneInstances = x.instances.ToList());
            Messager.Instance.SubscribeKeeping<SceneUnloadedMessage>(_ => currentSceneInstances.Clear());
        }

        public void SearchObjectBindings(GameObject obj)
        {
            foreach (var cb in obj.GetComponentsInChildren<ComponentBinding>(true))
            {
                if (cb.Context != null)
                {
                    SceneBinding newBind = new SceneBinding() { instance = cb.Context, id = cb.Id };
                    
                    if (cb.ThisSceneOnly)
                    {
                        currentSceneInstances.Add(newBind);
                    }
                    else
                    {
                        instances.Add(newBind);
                    }
                    
                }
            }
        }

        public void InjectInstances()
        {
            foreach(var instance in instances)
            {
                Inject(instance);
            }
        }

        public void AddInstance(object instance)
        {
            if(instances.Where(x => x.GetType() == instance.GetType()).Any()) 
            {
                Debug.LogWarning($"Resolver already contains {instance.GetType()}");
            }
            else
            {
                instances.Add(instance);
            }
        }

        public void RemoveInstance(object instance)
        {
            if(instances.Contains(instance))
            {
                instances.Remove(instance);
            }
            else
            {
                Debug.LogWarning($"Resolver doesn't contain {instance.GetType()}");
            }
        }
    
        public void Inject(GameObject obj)
        {
            //Getting all monos from gameobject
            Component[] monosToInject = obj.GetComponentsInChildren<Component>(true);

            foreach(var mono in monosToInject)
            {
                Inject(mono);
            }
        }

        public void Inject(object obj)
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
                    .Where(f => f.GetCustomAttributes(injectAttributeType, true).Any());
                }

                foreach(var field in fields)
                {
                    string id = "";

                    if (currentSceneInstances.Count > 0)
                    {
                        id = field.GetCustomAttribute<InjectAttribute>(true).ID;
                    }

                    ResolveField(field, obj, id);
                }
            }
        }

        public T GetInstance<T>()
        {
            return (T)instances.Where(i => i is T).FirstOrDefault();
        }

        private void ResolveField(FieldInfo fieldInfo, object owner, string id = "")
        {
            //getting instance which type equals to required
            var instance = instances.Where(x => x.GetType() == fieldInfo.FieldType || fieldInfo.FieldType.IsInstanceOfType(x))
                .FirstOrDefault();

            if (instance == null)
            {
                SceneBinding binding;
                
                if (string.IsNullOrEmpty(id))
                {
                    binding = currentSceneInstances.Where(x => x.instance.GetType() == fieldInfo.FieldType).FirstOrDefault();
                }
                else
                {
                    binding = currentSceneInstances.Where(x => x.id == id && x.instance.GetType() == fieldInfo.FieldType).FirstOrDefault();
                }

                if (binding != null)
                {
                    instance = binding.instance;
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
    }

    [System.Serializable]
    public class SceneBinding
    {
        public object instance;
        public string id;
    }

    [Serializable]
    public class SceneBindingMessage: Message
    {
        public List<SceneBinding> instances = new List<SceneBinding>();
    }
}
