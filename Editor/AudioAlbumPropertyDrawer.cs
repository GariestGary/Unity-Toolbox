//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.Search;
//using UnityEngine;

//namespace VolumeBox.Toolbox
//{
//    [CustomPropertyDrawer(typeof(AudioAlbum))]
//    public class AudioAlbumPropertyDrawer : PropertyDrawer
//    {
//        private SerializedProperty m_clips;

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            EditorGUI.BeginProperty(position, label, property);

//            Rect foldoutBox = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);

//            property.isExpanded = EditorGUI.Foldout(foldoutBox, property.isExpanded, property.FindPropertyRelative("albumName").stringValue, true);

//            if(property.isExpanded)
//            {
//                GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));

//                Rect searchBox = new Rect(foldoutBox.y, foldoutBox.y + EditorGUIUtility.singleLineHeight, foldoutBox.width, foldoutBox.width);

//                string searchValue = string.Empty;

//                searchBox.width -= 175;

//                searchValue = GUI.TextField(searchBox, searchValue, GUI.skin.FindStyle("ToolbarSeachTextField"));

//                searchBox.x += searchBox.width;
//                searchBox.width = 175;

//                if (GUI.Button(searchBox, "", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
//                {
//                    searchValue = "";
//                    GUI.FocusControl(null);
//                }

//                //if (GUI.Button(searchBox, "Add"))
//                //{
//                //    //m_poolsList.InsertArrayElementAtIndex(0);
//                //}

//                GUILayout.EndHorizontal();
//            }

//            EditorGUI.EndProperty();
//        }

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            if(property.isExpanded)
//            {
//                return 250;
//            }
//            else
//            {
//                return base.GetPropertyHeight(property, label);
//            }
//        }
//    }
//}
