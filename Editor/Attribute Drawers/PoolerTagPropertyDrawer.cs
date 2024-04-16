using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(FromPoolAttribute))]
    public class PoolerTagPropertyDrawer : PropertyDrawer
    {
        public static bool IsPoolsChanged { get; set; }

        private PoolerDataHolder m_PoolerDataHolder;
        private Component m_Target;
        private string[] m_PoolsAvailable;
        private string[] m_ParsedPoolTags = new string[0];

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValidatePools(property);

            var labelRect = position;
            labelRect.width = EditorGUIUtility.labelWidth + 2;

            EditorGUI.LabelField(labelRect, label);

            var poolRect = position;
            poolRect.x = labelRect.width;
            poolRect.width -= labelRect.width;

            if (m_PoolsAvailable == null || m_PoolsAvailable.Length <= 0)
            {
                EditorGUI.LabelField(poolRect ,"There is no available pools");
                return;
            }

            EditorGUI.BeginChangeCheck();

            var currentPool = property.stringValue;
            var selectedPoolIndex = Array.IndexOf(m_PoolsAvailable, currentPool);

            if(selectedPoolIndex < 0)
            {
                selectedPoolIndex = 0;
            }

            selectedPoolIndex = EditorGUI.Popup(poolRect, selectedPoolIndex, m_ParsedPoolTags);

            if(m_PoolsAvailable.Length <= 0)
            {
                property.stringValue = string.Empty;
            }
            else
            {
                property.stringValue = m_PoolsAvailable[selectedPoolIndex];
            }

            EditorGUI.EndChangeCheck();
        }

        private void ValidatePools(SerializedProperty property)
        {
            if(m_PoolerDataHolder == null)
            {
                m_PoolerDataHolder = ResourcesUtils.ResolveScriptable<PoolerDataHolder>(SettingsData.poolerResourcesDataPath);
            }

            if(m_Target == null)
            {
                var target = property.GetValue();
                m_Target = target as Component;
            }

            if(IsPoolsChanged || m_PoolsAvailable == null)
            {
                m_PoolsAvailable = new string[0];

                m_PoolsAvailable = m_PoolsAvailable.Concat(m_PoolerDataHolder.PoolsList.ConvertAll(x => x.tag)).ToArray();
                var scenePools = Resources.FindObjectsOfTypeAll<ScenePool>().Where(x => x.gameObject.scene == m_Target.gameObject.scene).ToList();
                m_ParsedPoolTags = m_PoolsAvailable.ToArray();

                scenePools.ForEach(x => 
                {
                    var convertedScenePoolTags = x.Pools.ConvertAll(y => y.tag);
                    m_ParsedPoolTags = m_ParsedPoolTags.Concat(convertedScenePoolTags.ConvertAll(p => $"{p} [{x.gameObject.scene.name}]")).ToArray();
                    m_PoolsAvailable = m_PoolsAvailable.Concat(convertedScenePoolTags).ToArray();
                });

                IsPoolsChanged = false;
            }
        }
    }
}
