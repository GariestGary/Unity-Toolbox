namespace VolumeBox.Toolbox
{
    public interface IPooled<T>: IPooledBase
    {
        void OnSpawn(T data);
    }

    public interface IPooledBase { }
}
