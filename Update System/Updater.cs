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
    public class Updater : Singleton<Updater>, IRunner
    {
        private static float timeScale = 1;
        private static float delta;
        
        public float UnscaledDelta => Time.deltaTime;
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

        private MethodInfo riseMethod;
        private MethodInfo readyMethod;
        private MethodInfo subscribeProcessMethod;

        public void Run()
        {
            riseMethod = typeof(MonoCached).GetMethod("OnRise", BindingFlags.NonPublic | BindingFlags.Instance);
            readyMethod = typeof(MonoCached).GetMethod("OnReady", BindingFlags.NonPublic | BindingFlags.Instance);
            subscribeProcessMethod = typeof(MonoCached).GetMethod("HandleProcessSubscribe", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Injects given GameObjects, rises and ready them, and then adds them to process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public void InitializeObjects(GameObject[] objs)
        {
            foreach(var obj in objs)
            {
                InitializeObject(obj);
            }
        }

        /// <summary>
        /// Injects given GameObject, rises and ready it, and then adds it to process
        /// </summary>
        /// <param name="obj"></param>
        public void InitializeObject(GameObject obj)
        {
            if (obj == null) return;

            Resolver.Instance.Inject(obj);

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

        public void InitializeMono(MonoCached mono)
        {
            if (mono == null) return;

            Resolver.Instance.Inject(mono);
            InvokeRise(mono);
            InvokeReady(mono);
            InvokeProcessSubscription(mono);
        }
        

        #region Invoke Reflection Methods
        private void InvokeRise(MonoCached mono)
        {
            riseMethod.Invoke(mono, null);
        }

        private void InvokeReady(MonoCached mono)
        {
            readyMethod.Invoke(mono, null);
        }

        private void InvokeProcessSubscription(MonoCached mono)
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
