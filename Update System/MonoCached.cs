using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MonoCached : MonoBehaviour
    {
        [SerializeField] protected bool pauseable = true;

        protected float delta;
        protected float fixedDelta;
        protected bool paused;
        protected float interval;

        private bool active;
        private bool raised;
        private bool ready;

        [HideInInspector] public float IntervalTimer;
        [HideInInspector] public float TimeStack;
        [HideInInspector] public float FixedTimeStack;

        public float Interval
        {
            get { return interval; }
            set
            {
                if (value < 0)
                {
                    interval = 0;
                }
                else
                {
                    interval = value;
                }
            }
        }

        public RectTransform rect
        {
            get
            {
                if (transform is RectTransform)
                {
                    return transform as RectTransform;
                }
                else
                {
                    return null;
                }
            }
            private set { }
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

        protected virtual void Rise()
        {
        }

        protected virtual void Ready()
        {
        }

        protected virtual void Tick()
        {
        }

        protected virtual void FixedTick()
        {
        }

        protected virtual void LateTick()
        {
        }

        protected virtual void OnPause()
        {
        }

        protected virtual void OnResume()
        {
        }

        protected virtual void Destroyed()
        {
            
        }

        public virtual void OnRemove(){}
        public virtual void OnAdd(){}
        public virtual void OnActivate(){}
        public virtual void OnDeactivate(){}
        
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

        private void Pause()
        {
            if(paused || !pauseable) return;
            paused = true;
            OnPause();
        }

        private void Resume()
        {
            if(!paused) return;
            paused = false;
            OnResume();
        }
        
        public void EnableGameObject()
        {
            if(gameObject.activeSelf) return;
            
            gameObject.SetActive(true);
        }

        public void DisableGameObject()
        {
            if (!gameObject.activeSelf) return;
            
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if(active) return;
            active = true;
            OnActivate();
            Resume();
        }

        private void OnDisable()
        {
            if(!active) return;
            active = false;
            OnDeactivate();
            Pause();
        }

        private void OnDestroy()
        {
            //
            if(!Updater.HasInstance) return;

            Updater upd = Updater.Instance;
            
            if(upd == null) return;
            
            upd.RemoveMonoFromProcess(this);
            
            Destroyed();
        }
    }

    public static class GameObjectExtensions
    {
        public static void Enable(this GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        public static void Disable(this GameObject gameObject)
        {
            gameObject.SetActive(false);
        }
    }
}
