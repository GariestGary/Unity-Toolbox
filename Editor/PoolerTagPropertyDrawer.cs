using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    [CustomPropertyDrawer(typeof(FromPoolAttribute))]
    public class PoolerTagPropertyDrawer : PropertyDrawer
    {
        public static bool IsPoolsChanged;

        private PoolerDataHolder m_PoolerDataHolder;
        private string[] m_PoolsAvailable = new string[0];
        private bool m_IsDataHolderValid = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ValidatePools();

            if (!m_IsDataHolderValid)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginChangeCheck();

            var currentPool = property.stringValue;
            var selectedPoolIndex = Array.IndexOf(m_PoolsAvailable, currentPool);

            if(selectedPoolIndex < 0)
            {
                selectedPoolIndex = 0;
            }

            EditorGUI.LabelField(position, label);

            var labelRect = position;
            labelRect.x += EditorGUIUtility.labelWidth;
            labelRect.width -= EditorGUIUtility.labelWidth;

            selectedPoolIndex = EditorGUI.Popup(labelRect, selectedPoolIndex, m_PoolsAvailable);

            if(m_PoolsAvailable.Length <= 0)
            {
                property.stringValue = string.Empty;
            }
            else
            {
                property.stringValue = m_PoolsAvailable[selectedPoolIndex];
            }
        }

        private void ValidatePools()
        {
            if(IsPoolsChanged || m_PoolerDataHolder == null)
            {
                m_IsDataHolderValid = true;

                if(m_PoolerDataHolder == null)
                {
                    m_PoolerDataHolder = ResourcesUtils.ResolveScriptable<PoolerDataHolder>(SettingsData.poolerResourcesDataPath);
                }

                if(m_PoolerDataHolder == null)
                {
                    m_IsDataHolderValid = false;
                    return;
                }

                m_PoolsAvailable = new string[0];

                m_PoolsAvailable.Concat(m_PoolerDataHolder.PoolsList.ConvertAll(x => x.tag));
                //var scenePools = Resources.FindObjectsOfTypeAll<ScenePool>().ToList();
                //scenePools.ForEach(x => m_PoolsAvailable.Concat(x.Pools.ConvertAll(y => y.tag)));

                IsPoolsChanged = false;
            }
        }
    }
}
