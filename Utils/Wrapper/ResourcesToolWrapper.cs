﻿using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class ResourcesToolWrapper<T, T1>: ToolWrapperBase<T> where T: MonoCached where T1: ScriptableObject
    {
        protected T1 Data;
        
        /// <summary>
        /// For internal use
        /// </summary>
        /// <returns></returns>
        public abstract string GetDataPath();

        protected sealed override void Run()
        {
            LoadData();

            PostLoadRun();
        }

        protected abstract void PostLoadRun();

        private void LoadData()
        {
            Data = ResourcesUtils.ResolveScriptable<T1>(GetDataPath());
        }
    }
}