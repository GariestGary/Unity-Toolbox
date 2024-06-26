using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace VolumeBox.Toolbox.Editor
{
    public class SceneAdvancedDropdown: AdvancedDropdown
    {
        private Action<string> m_Callback;

        public SceneAdvancedDropdown(AdvancedDropdownState state) : base(state)
        {

        }

        public SceneAdvancedDropdown(AdvancedDropdownState state, Action<string> onSceneSelectedCallback) : base(state)
        {
            m_Callback = onSceneSelectedCallback;
        }

        public static string[] GetFormattedScenesList()
        {
            var scenes = new string[EditorBuildSettings.scenes.Length];

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                var scene = EditorBuildSettings.scenes[i];

                int pos = scene.path.LastIndexOf("/") + 1;
                scenes[i] = scene.path.Substring(pos, scene.path.Length - pos).Replace(".unity", "");
            }

            return scenes;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Scenes");

            var scenes = GetFormattedScenesList();

            for (int i = 0; i < scenes.Length; i++)
            {
                root.AddChild(new AdvancedDropdownItem(scenes[i]));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            m_Callback?.Invoke(item.name);
        }
    }
}
