using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class TestLogger : MonoBehaviour
    {
        [SerializeField] private string message;

        public void Log()
        {
            Debug.Log(message);
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }
    }
}
