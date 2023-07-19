using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class Database: ResourcesToolWrapper<Database, DatabaseDataHolder>
    {
        public override string GetDataPath()
        {
            return SettingsData.saverResourcesDataPath;
        }

        #region GETTERS
        public static bool? GetBool(string id)
        {
            return Instance.Data.Properties.GetBool(id);
        }

        public static int? GetInteger(string id)
        {
            return Instance.Data.Properties.GetInt(id);
        }
        public static float? GetFloat(string id)
        {
            return Instance.Data.Properties.GetFloat(id);
        }
        public static Vector2? GetVector2(string id)
        {
            return Instance.Data.Properties.GetVector2(id);
        }
        public static Vector3? GetVector3(string id)
        {
            return Instance.Data.Properties.GetVector3(id);
        }

        public static Vector4? GetVector4(string id)
        {
            return Instance.Data.Properties.GetVector4(id);
        }

        public static string GetString(string id)
        {
            return Instance.Data.Properties.GetString(id);
        }
        #endregion

        #region SETTERS

        public void SetBool(string id, bool value)
        {
            Instance.Data.Properties.SetBool(id, value);
        }

        public void SetInteger(string id, int value)
        {
            Instance.Data.Properties.SetInt(id, value);
        }

        public void SetFloat(string id, float value)
        {
            Instance.Data.Properties.SetFloat(id, value);
        }

        public void SetVector2(string id, Vector2 value)
        {
            Instance.Data.Properties.SetVector2(id, value);
        }

        public void SetVector3(string id, Vector3 value)
        {
            Instance.Data.Properties.SetVector3(id, value);
        }

        public void SetVector4(string id, Vector4 value)
        {
            Instance.Data.Properties.SetVector4(id, value);
        }

        public void SetString(string id, string value)
        {
            Instance.Data.Properties.SetString(id, value);
        }

        #endregion

        protected override void Clear()
        {
            base.Data?.Clear();
        }

        protected override void PostLoadRun()
        {
            base.Data?.Run();
        }
    }
}
