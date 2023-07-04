using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CreateAssetMenu(fileName = "New Properties Database", menuName = "Toolbox/Properties Database")]
    public class PropertiesDatabase : ScriptableObject
    {
        [SerializeField, HideInInspector] private List<Property> properties;

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

        public void SetBool(string id, bool value)
        {
            var val = GetValue(id, PropertyType.Bool);

            if (val != null)
            {
                val.boolData = value;
            }
        }

        public void SetInt(string id, int value)
        {
            var val = GetValue(id, PropertyType.Integer);

            if(val != null)
            {
                val.integerData = value;
            }
        }

        public void SetFloat(string id, float value)
        {
            var val = GetValue(id, PropertyType.Float);

            if (val != null)
            {
                val.floatData = value;
            }
        }

        public void SetVector2(string id, Vector2 value)
        {
            var val = GetValue(id, PropertyType.Vector2);

            if (val != null)
            {
                val.vector2Data = value;
            }
        }

        public void SetVector3(string id, Vector3 value)
        {
            var val = GetValue(id, PropertyType.Vector3);

            if (val != null)
            {
                val.vector3Data = value;
            }
        }

        public void SetVector4(string id, Vector4 value)
        {
            var val = GetValue(id, PropertyType.Vector4);

            if (val != null)
            {
                val.vector4Data = value;
            }
        }

        public void SetString(string id, string value)
        {
            var val = GetValue(id, PropertyType.String);

            if (val != null)
            {
                val.stringData = value;
            }
        }

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
