#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    public static class AudioUtils
    {
        private static MethodInfo m_PlayMethod;
        private static MethodInfo m_StopMethod;

        private static void ValidateMethods()
        {
            if(m_PlayMethod == null || m_StopMethod == null)
            {
                Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
                Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
                m_PlayMethod = audioUtilClass.GetMethod(
                    "PlayPreviewClip",
                    BindingFlags.Static | BindingFlags.Public
                );

                m_StopMethod = audioUtilClass.GetMethod(
                    "StopAllPreviewClips",
                    BindingFlags.Static | BindingFlags.Public
                );
            }
        }

        public static void PlayPreviewClip(AudioClip clip)
        {
            ValidateMethods();

            m_PlayMethod.Invoke(
                null,
                new object[]
                {
                        clip,
                        0,
                        false
                }
            );
        }

        public static void StopAllPreviewClips()
        {
            ValidateMethods();

            m_StopMethod.Invoke(
                null,
                new object[] { }
            );
        }
    }
}
#endif