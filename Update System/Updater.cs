using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using BeauRoutine;

namespace VolumeBox.Toolbox
{
    public class Updater : Singleton<Updater>, IRunner
    {
        //TODO: modifiers clears at start, try to consistent set to list, maybe cancel inherit from scriptableobjects
        [SerializeField] protected List<UpdateModifier> modifiers;
        private List<MonoCached> monos = new List<MonoCached>();

        private float timeScale = 1;
        private float delta;
        
        public float UnscaledDelta => Time.deltaTime;
        public float Delta => delta;
        public event Action<float> deltaTick;
        public event Action<float> fixedDeltaTick;

        public float TimeScale
        {
            get
            {
                return timeScale;
            } 
            set
            {
                if(value < 0)
                {
                    Routine.TimeScale = 0f;
                    timeScale = 0f;
                } 
                else if(value > 1)
                {
                    Routine.TimeScale = 1f;
                    timeScale = 1f;
                } 
                else
                {
                    Routine.TimeScale = value;
                    timeScale = value;
                }
            }
        }

        [Button("Refresh Modifiers")]
        public void RefreshModifiers()
        {
            if(modifiers == null)
            {
                modifiers = new List<UpdateModifier>();
            }

            var modifierTypes = Assembly
            .GetAssembly(typeof(UpdateModifier))
            .GetTypes()
            .Where(x => typeof(UpdateModifier).IsAssignableFrom(x) && !x.IsAbstract);

            foreach (var modifierType in modifierTypes)
            {
                UpdateModifier newModifier = ScriptableObject.CreateInstance(modifierType) as UpdateModifier;

                if(!modifiers.Any(x => x.GetType() == newModifier.GetType()))
                {
                    modifiers.Add(newModifier);
                }
            }
        }

        [Button("Clear Modifiers")]
        public void ClearModifiers()
        {
            if(modifiers != null)
            {
                modifiers.Clear();
            }
            else
            {
                modifiers = new List<UpdateModifier>();
            }
        }

        public void Run()
        {
            //modifiers = new List<UpdateModifier>();
            TimeScale = 1;
        }

        /// <summary>
        /// Injects given GameObjects, rises and ready them, and then adds them to process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public void InitializeObjects(GameObject[] objs)
        {
            MonoCached[] objsMonos = new MonoCached[]{};

            foreach(var obj in objs)
            {
                Resolver.Instance.Inject(obj); 
                objsMonos = objsMonos.Concat(obj.GetComponentsInChildren<MonoCached>(true)).ToArray();
            }

            foreach(var mono in objsMonos)
            {
                mono.Rise();
            }

            foreach (var mono in objsMonos)
            {
                mono.Ready();
            }

            foreach(var mono in objsMonos)
            {
                AddMonoToProcess(mono);
            }
        }

        /// <summary>
        /// Injects given GameObject, rises and ready it, and then adds it to process
        /// </summary>
        /// <param name="obj"></param>
        public void InitializeObject(GameObject obj)
        {
            Resolver.Instance.Inject(obj);

            MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

            foreach (var mono in objMonos)
            {
                mono.Rise();
            }

            foreach (var mono in objMonos)
            {
                mono.Ready();
                AddMonoToProcess(mono);
            }
        }

        public void InitializeMono(MonoCached mono)
        {
            Resolver.Instance.Inject(mono);
            mono.Rise();
            mono.Ready();
            AddMonoToProcess(mono);
        }

        private void AddMonoToProcess(MonoCached mono)
        {
            if(!monos.Contains(mono))
            {
                mono.Activate();
                monos.Add(mono);
            }
        }

        private void RemoveMonoFromProcess(MonoCached mono)
        {
            if(monos.Contains(mono))
            {
                mono.Deactivate();
                mono.OnRemove();
                monos.Remove(mono);
            }
        }

        public void RemoveObjectFromUpdate(GameObject obj)
        {
            MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

            foreach (var mono in objMonos)
            {
                RemoveMonoFromProcess(mono);
            }
        }

        public void RemoveObjectsFromUpdate(GameObject[] objs)
        {
            for (var i = 0; i < objs.Length; i++)
            {
                RemoveObjectFromUpdate(objs[i]);
            }
        }

        void Update()
        {
            delta = Time.deltaTime * TimeScale;
            deltaTick?.Invoke(delta);

            if(monos != null && monos.Count > 0)
            {
                for (var i = 0; i < monos.Count; i++)
                {
                    var mono = monos[i];

                    if(modifiers != null && modifiers.Count > 0)
                    {    
                        foreach(var modifier in modifiers)
                        {
                            float? modifiedDelta = modifier?.Modify(delta, mono);
                            delta = modifiedDelta.Value;
                        }
                    }

                    if(mono.Interval > 0)
                    {
                        if(mono.IntervalTimer >= mono.Interval)
                        {
                            mono.Process(mono.TimeStack);
                            mono.TimeStack = 0;
                            mono.IntervalTimer -= mono.Interval;
                        }
                        mono.IntervalTimer += delta;
                        mono.TimeStack += delta;
                    }
                    else
                    {
                        mono.Process(delta);
                    }

                }
            }

            Routine.ManualUpdate();
        }

        void FixedUpdate()
        {
            float fixedDelta = Time.fixedDeltaTime * timeScale;
            fixedDeltaTick?.Invoke(fixedDelta);

            if(monos != null && monos.Count > 0)
            {
                for (var i = 0; i < monos.Count; i++)
                {
                    var mono = monos[i];

                    if(mono.Interval > 0)
                    {
                        if(mono.IntervalTimer >= mono.Interval)
                        {
                            mono.FixedProcess(mono.FixedTimeStack);
                            mono.FixedTimeStack = 0;
                        }
                        mono.FixedTimeStack += fixedDelta;
                    }
                    else
                    {
                        mono.FixedProcess(fixedDelta);
                    }
                }

                
            }
        }

        void LateUpdate()
        {
            if(monos != null && monos.Count > 0)
            {
                for (var i = 0; i < monos.Count; i++)
                {
                    var mono = monos[i];

                    if(modifiers != null && modifiers.Count > 0)
                    {
                        foreach(var modifier in modifiers)
                        {
                            modifier.Modify(delta, mono);
                        }
                    }

                    if(mono.Interval > 0)
                    {
                        if(mono.IntervalTimer >= mono.Interval)
                        {
                            //Time stack counting in Process method
                            mono.LateProcess(mono.TimeStack);
                        }
                    }
                    else
                    {
                        mono.LateProcess(delta);
                    }
                }
            }
        }
    }
}
