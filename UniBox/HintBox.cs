using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VolumeBox.Toolbox.UIInformer
{
    public class HintBox : BoxBase, IPointerClickHandler
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private float defaultDelay;
        
        private float _currentDelay;

        protected override void Rise()
        {
            _currentDelay = defaultDelay;
        }

        public void SetDelay(float? delay)
        {
            if (delay.HasValue)
            {
                _currentDelay = Mathf.Clamp(delay.Value, 0, float.MaxValue);
            }
            else
            {
                _currentDelay = defaultDelay;
            }
        }

        protected override bool OnShow()
        {
            StopCoroutine(nameof(CloseCoroutine));
            canvasGroup.alpha = 1;
            
            StartCoroutine(nameof(CloseCoroutine));
            
            return true;
        }

        protected override void OnClose()
        {
            _currentDelay = defaultDelay;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StopCoroutine(nameof(CloseCoroutine));
            Close();
        }

        private IEnumerator CloseCoroutine()
        {
            yield return new WaitForSeconds(_currentDelay);
            
            float timer = fadeOutDuration;
            
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                canvasGroup.alpha = timer / fadeOutDuration;
                yield return null;
            }
            
            Close();
        }
    }
}