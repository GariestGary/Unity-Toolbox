using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VolumeBox.Toolbox.UIInformer
{
    public class HintBox : BoxBase, IPointerClickHandler
    {
        [SerializeField] private float selfFadeOutDuration;
        [SerializeField] private float defaultDelay;
        
        private float _currentDelay;
        private Coroutine selfFadeOutCoroutine;

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
            if(selfFadeOutCoroutine != null)
                StopCoroutine(selfFadeOutCoroutine);
            
            selfFadeOutCoroutine = StartCoroutine(CloseCoroutine());
            
            return true;
        }

        protected override void OnClose()
        {
            if(selfFadeOutCoroutine != null)
                StopCoroutine(selfFadeOutCoroutine);
            
            _currentDelay = defaultDelay;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (selfFadeOutCoroutine != null)
                StopCoroutine(selfFadeOutCoroutine);
            
            Close();
        }

        private IEnumerator CloseCoroutine()
        {
            yield return new WaitForSeconds(_currentDelay);
            
            float timer = selfFadeOutDuration;
            
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                canvasGroup.alpha = timer / selfFadeOutDuration;
                yield return null;
            }
            
            Close();
        }
    }
}