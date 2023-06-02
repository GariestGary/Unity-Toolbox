using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VolumeBox.Toolbox.UIInformer
{
    public abstract class BoxBase: MonoCached, IInformer
    {
        [SerializeField] protected RectTransform layoutRect;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float fadeInDuration;
        [SerializeField] protected float fadeOutDuration;
        [SerializeField] protected bool canChangeWhenOpened;

        public UnityEvent<string> MessageTextEvent;
        public UnityEvent<string> HeaderCaptionEvent;

        protected string _currentMessage;
        protected string _currentHeaderCaption;

        private bool _opened;
        private Coroutine fadeInCoroutine;
        private Coroutine fadeOutCoroutine;

        public bool Opened => _opened;
        public RectTransform LayoutRect => layoutRect;

        protected bool CanChange => (_opened && canChangeWhenOpened) || !_opened;

        public void SetHeaderCaption(string caption)
        {
            _currentHeaderCaption = caption;
            HeaderCaptionEvent.Invoke(_currentHeaderCaption);
        }

        public void SetMessage(string message)
        {
            _currentMessage = message;
            MessageTextEvent.Invoke(_currentMessage);
        }

        public bool Show()
        {
            if (!CanChange) return false;

            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            if (fadeInCoroutine != null)
                StopCoroutine(fadeInCoroutine);

            fadeInCoroutine = StartCoroutine(FadeInCoroutine());

            _opened = true;
            return OnShow();
        }

        protected abstract bool OnShow();

        public void Close()
        {
            if (!_opened) return;

            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            if (fadeInCoroutine != null)
                StopCoroutine(fadeInCoroutine);

            fadeOutCoroutine = StartCoroutine(FadeOutCoroutine());

            OnClose();

            _currentMessage = string.Empty;
            _opened = false;
        }

        protected abstract void OnClose();

        private IEnumerator FadeOutCoroutine()
        {
            if (canvasGroup == null) yield break;

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha > 0)
            {
                stack += Time.deltaTime / fadeOutDuration;
                alpha = Mathf.Lerp(1, 0, stack);
                canvasGroup.alpha = alpha;
                yield return null;
            }

            canvasGroup.SetInteractions(false);
        }

        private IEnumerator FadeInCoroutine()
        {
            if (canvasGroup == null) yield break;

            canvasGroup.SetInteractions(true);

            float alpha = canvasGroup.alpha;
            float stack = 0;

            while (alpha < 1)
            {
                stack += Time.deltaTime / fadeInDuration;
                alpha = Mathf.Lerp(0, 1, stack);
                canvasGroup.alpha = alpha;
                yield return null;
            }
        }
    }
}