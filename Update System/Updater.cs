using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace VolumeBox.Toolbox
{
    public class Updater : ToolWrapper<Updater>
    {
        #pragma warning disable
        private static float timeScale = 1;
        private static float delta;
        #pragma warning restore
        
        public static float UnscaledDelta => Time.deltaTime;
        public static float TimeScale
        {
            get
            {
                return timeScale;
            } 
            set
            {
                if(value < 0)
                {
                    timeScale = 0f;
                } 
                else if(value > 1)
                {
                    timeScale = 1f;
                } 
                else
                {
                    timeScale = value;
                }
            }
        }
        public static float Delta => delta;

        public static event Action<float> ProcessTick;
        public static event Action<float> FixedProcessTick;
        public static event Action<float> LateProcessTick;

        private static MethodInfo riseMethod;
        private static MethodInfo readyMethod;
        private static MethodInfo subscribeProcessMethod;

        protected override void Run()
        {
            riseMethod = typeof(MonoCached).GetMethod("OnRise", BindingFlags.NonPublic | BindingFlags.Instance);
            readyMethod = typeof(MonoCached).GetMethod("OnReady", BindingFlags.NonPublic | BindingFlags.Instance);
            subscribeProcessMethod = typeof(MonoCached).GetMethod("HandleProcessSubscribe", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        protected override void Clear()
        {
            
        }

        /// <summary>
        /// Injects given GameObjects, rises and ready them, and then adds them to process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public static void InitializeObjects(GameObject[] objs)
        {
            foreach(var obj in objs)
            {
                if (obj == null) return;

                Resolver.Inject(obj);

                MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

                foreach (var mono in objMonos)
                {
                    InvokeRise(mono);
                }
            }

            foreach (var obj in objs)
            {
                if (obj == null) return;

                MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

                foreach (var mono in objMonos)
                {
                    InvokeReady(mono);
                }
            }

            foreach (var obj in objs)
            {
                if (obj == null) return;

                MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

                foreach (var mono in objMonos)
                {
                    InvokeProcessSubscription(mono);
                }
            }
        }

        /// <summary>
        /// Injects given GameObject, rises and ready it, and then adds it to process
        /// </summary>
        /// <param name="obj"></param>
        public static void InitializeObject(GameObject obj)
        {
            if (obj == null) return;

            Resolver.Inject(obj);

            MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

            foreach (var mono in objMonos)
            {
                InvokeRise(mono);
            }

            foreach (var mono in objMonos)
            {
                InvokeReady(mono);
            }

            foreach (var mono in objMonos)
            {
                InvokeProcessSubscription(mono);
            }
        }

        public static void InitializeMono(MonoCached mono)
        {
            if (mono == null) return;

            Resolver.Inject(mono);
            InvokeRise(mono);
            InvokeReady(mono);
            InvokeProcessSubscription(mono);
        }
        

        #region Invoke Reflection Methods
        private static void InvokeRise(MonoCached mono)
        {
            riseMethod.Invoke(mono, null);
        }

        private static void InvokeReady(MonoCached mono)
        {
            readyMethod.Invoke(mono, null);
        }

        private static void InvokeProcessSubscription(MonoCached mono)
        {
            subscribeProcessMethod.Invoke(mono, null);
        }
        #endregion

        #region Updates
        private void Update()
        {
            delta = Time.deltaTime * TimeScale;
            ProcessTick?.Invoke(delta);
        }

        private void FixedUpdate()
        {
            float fixedDelta = Time.fixedDeltaTime * timeScale;
            FixedProcessTick?.Invoke(fixedDelta);  
        }

        private void LateUpdate()
        {
            LateProcessTick?.Invoke(delta);
        }
        #endregion
    }
}
