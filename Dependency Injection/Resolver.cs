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
            MonoBehaviour[] monosToInject = obj.GetComponentsInChildren<MonoBehaviour>(true);

            foreach(var mono in monosToInject)
            {
                Inject(mono);
            }
        }

        public void Inject(object obj)
        {
            if(obj == null) return;

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
                    ResolveField(field, obj);
                }
            }
        }

        public T GetInstance<T>()
        {
            return (T)instances.Where(i => i is T).FirstOrDefault();
        }

        private void ResolveField(FieldInfo fieldInfo, object owner)
        {
            //getting instance which type equals to required
            var instance = instances.Where(x => x.GetType() == fieldInfo.FieldType).FirstOrDefault();

            if(instance == null)
            {
                throw new Exception($"Instance of {fieldInfo.FieldType} doesn't binded");
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
}
