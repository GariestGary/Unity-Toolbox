using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LevelAttribute: Attribute
    {
        public string SceneName { get; }
        public bool IsGameplayLevel { get; }

        public LevelAttribute() { }

        public LevelAttribute(string sceneName, bool isGameplayLevel = true)
        {
            IsGameplayLevel = isGameplayLevel;
            SceneName = sceneName;
        }
    }
}
