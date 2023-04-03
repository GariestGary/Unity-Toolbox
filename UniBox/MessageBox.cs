using System;
using UnityEngine;

namespace VolumeBox.Toolbox.UIInformer
{
    public class MessageBox : BoxBase
    {
        [SerializeField] private GameObject okButton;
        [SerializeField] private GameObject cancelButton;

        private bool _isAsync;

        private Action _currentOkAction;
        private Action _currentCancelAction;

        public event Action OnButtonClicked;

        public MessageBoxResult Result { get; private set; }

        public void OkClick()
        {
            _currentOkAction?.Invoke();
            OnButtonClicked?.Invoke();
            Result = MessageBoxResult.OK;
            Close();
        }

        public void CancelClick()
        {
            _currentCancelAction?.Invoke();
            OnButtonClicked?.Invoke();
            Result = MessageBoxResult.CANCEL;
            Close();
        }
        
        public void SetOkAction(Action action)
        {
            if (!CanChange) return;
            
            _currentOkAction = action;
        }

        public void SetCancelAction(Action action)
        {
            if(!CanChange) return;
            
            _currentCancelAction = action;
        }

        public void SetAsync()
        {
            _isAsync = true;
        }

        protected override bool OnShow()
        {
            if (_currentCancelAction == null && _currentOkAction == null) return false;

            if (_currentOkAction == null && !_isAsync)
            {
                okButton.Disable();
            }
            else
            {
                okButton.Enable();
            }

            return true;
        }

        protected override void OnClose()
        {
            _currentCancelAction = null;
            _currentOkAction = null;
            _isAsync = false;
        }
    }

    public enum MessageBoxResult
    {
        OK,
        CANCEL,
    }
}