using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CreateAssetMenu(fileName = "New Properties Database", menuName = "Toolbox/Properties Database")]
    public class PropertiesDatabase : ScriptableObject
    {
        [SerializeField] private List<Property> properties;

        public bool? GetBool(string id)
        {
            return (bool?)GetValue(id, PropertyType.Bool);
        }

        public int? GetInt(string id)
        {
            return (int?)GetValue(id, PropertyType.Integer);
        }

        public float? GetFloat(string id) 
        {
            return (float?)GetValue(id, PropertyType.Float);
        }

        public Vector2? GetVector2(string id)
        {
            return (Vector2?)GetValue(id, PropertyType.Vector2);
        }

        public Vector3? GetVector3 (string id) 
        {
            return (Vector3?)GetValue(id,PropertyType.Vector3);
        }

        public Vector4? GetVector4(string id)
        {
            return (Vector4?)GetValue(id,PropertyType.Vector4);
        }

        public string GetString(string id) 
        {
            return (string)GetValue(id, PropertyType.String);
        }

        public object GetValue(string id, PropertyType type)
        {
            var props = properties.Where(p => p.id == id && p.type == type).ToList();

            if(props == null || props.Count <= 0)
            {
                Debug.LogWarning($"There is no property with {id} id and {type} type");
                return null;
            }
            else
            {
                return props[UnityEngine.Random.Range(0, props.Count)];
            }
        }
    }
}
