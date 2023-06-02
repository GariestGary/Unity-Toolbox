using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VolumeBox.Toolbox.UIInformer
{
    public class MessageBox: BoxBase
    {
        [SerializeField] private GameObject okButton;
        [SerializeField] private GameObject cancelButton;
        [SerializeField] private Image okImage;
        [SerializeField] private Image cancelImage;
        [SerializeField] private TMP_Text okText;
        [SerializeField] private TMP_Text cancelText;
        [SerializeField] private string defaultOkCaption;
        [SerializeField] private string defaultCancelCaption;
        [SerializeField] private Color defaultColor;

        private bool _isAsync;

        private Action _currentOkAction;
        private Action _currentCancelAction;

        public event Action OnButtonClicked;

        private string _currentOkCaption;
        private string _currentCancelCaption;

        private Color _currentOkColor;
        private Color _currentCancelColor;

        public MessageBoxResult Result { get; private set; }

        protected override void Rise()
        {
            base.Rise();

            _currentCancelColor = defaultColor;
            _currentOkColor = defaultColor;
            _currentOkCaption = defaultOkCaption;
            _currentCancelCaption = defaultCancelCaption;
        }

        public void OkClick()
        {
            _currentOkAction?.Invoke();
            OnButtonClicked?.Invoke();
            Result = MessageBoxResult.OK;
            Close();
        }

        public void SetOkCaption(string caption)
        {
            _currentOkCaption = caption;
        }

        public void SetOkColor(Color color)
        {
            _currentOkColor = color;
        }

        public void CancelClick()
        {
            _currentCancelAction?.Invoke();
            OnButtonClicked?.Invoke();
            Result = MessageBoxResult.CANCEL;
            Close();
        }

        public void SetCancelCaption(string caption)
        {
            _currentCancelCaption = caption;
        }

        public void SetCancelColor(Color color)
        {
            _currentCancelColor = color;
        }

        public void SetOkAction(Action action)
        {
            if (!CanChange) return;

            _currentOkAction = action;
        }

        public void SetCancelAction(Action action)
        {
            if (!CanChange) return;

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

            if (cancelText != null)
            {
                cancelText.text = _currentCancelCaption;
            }

            if (okText != null)
            {
                okText.text = _currentOkCaption;
            }

            if (okImage != null)
            {
                okImage.color = _currentOkColor;
            }

            if (cancelImage != null)
            {
                cancelImage.color = _currentCancelColor;
            }

            _currentOkColor = defaultColor;
            _currentCancelColor = defaultColor;

            _currentCancelCaption = defaultCancelCaption;
            _currentOkCaption = defaultOkCaption;

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