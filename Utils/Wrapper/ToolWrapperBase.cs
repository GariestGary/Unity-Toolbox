namespace VolumeBox.Toolbox
{
    public abstract class ToolWrapperBase<T>: CachedSingleton<T>, IToolWrapper where T: MonoCached
    {
        private bool runned = false;

        public void RunInternal()
        {
            if(!runned)
            {
                Run();
                runned = true;
            }
        }

        public void ClearInternal()
        {
            Clear();
        }

        protected abstract void Run();

        protected abstract void Clear();
    }
}