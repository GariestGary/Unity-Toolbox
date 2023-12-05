//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace VolumeBox.Toolbox.Editor
//{
//    [CustomPropertyDrawer(typeof(PoolAttribute))]
//    public class PoolPropertyDrawer : PropertyDrawer
//    {
//        private PoolerDataHolder m_PoolerDataHolder;

//        private bool m_IsDataHolderValid = false;

//        public static bool IsPoolsChanged = false;

//        private 

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
            

//            var labelRect = position;
//            position.width = EditorGUIUtility.labelWidth;
//            EditorGUI.LabelField(labelRect, label);
//        }

//        private void ValidatePools()
//        {
//            if(IsPoolsChanged || (!IsPoolsChanged && m_PoolerDataHolder == null))
//            {
//                m_IsDataHolderValid = true;

//                if(m_PoolerDataHolder == null)
//                {
//                    m_PoolerDataHolder = ResourcesUtils.ResolveScriptable<PoolerDataHolder>(SettingsData.poolerResourcesDataPath);
//                }

//                if(m_PoolerDataHolder == null)
//                {
//                    m_IsDataHolderValid = false;
//                    return;
//                }
//            }
//        }
//    }
//}
