using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VolumeBox.Toolbox
{
    /// <summary>
    /// Class that controls lifecycle of <see cref="MonoCached">MonoCached</see> objects
    /// </summary>
    public class Updater : MonoBehaviour, IClear
    {
        private float _InternalTimeScale = 1;
        private float _InternalDelta;
        
        public float UnscaledDelta => Time.deltaTime;
        public float TimeScale
        {
            get
            {
                return _InternalTimeScale;
            } 
            set
            {
                if(value < 0)
                {
                    _InternalTimeScale = 0f;
                }
                else
                {
                    _InternalTimeScale = value;
                }
            }
        }
        public float Delta => _InternalDelta;

        private List<MonoCached> _RunningMonos = new List<MonoCached>();
        private List<Action<float>> _CustomTicks = new List<Action<float>>();
        private List<Action<float>> _CustomFixedTicks = new List<Action<float>>();
        private List<Action<float>> _CustomLateTicks = new List<Action<float>>();

        #region Custom Processes

        public void AddCustomTick(Action<float> tick)
        {
            _CustomTicks.Add(tick);
        }

        public void AddCustomFixedTick(Action<float> tick)
        {
            _CustomFixedTicks.Add(tick);
        }

        public void AddCustomLateTick(Action<float> tick)
        {
            _CustomLateTicks.Add(tick);
        }

        public void RemoveCustomTick(Action<float> tick)
        {
            _CustomTicks.Remove(tick);
        }

        public void RemoveCustomFixedTick(Action<float> tick)
        {
            _CustomFixedTicks.Remove(tick);
        }

        public void RemoveCustomLateTick(Action<float> tick)
        {
            if (_CustomLateTicks.Contains(tick))
            {
                _CustomLateTicks.Remove(tick);
            }
        }

        #endregion
        
        /// <summary>
        /// Invokes Rise and Ready on given GameObjects, and then adds them to process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public void InitializeObjects(GameObject[] objs)
        {
            MonoCached[] monos = new MonoCached[0];

            for (int i = 0; i < objs.Length; i++)
            {
                var components = objs[i].GetComponentsInChildren<MonoCached>(true);
                monos = monos.Concat(components).ToArray();
            }

            for (int i = 0; i < monos.Length; i++)
            {
                var mono = monos[i];
                
                if (mono == null || _RunningMonos.Contains(mono))
                {
                    continue;
                }

                InvokeRise(mono);
            }

            for (int i = 0; i < monos.Length; i++)
            {
                var mono = monos[i];
                
                if (mono == null || _RunningMonos.Contains(mono))
                {
                    continue;
                }

                InvokeReady(mono);
            }

            for (int i = 0; i < monos.Length; i++)
            {
                var mono = monos[i];
                
                if (mono == null || _RunningMonos.Contains(mono))
                {
                    continue;
                }

                _RunningMonos.Add(mono);
            }
        }

        /// <summary>
        /// Removes all GameObjects from process
        /// </summary>
        /// <param name="objs">Array of GameObjects</param>
        public void RemoveObjectsFromUpdate(GameObject[] objs)
        {
            MonoCached[] monos = new MonoCached[0];

            for (int i = 0; i < objs.Length; i++)
            {
                var components = objs[i].GetComponentsInChildren<MonoCached>(true);
                monos = monos.Concat(components).ToArray();
            }

            for (int i = 0; i < monos.Length; i++)
            {
                RemoveMonoFromUpdate(monos[i]);
            }
        }

        /// <summary>
        /// Invokes Rise and Ready on given GameObject, and then adds it to process
        /// </summary>
        /// <param name="obj"></param>
        public void InitializeObject(GameObject obj)
        {
            if (obj == null) return;

            MonoCached[] objMonos = obj.GetComponentsInChildren<MonoCached>(true);

            for (int i = 0; i < objMonos.Length; i++)
            {
                var mono = objMonos[i];
                
                if (_RunningMonos.Contains(mono))
                {
                    continue;
                }

                InvokeRise(mono);
            }

            for (int i = 0; i < objMonos.Length; i++)
            {
                var mono = objMonos[i];
                
                if (_RunningMonos.Contains(mono))
                {
                    continue;
                }

                InvokeReady(mono);
            }
            
            for (int i = 0; i < objMonos.Length; i++)
            {
                var mono = objMonos[i];
                
                if (_RunningMonos.Contains(mono))
                {
                    continue;
                }

                _RunningMonos.Add(mono);
            }
        }

        /// <summary>
        /// Invokes Rise and Ready on given MonoCached, and then adds it to process
        /// </summary>
        public void InitializeMono(MonoCached mono)
        {
            if (mono == null && !_RunningMonos.Contains(mono)) return;

            InvokeRise(mono);
            InvokeReady(mono);
            _RunningMonos.Add(mono);
            mono.Resume();
        }

        /// <summary>
        /// Removes given MonoCached from process
        /// </summary>
        public void RemoveMonoFromUpdate(MonoCached mono)
        {
            if (mono == null) return;

            mono.Pause();
            _RunningMonos.Remove(mono);
        }
        
        private void InvokeRise(MonoCached mono)
        {
            mono.OnRise();
        }

        private void InvokeReady(MonoCached mono)
        {
            mono.OnReady();
        }

        #region Updates
        private void Update()
        {
            _InternalDelta = Time.deltaTime * TimeScale;

            for (int i = 0; i < _RunningMonos.Count; i++)
            {
                var deltaToUse = _RunningMonos[i].IgnoreTimeScale ? Time.deltaTime : _InternalDelta;
                _RunningMonos[i].ProcessControl(deltaToUse);
            }

            foreach (var tick in _CustomTicks)
            {
                tick?.Invoke(_InternalDelta);
            }
        }

        private void FixedUpdate()
        {
            float fixedDelta = Time.fixedDeltaTime * _InternalTimeScale;

            for (int i = 0; i < _RunningMonos.Count; i++)
            {
                var deltaToUse = _RunningMonos[i].IgnoreTimeScale ? Time.fixedDeltaTime : fixedDelta;
                _RunningMonos[i].FixedProcessControl(deltaToUse);
            }

            foreach (var fixedTick in _CustomFixedTicks)
            {
                fixedTick?.Invoke(fixedDelta);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < _RunningMonos.Count; i++)
            {
                var deltaToUse = _RunningMonos[i].IgnoreTimeScale ? Time.deltaTime : _InternalDelta;
                _RunningMonos[i].LateProcessControl(deltaToUse);
            }

            foreach (var lateTick in _CustomLateTicks)
            {
                lateTick?.Invoke(_InternalDelta);
            }
        }
        #endregion

        public void Clear()
        {
            
        }
    }
}
