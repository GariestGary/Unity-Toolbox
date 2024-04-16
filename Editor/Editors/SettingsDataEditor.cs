#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    [CustomEditor(typeof(SettingsData))]
    public class SettingsDataEditor: UnityEditor.Editor
    {
        [SerializeField] private GUISkin m_Skin;

        public const float LABEL_WIDTH = 150;

        private SerializedProperty m_resolveAtPlay;
        private SerializedProperty m_timeScale;
        private SerializedProperty m_targetFrameRate;
        private SerializedProperty m_initialSceneName;
        private SerializedProperty m_initialSceneArgs;
        private SerializedProperty m_manualFadeOut;
        private SerializedProperty m_fadeOutDuration;

        private int selectedScene;
        private string[] scenesList;

        private void OnEnable()
        {
            m_resolveAtPlay = serializedObject.FindProperty("AutoResolveScenesAtPlay");
            m_timeScale = serializedObject.FindProperty("TimeScale");
            m_targetFrameRate = serializedObject.FindProperty("TargetFrameRate");
            m_initialSceneName = serializedObject.FindProperty("InitialSceneName");
            m_initialSceneArgs = serializedObject.FindProperty("InitialSceneArgs");
            m_manualFadeOut = serializedObject.FindProperty("ManualFadeOut");
            m_fadeOutDuration = serializedObject.FindProperty("FadeOutDuration");

            RebuildScenesList();
        }

        private void RebuildScenesList()
        {
            scenesList = EditorBuildSettings.scenes.ToList().ConvertAll(x =>
            {
                int pos = x.path.LastIndexOf("/") + 1;
                return x.path.Substring(pos, x.path.Length - pos).Replace(".unity", "");
            }).ToArray();

            selectedScene = scenesList.ToList().IndexOf(m_initialSceneName.stringValue);

            if (selectedScene < 0)
            {
                selectedScene = 0;
            }
        }

        private bool IsScenesChanged()
        {
            if(scenesList.Length != EditorBuildSettings.scenes.Length)
            {
                return true;
            }

            for (int i = 0; i < scenesList.Length; i++)
            {
                if (!EditorBuildSettings.scenes[i].path.Contains(scenesList[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public void DrawIMGUI()
        {
            if(IsScenesChanged())
            {
                RebuildScenesList();
            }

            var oldSkin = GUI.skin;

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            //HEADER
            GUI.skin = m_Skin;
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            GUI.skin = oldSkin;
            EditorGUILayout.LabelField("Scene Management", EditorStyles.largeLabel);
            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Resolve Scenes On Play", GUILayout.Width(LABEL_WIDTH));
            m_resolveAtPlay.boolValue = EditorGUILayout.Toggle(m_resolveAtPlay.boolValue, GUILayout.Width(EditorGUIUtility.singleLineHeight));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            //HEADER
            GUI.skin = m_Skin;
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            GUI.skin = oldSkin;
            EditorGUILayout.LabelField("Timings", EditorStyles.largeLabel);
            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time Scale", GUILayout.Width(LABEL_WIDTH));
            m_timeScale.floatValue = EditorGUILayout.Slider(m_timeScale.floatValue, 0, 5);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Frame Rate", GUILayout.Width(LABEL_WIDTH));
            var oldFramerate = m_targetFrameRate.intValue;
            var newFramerate = EditorGUILayout.IntField(GUIContent.none, m_targetFrameRate.intValue);
            m_targetFrameRate.intValue = newFramerate;

            if(newFramerate < 0)
            {
                newFramerate = 0;
            }

            if(oldFramerate != newFramerate)
            {
                ENTRY.UpdateTargetFramerate(newFramerate);
            }

            GUILayout.EndHorizontal();

            var optionDataList = new List<string>();

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                optionDataList.Add(name);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            //HEADER
            GUI.skin = m_Skin;
            EditorGUILayout.BeginVertical(GUI.skin.FindStyle("Box"));
            GUI.skin = oldSkin;
            EditorGUILayout.LabelField("Initial Scene", EditorStyles.largeLabel);
            EditorGUILayout.Space(5);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Scene Name", GUILayout.Width(EditorGUIUtility.labelWidth));
            selectedScene = EditorGUILayout.Popup(selectedScene, scenesList);
            m_initialSceneName.stringValue = scenesList[selectedScene];
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Initial Scene Args", GUILayout.Width(LABEL_WIDTH));
            EditorGUILayout.PropertyField(m_initialSceneArgs, GUIContent.none);
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Manual Fade Out", GUILayout.Width(LABEL_WIDTH));
            m_manualFadeOut.boolValue = EditorGUILayout.Toggle(m_manualFadeOut.boolValue);
            EditorGUILayout.EndHorizontal();

            if(!m_manualFadeOut.boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Fade Out Duration", GUILayout.Width(LABEL_WIDTH));
                m_fadeOutDuration.floatValue = EditorGUILayout.FloatField(m_fadeOutDuration.floatValue);
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif