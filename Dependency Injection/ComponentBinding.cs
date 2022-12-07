using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class ComponentBinding: MonoBehaviour
    {
        [SerializeField] private Component context;
        [SerializeField] private string _id;

        public Component Context => context;
        public string Id => _id;
    }
}