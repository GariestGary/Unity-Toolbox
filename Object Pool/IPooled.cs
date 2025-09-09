namespace VolumeBox.Toolbox
{
    public interface IPooled<T>: IPooledBase
    {
        void OnSpawn(T data);
    }

    public interface IPooled: IPooledBase
    {
        void OnSpawn();
    }

    public interface IPooledBase
    {
        
    }
}
