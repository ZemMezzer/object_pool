using System;

namespace ObjectPool
{
    public interface IPoolObject
    {
        public void Initialize(Action<PoolObject> onReleaseCallback);
        public void Put();
    }
}
