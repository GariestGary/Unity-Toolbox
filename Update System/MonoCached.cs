using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MonoCached : MonoBehaviour
    {
        [SerializeField] protected bool pauseable = true;

        protected float delta;
        protected float fixedDelta;
        protected bool paused;
        protected bool active;
        protected float interval;

        private bool raised;
        private bool ready;

        [HideInInspector] public float IntervalTimer;
        [HideInInspector] public float TimeStack;
        [HideInInspector] public float FixedTimeStack;

        private RectTransform _rect;

        public RectTransform rect
        {
            get 
            {
                if(_rect != null) return _rect;

                if(transform is RectTransform)
                {
                    _rect = transform as RectTransform;
                    return _rect;
                }
                else
                {
                    return null;
                }
            }
        }

        public float Interval
        {
            get
            {
                return interval;
            }
            set
            {
                if(value < 0) 
                {
                    interval = 0;
                }
                else
                {
                    interval = value;
                }
            }
        }
        public bool Paused => paused;
        public bool Pauseable => pauseable;
        public bool Active => active;

        public void OnRise()
        {
            if (raised) return;

            Rise();

            raised = true;
        }

        public void OnReady()
        {
            if (ready) return;

            Ready();

            ready = true;
        }

        protected virtual void Rise(){}
        protected virtual void Ready(){}
        protected virtual void Tick(){}
        protected virtual void FixedTick(){}
        protected virtual void LateTick(){}
        protected virtual void OnPause(){}
        protected virtual void OnResume(){}
        public virtual void OnRemove(){}
        
        public void Process(float delta)
        {
            this.delta = delta;

            if(!active || paused) return;

            Tick();
        }

        public void FixedProcess(float fixedDelta)
        {
            this.fixedDelta = fixedDelta;

            if(!active || paused) return;

            FixedTick();
        }

        public void LateProcess(float delta)
        {
            if(!active || paused) return;

            LateTick();
        }

        public void Pause()
        {
            if(paused || !pauseable) return;
            paused = true;
            OnPause();
        }

        public void Resume()
        {
            if(!paused) return;
            paused = false;
            OnResume();
        }

        public void Activate()
        {
            if(active) return;
            active = true;
            Resume();
        }

        public void Deactivate()
        {
            if(!active) return;
            active = false;
            Pause();
        }

    }
}
