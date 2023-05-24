using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [CustomEditor(typeof(AudioPlayerDataHolder))]
    public class AudioPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
