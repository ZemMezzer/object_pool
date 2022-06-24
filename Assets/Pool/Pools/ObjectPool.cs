using System.Collections.Generic;

namespace ObjectPool
{
    public class ObjectPool<T> where T : class, new()
    {
        private static readonly Dictionary<T, ObjectPool<T>> PoolsPool = new Dictionary<T, ObjectPool<T>>();
        
        public static ObjectPool<T> Get(T instance, int fillAmount)
        {
            if (!PoolsPool.ContainsKey(instance))
            {
                ObjectPool<T> poolInstance = new ObjectPool<T>(instance);
                PoolsPool.Add(instance, poolInstance);
                poolInstance.Fill(fillAmount);
            }
            
            return PoolsPool[instance];
        }
        
        public static ObjectPool<T> Get(T instance)
        {
            return Get(instance, 0);
        }
        
        private readonly Stack<T> poolQueue = new Stack<T>();
        private readonly T poolObject;
        
        private ObjectPool(T instance)
        {
            poolObject = instance;
        }
        
        public T Get()
        {
            var instance = poolQueue.Count <= 0 ? new T() : poolQueue.Pop();
            return instance;
        }
        
        public void Put(T instance)
        {
            poolQueue.Push(instance);
        }
        
        public void Fill(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var instance = poolQueue.Count <= 0 ? new T() : poolQueue.Pop();
                Put(instance);
            }
        }
    }
}
