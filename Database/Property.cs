using System;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [Serializable]
    public class Property
    {
        public string id;
        public PropertyType type;
        public bool boolData;
        public int integerData;
        public float floatData;
        public Vector2 vector2Data;
        public Vector3 vector3Data;
        public Vector4 vector4Data;
        public string stringData;
    }

    public enum PropertyType
    {
        Bool,
        Integer,
        Float,
        Vector2,
        Vector3,
        Vector4,
        String,
    }
}
