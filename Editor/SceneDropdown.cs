using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace VolumeBox.Toolbox.Editor
{
    public class SceneDropdown: AdvancedDropdown
    {
        private Action<string> m_Callback;

        public SceneDropdown(AdvancedDropdownState state) : base(state)
        {

        }

        public SceneDropdown(AdvancedDropdownState state, Action<string> onSceneSelectedCallback) : base(state)
        {
            m_Callback = onSceneSelectedCallback;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Scenes");

            foreach(var scene in EditorBuildSettings.scenes)
            {
                int pos = scene.path.LastIndexOf("/") + 1;
                var name = scene.path.Substring(pos, scene.path.Length - pos).Replace(".unity", "");

                root.AddChild(new AdvancedDropdownItem(name));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            m_Callback?.Invoke(item.name);
        }
    }
}
