using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    public class InitialScreenWindow : EditorWindow
    {
        private const int WindowWidth = 480;
        private const int WindowHeight = 400;

        private static Texture2D m_HeaderTex;

        [InitializeOnLoadMethod]
        private static void InitLoad()
        {
            EditorApplication.update += InitialShow;
        }

        [MenuItem("Toolbox/Setup Screen")]
        private static void InitialShow()
        {
#pragma warning disable
            InitialShowAsync();
#pragma warning enable
        }

        private static void InitialShowAsync()
        {
            if (!Application.isPlaying)
            {
                m_HeaderTex = Resources.Load<Sprite>("Icons/toolbox_banner").texture;

                GetWindow<InitialScreenWindow>(true, "Toolbox Setup");
                EditorApplication.update -= InitialShow;
            }
        }

        private void OnEnable()
        {
            

            this.position = new Rect((Screen.width / 2.0f) - WindowWidth / 2, (Screen.height / 2.0f) - WindowHeight / 2, WindowWidth, WindowHeight);
            this.minSize = new Vector2(WindowWidth, WindowHeight);
            this.maxSize = new Vector2(WindowWidth, WindowHeight);
        }

        private void OnGUI()
        {
            Rect headerRect = GUILayoutUtility.GetRect(480, 125);
            GUI.DrawTexture(headerRect, m_HeaderTex);

            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 20;

            if (!EditorLoadUtils.IsMainSceneCorrectInBuild())
            {
                Debug.LogError("There's an issue with MAIN scene, please open Toolbox Setup window to fix this");

                EditorGUILayout.HelpBox("MAIN scene is not in build setting or it's index not 0. You can fix this by pressing button below. It may take a while", MessageType.Error);

                if (GUILayout.Button("Initialize MAIN Scene", buttonStyle, GUILayout.Height(45)))
                {
                    EditorLoadUtils.InitializeMain();
                }
            }
            else
            {
                if (GUILayout.Button("Open MAIN Scene", buttonStyle, GUILayout.Height(45)))
                {
                    EditorLoadUtils.OpenMainScene();
                }
            }

            if(GUILayout.Button("Open Toolbox Settings", buttonStyle, GUILayout.Height(45)))
            {
                GetWindow<ToolboxSettingsEditorWindow>(false, "Toolbox Settings");
            }
        }

        private static void OnQuit()
        {

        }
    }
}
