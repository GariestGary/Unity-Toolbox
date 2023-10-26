using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VolumeBox.Toolbox;

namespace VolumeBox.Toolbox.Utils.UI
{
    public class ToggleButton: MonoCached
    {
        [SerializeField] private bool autoResolveImage;
        [SerializeField] private bool enabledOnStart;
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        public UnityEvent<Sprite> ToggledOn;
        public UnityEvent<Sprite> ToggledOff;

        private Image img;
        private bool _on = false;

        public bool IsEnabled => _on;

        protected override void Rise()
        {
            _on = enabledOnStart;

            var toggle = GetComponent<Toggle>();

            if (toggle != null)
            {
                toggle.isOn = _on;
                toggle.onValueChanged.AddListener(SetState);
            }

            img = GetComponent<Image>();
        }

        public void OnClick()
        {
            _on = !_on;

            ResolveEvent();
        }

        private void ResolveEvent()
        {
            ResolveSprite();

            if (_on)
            {
                ToggledOn.Invoke(onSprite);
            }
            else
            {
                ToggledOff.Invoke(offSprite);
            }
        }

        public void ResolveSprite()
        {
            if (_on)
            {
                if (autoResolveImage && img != null)
                {
                    img.sprite = onSprite;
                }
            }
            else
            {
                if (autoResolveImage && img != null)
                {
                    img.sprite = offSprite;
                }
            }
        }

        public void SetState(bool value)
        {
            _on = value;

            ResolveEvent();
        }

        public void ForceResolve()
        {
            var toggle = GetComponent<Toggle>();
            toggle.isOn = true;
        }


        public void SetStateSilently(bool value)
        {
            _on = value;
        }
    }
}