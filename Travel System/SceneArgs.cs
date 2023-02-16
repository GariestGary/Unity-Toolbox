using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class SceneArgs : ScriptableObject
    {
        [Scene] public string sceneName;
        public Sprite backgroundSprite;

        [Button("Resolve Background")]
        public virtual void ResolveBackground()
        { 
        }
    }
}
