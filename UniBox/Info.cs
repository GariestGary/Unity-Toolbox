using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox.UIInformer
{
    public class Info : Singleton<Info>, IRunner
    {
        [SerializeField] private MessageBox messageBox;
        [SerializeField] private HintBox hintBox;

        private bool _messageBoxGettedResult;

        public void ShowBox(string message, Action okAction, Action cancelAction = null, string headerCaption = null, string okCaption = null, string cancelCaption = null, Color? okColor = null, Color? cancelColor = null)
        {
            messageBox.SetMessage(message);
            messageBox.SetCancelAction(cancelAction);
            messageBox.SetOkAction(okAction);

            if(headerCaption.IsValuable())
            {
                messageBox.SetHeaderCaption(headerCaption);
            }

            if (okCaption.IsValuable())
            {
                messageBox.SetOkCaption(okCaption);
            }

            if(cancelCaption.IsValuable())
            {
                messageBox.SetCancelCaption(cancelCaption);
            }

            if(okColor.HasValue)
            {
                messageBox.SetOkColor(okColor.Value);
            }

            if(cancelColor.HasValue)
            {
                messageBox.SetCancelColor(cancelColor.Value);
            }

            messageBox.Show();
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageBox.Rect);
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

        public async UniTask<MessageBoxResult> ShowBox(string message)
        {
            messageBox.SetAsync();
            messageBox.SetMessage(message);
            messageBox.Show();
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageBox.LayoutRect);

            while(!_messageBoxGettedResult)
            {
                await UniTask.Yield();
            }

            _messageBoxGettedResult = false;

            return messageBox.Result;
        }

        private void OnMessageBoxClickedCallback()
        {
            _messageBoxGettedResult = true;
        }

        public void Run()
        {
            messageBox.OnButtonClicked += OnMessageBoxClickedCallback;
        }

        private void OnDestroy()
        {
            messageBox.OnButtonClicked -= OnMessageBoxClickedCallback;
        }
    }
}
