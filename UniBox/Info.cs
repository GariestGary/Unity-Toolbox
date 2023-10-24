using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox.UIInformer
{
    public class Info: Singleton<Info>, IRunner
    {
        [SerializeField] private HintBox hintBox;

        private bool _messageBoxGettedResult;

        public void Run()
        {

        }

        public void ShowHint(string message, float? delay = null)
        {
            if (delay.HasValue)
            {
                hintBox.SetDelay(delay.Value);
            }
            
            hintBox.SetMessage(message);
            hintBox.Show();
            LayoutRebuilder.ForceRebuildLayoutImmediate(hintBox.LayoutRect);
        }

        private void OnMessageBoxClickedCallback()
        {
            _messageBoxGettedResult = true;
        }
    }
}
