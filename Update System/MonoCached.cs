using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    public class MonoCached : MonoBehaviour
    {
        [Foldout("Process Settings")]
        [SerializeField]
        private bool processIfInactive = false;
        [Foldout("Process Settings")]
        [ShowIf(nameof(processIfInactive))]
        [SerializeField]
        private bool processIfInactiveInHierarchy = false;

        private RectTransform rect;
        protected float delta;
        protected float fixedDelta;
        protected float interval;
        private bool pausedByActiveState = false;
        private bool pausedManual = false;

        private bool raised;
        private bool ready;

        [HideInInspector] public float IntervalTimer;
        [HideInInspector] public float TimeStack;
        [HideInInspector] public float FixedTimeStack;

        public bool Paused => pausedByActiveState || pausedManual;

        public bool ProcessIfInactive
        {
            get => processIfInactive;

            set
            {
                processIfInactive = value;
            }
        }

        public bool ProcessIfInactiveInHierarchy
        {
            get => processIfInactiveInHierarchy;

            set
            {
                processIfInactiveInHierarchy = value;

                if(value)
                {
                    processIfInactive = true;
                }
            }
        }

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

        public RectTransform Rect
        {
            get
            {
                if(rect == null)
                {
                    if (transform is RectTransform)
                    {
                        rect = transform as RectTransform;
                    }
                }

                return rect;
            }
            private set { }
        }

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


        /// <summary>
        /// Alternative to Awake()
        /// </summary>
        protected virtual void Rise(){}

        /// <summary>
        /// Alternative to Start()
        /// </summary>
        protected virtual void Ready(){}

        /// <summary>
        /// Alternative to Update()
        /// </summary>
        protected virtual void Tick(){}

        /// <summary>
        /// Alternative to FixedUpdate()
        /// </summary>
        protected virtual void FixedTick(){}

        /// <summary>
        /// Alternative to LateUpdate()
        /// </summary>
        public virtual void LateTick(){}


        public virtual void Destroyed(){}

        public virtual void OnRemove(){}

        public virtual void OnAdd(){}
        
        
        protected virtual void OnPause(){}

        protected virtual void OnResume(){}

        protected virtual void OnActivate(){}

        protected virtual void OnDeactivate(){}
        
        public void Process(float delta)
        {
            this.delta = delta;

            if(pausedByActiveState || pausedManual) return;

            Tick();
        }

        public void FixedProcess(float fixedDelta)
        {
            this.fixedDelta = fixedDelta;

            if(pausedByActiveState || pausedManual) return;

            FixedTick();
        }

        public void LateProcess(float delta)
        {
            if(pausedByActiveState || pausedManual) return;

            LateTick();
        }

        public void Pause()
        {
            if(pausedManual) return;
            pausedManual = true;
            OnPause();
        }

        public void Resume()
        {
            if(!pausedManual) return;
            pausedManual = false;
            OnResume();
        }
        
        public void EnableGameObject()
        {
            gameObject.SetActive(true);
        }

        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            pausedByActiveState = false;

            if(!pausedManual)
            {
                OnResume();
            }

            OnActivate();
        }

        private void OnDisable()
        {
            if(!processIfInactive)
            {
                if(gameObject.activeInHierarchy)
                {
                    if(!processIfInactiveInHierarchy)
                    {
                        if(!pausedByActiveState)
                        {
                            OnPause();
                        }

                        pausedByActiveState = true;
                    }
                }
                else
                {
                    if(!pausedByActiveState)
                    {
                        OnPause();
                    }

                    pausedByActiveState = true;
                }
            }

            OnDeactivate();
        }

        private void OnDestroy()
        {
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
