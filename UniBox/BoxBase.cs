using UnityEngine;
using UnityEngine.Events;

namespace VolumeBox.Toolbox.UIInformer
{
    public abstract class BoxBase: MonoCached, IInformer
    {
        [SerializeField] private bool canChangeWhenOpened;
        
        public UnityEvent<string> MessageTextEvent;
        
        protected string _currentMessage;

        private bool _opened;

        public bool Opened => _opened;

        protected bool CanChange => (_opened && canChangeWhenOpened) || !_opened;

        public void SetMessage(string message)
        {
            _currentMessage = message;
            MessageTextEvent.Invoke(_currentMessage);
        }
        
        public bool Show()
        {
            if(!CanChange) return false;
            
            _opened = true;
            EnableGameObject();
            return OnShow();
        }

        protected abstract bool OnShow();

        public void Close()
        {
            if (!_opened) return;
            
            DisableGameObject();
            OnClose();
            _currentMessage = string.Empty;
            _opened = false;
        }

        protected abstract void OnClose();
    }
}