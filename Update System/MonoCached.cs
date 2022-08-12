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
        [HideInInspector] public float IntervalTimer;
        [HideInInspector] public float TimeStack;
        [HideInInspector] public float FixedTimeStack;

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


        public virtual void Rise(){}
        public virtual void Ready(){}
        public virtual void Tick(){}
        public virtual void FixedTick(){}
        public virtual void LateTick(){}
        public virtual void OnRemove(){}
        protected virtual void OnPause(){}
        protected virtual void OnResume(){}
        
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
