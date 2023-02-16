using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class ComponentBinding : MonoBehaviour
    {
        [SerializeField] private Component context;
        [SerializeField] private bool _thisSceneOnly = true;
        [SerializeField] private string _id;

        public Component Context => context;
        public bool ThisSceneOnly => _thisSceneOnly;
        public string Id => _id;
    }
}
