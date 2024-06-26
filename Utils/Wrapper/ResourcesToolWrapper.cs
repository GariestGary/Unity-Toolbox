using UnityEngine;

namespace VolumeBox.Toolbox
{
    public abstract class ResourcesToolWrapper<T, T1>: ToolWrapperBase<T> where T: MonoCached where T1: ScriptableObject
    {
        private T1 m_Data;

        protected T1 Data
        {
            get
            {
                if(m_Data == null)
                {
                    LoadData();
                }

                return m_Data;
            }
            private set { }
        }
        
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
            m_Data = ResourcesUtils.ResolveScriptable<T1>(GetDataPath());
        }
    }
}