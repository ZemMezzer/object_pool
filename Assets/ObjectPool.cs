using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class ObjectPool<T> where T : Component
    {
        private static readonly Dictionary<Component, ObjectPool<T>> PoolsPool = new Dictionary<Component, ObjectPool<T>>();

        public static ObjectPool<T> Get(Component instance, int fillAmount, HideFlags flags)
        {
            if (!PoolsPool.ContainsKey(instance))
            {
                ObjectPool<T> poolInstance = new ObjectPool<T>(instance, flags);
                PoolsPool.Add(instance, poolInstance);
                poolInstance.Fill(fillAmount);
            }
            
            return PoolsPool[instance];
        }

        public static ObjectPool<T> Get(Component instance, int fillAmount)
        {
            return Get(instance, fillAmount, HideFlags.None);
        }
        
        public static ObjectPool<T> Get(Component instance, HideFlags flags)
        {
            return Get(instance, 0, HideFlags.None);
        }

        public static ObjectPool<T> Get(Component instance)
        {
            return Get(instance, 0, HideFlags.None);
        }

        public static ObjectPool<T> Get(GameObject instance, int fillAmount, HideFlags flags)
        {
            return Get(instance.transform, fillAmount, flags);
        }
        
        public static ObjectPool<T> Get(GameObject instance, int fillAmount)
        {
            return Get(instance.transform, fillAmount, HideFlags.None);
        }
        
        public static ObjectPool<T> Get(GameObject instance, HideFlags flags)
        {
            return Get(instance.transform, 0, HideFlags.None);
        }
        
        public static ObjectPool<T> Get(GameObject instance)
        {
            return Get(instance.transform);
        }

        private readonly Stack<T> poolQueue = new Stack<T>();
        private readonly Component poolObject;
        private readonly HideFlags hideFlags;
        
        private ObjectPool(Component instance, HideFlags flags)
        {
            poolObject = instance;
            hideFlags = flags;
        }

        public T Get()
        {
            Component instance = null;
            if (poolQueue.Count <= 0)
            {
                instance = Object.Instantiate(poolObject) as T;
                instance.gameObject.SetActive(true);
                instance.hideFlags = hideFlags;
                
                if(instance is PoolObject poolObjectInstance)
                    poolObjectInstance.Initialize((obj) => Put(obj as T));
            }
            else
            {
                instance = poolQueue.Pop();
                instance.gameObject.SetActive(true);
            }
            
            return (T)instance;
        }

        public T Get(Vector3 position)
        {
            var instance = Get();
            instance.transform.position = position;
            return instance;
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            var instance = Get(position);
            instance.transform.rotation = rotation;
            return instance;
        }

        public T Get(Transform transform)
        {
            var instance = Get(transform.position, transform.rotation);
            instance.transform.SetParent(transform);
            return instance;
        }

        public void Put(T instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(null);
            poolQueue.Push(instance);
        }

        public void Fill(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var instance = Object.Instantiate(poolObject) as T;
                Put(instance);
            }
        }
    }
}
