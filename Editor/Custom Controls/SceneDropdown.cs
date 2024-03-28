using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VolumeBox.Toolbox.Editor
{
    internal sealed class SceneDropdown: AdvancedDropdown
    {
        public SceneDropdown(AdvancedDropdownState state) : base(state)
        {
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var scenes = EditorBuildSettings.scenes.ToList().ConvertAll(x => x.path);

            var root = new AdvancedDropdownItem("Scenes In Build");

            foreach(var scene in scenes)
            {
                var lastSlash = scene.LastIndexOf("/") + 1;
                var formattedSceneName = scene.Substring(lastSlash, scene.Length - lastSlash).Replace(".unity", "");

                root.AddChild(new SceneDropdownItem(formattedSceneName, formattedSceneName));
            }

            return root;
        }
    }

    public sealed class SceneDropdownItem: AdvancedDropdownItem
    {
        public readonly string SceneName;

        public SceneDropdownItem(string sceneName, string name) : base(name)
        {
            SceneName = sceneName;
        }
    }
}
