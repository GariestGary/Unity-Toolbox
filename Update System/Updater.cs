using System.Collections.Generic;
using UnityEngine;

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

        private List<MonoCached> monos = new List<MonoCached>();

        protected override void Run()
        {
            
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

                MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

                foreach (var mono in objMonos)
                {
                    if(Instance.monos.Contains(mono))
                    {
                        continue;
                    }

                    InvokeRise(mono);
                }
            }

            foreach (var obj in objs)
            {
                if (obj == null) return;

                MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

                foreach (var mono in objMonos)
                {
                    if (Instance.monos.Contains(mono))
                    {
                        continue;
                    }

                    InvokeReady(mono);
                }
            }

            foreach (var obj in objs)
            {
                if (obj == null) return;

                MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

                foreach (var mono in objMonos)
                {
                    if (Instance.monos.Contains(mono))
                    {
                        continue;
                    }

                    Instance.monos.Add(mono);
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

        public static void InitializeMono(MonoCached mono)
        {
            if (mono == null && !Instance.monos.Contains(mono)) return;

            InvokeRise(mono);
            InvokeReady(mono);
            Instance.monos.Add(mono);
        }

        public static void RemoveMonoFromUpdate(MonoCached mono)
        {
            if (mono == null) return;

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
