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

        #region GETTERS
        public bool? GetBool(string id)
        {
            return GetValue(id, PropertyType.Bool)?.boolData;
        }

        public int? GetInt(string id)
        {
            return GetValue(id, PropertyType.Integer)?.integerData;
        }

        public float? GetFloat(string id) 
        {
            return GetValue(id, PropertyType.Float)?.floatData;
        }

        public Vector2? GetVector2(string id)
        {
            return GetValue(id, PropertyType.Vector2)?.vector2Data;
        }

        public Vector3? GetVector3 (string id) 
        {
            return GetValue(id,PropertyType.Vector3)?.vector3Data;
        }

        public Vector4? GetVector4(string id)
        {
            return GetValue(id, PropertyType.Vector4)?.vector4Data;
        }

        public string GetString(string id) 
        {
            return GetValue(id, PropertyType.String)?.stringData;
        }
        #endregion

        #region SETTERS


        #endregion

        private Property GetValue(string id, PropertyType type)
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
