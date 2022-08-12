using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    public class Hider : EditorWindow 
    {
        [MenuItem("Tools/Hider")]
        private static void ShowWindow() 
        {
            var window = GetWindow<Hider>();
            window.titleContent = new GUIContent("Hider");
            window.Show();
        }
    
        private void OnGUI() 
        {
            if(GUILayout.Button("Unhide"))
            {
                var hidden = GameObject.FindObjectsOfType<GameObject>();

                foreach(var h in hidden)
                {
                    h.hideFlags = HideFlags.None;
                }
            }
        }
    }
}
