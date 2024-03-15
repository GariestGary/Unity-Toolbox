using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Class that controls lifecycle of <see cref="MonoCached">MonoCached</see> objects
    /// </summary>
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
                else
                {
                    timeScale = value;
                }
            }
        }
        public static float Delta => delta;

        private List<MonoCached> monos = new List<MonoCached>();

        protected override void Run()
        {
            
        }

        protected override void Clear()
        {
            
        }

        /// <summary>
        /// Invokes Rise and Ready on given GameObjects, and then adds them to process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public static void InitializeObjects(GameObject[] objs)
        {
            MonoCached[] monos = new MonoCached[0];

            foreach(var obj in objs)
            {
                var components = obj.GetComponentsInChildren<MonoCached>(true);
                monos = monos.Concat(components).ToArray();
            }

            foreach (var mono in monos)
            {
                if (mono == null || Instance.monos.Contains(mono))
                {
                    continue;
                }

                InvokeRise(mono);
            }

            foreach (var mono in monos)
            {
                if (mono == null || Instance.monos.Contains(mono))
                {
                    continue;
                }

                InvokeReady(mono);
            }

            foreach (var mono in monos)
            {
                if (mono == null || Instance.monos.Contains(mono))
                {
                    continue;
                }

                Instance.monos.Add(mono);
            }
        }

        /// <summary>
        /// Removes all GameObjects from process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public static void RemoveObjectsFromUpdate(GameObject[] objs)
        {
            MonoCached[] monos = new MonoCached[0];

            foreach (var obj in objs)
            {
                var components = obj.GetComponentsInChildren<MonoCached>(true);
                monos = monos.Concat(components).ToArray();
            }

            foreach (var mono in monos)
            {
                RemoveMonoFromUpdate(mono);
            }
        }

        /// <summary>
        /// Invokes Rise and Ready on given GameObject, and then adds it to process
        /// </summary>
        /// <param name="obj"></param>
        public static void InitializeObject(GameObject obj)
        {
            if (obj == null) return;

            MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

            foreach (var mono in objMonos)
            {
                if (Instance.monos.Contains(mono))
                {
                    continue;
                }

                InvokeRise(mono);
            }

            foreach (var mono in objMonos)
            {
                if (Instance.monos.Contains(mono))
                {
                    continue;
                }

                InvokeReady(mono);
            }

            foreach (var mono in objMonos)
            {
                if (Instance.monos.Contains(mono))
                {
                    continue;
                }

                Instance.monos.Add(mono);
            }
        }

        /// <summary>
        /// Invokes Rise and Ready on given MonoCached, and then adds it to process
        /// </summary>
        public static void InitializeMono(MonoCached mono)
        {
            if (mono == null && !Instance.monos.Contains(mono)) return;

            InvokeRise(mono);
            InvokeReady(mono);
            Instance.monos.Add(mono);
            mono.Resume();
        }

        /// <summary>
        /// Removes given MonoCached from process
        /// </summary>
        public static void RemoveMonoFromUpdate(MonoCached mono)
        {
            if (mono == null) return;

            mono.Pause();
            Instance.monos.Remove(mono);
        }
        

        #region Invoke Reflection Methods
        private static void InvokeRise(MonoCached mono)
        {
            mono.ProcessInternal(3, 0);
        }

        private static void InvokeReady(MonoCached mono)
        {
            mono.ProcessInternal(4, 0);
        }
        #endregion

        #region Updates
        private void Update()
        {
            delta = Time.deltaTime * TimeScale;

            for (int i = 0; i < monos.Count; i++)
            {
                monos[i].ProcessInternal(0, delta);
            }
        }

        private void FixedUpdate()
        {
            float fixedDelta = Time.fixedDeltaTime * timeScale;

            for (int i = 0; i < monos.Count; i++)
            {
                monos[i].ProcessInternal(1, fixedDelta);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < monos.Count; i++)
            {
                monos[i].ProcessInternal(2, delta);
            }
        }
        #endregion
    }
}
