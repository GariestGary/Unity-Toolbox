using System;
using UnityEngine;

namespace VolumeBox.Toolbox.UIInformer
{
    public class MessageBox : BoxBase
    {
        [SerializeField] private GameObject okButton;
        [SerializeField] private GameObject cancelButton;
        
        private Action _currentOkAction;
        private Action _currentCancelAction;

        public void OkClick()
        {
            _currentOkAction?.Invoke();
            Close();
        }

        public void CancelClick()
        {
            _currentCancelAction?.Invoke();
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

        protected override bool OnShow()
        {
            if (_currentCancelAction == null && _currentOkAction == null) return false;

            if (_currentOkAction == null)
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
        }
    }
}