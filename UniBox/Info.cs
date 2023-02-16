using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox.UIInformer
{
    public class Info : Singleton<Info>
    {
        [SerializeField] private MessageBox messageBox;
        [SerializeField] private HintBox hintBox;

        private BoxBase _currentOpenedBox;

        public void ShowBox(string message, Action okAction = null, Action cancelAction = null)
        {
            messageBox.SetMessage(message);
            messageBox.SetCancelAction(cancelAction);
            messageBox.SetOkAction(okAction);
            
            if (messageBox.Show())
            {
                _currentOpenedBox = messageBox;
            }
        }

        public void ShowHint(string message, float? delay = null)
        {
            if (delay.HasValue)
            {
                hintBox.SetDelay(delay.Value);
            }
            
            hintBox.SetMessage(message);
            hintBox.Show();
        }
    }
}
