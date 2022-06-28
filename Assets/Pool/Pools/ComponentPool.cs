using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class ComponentPool<T> : UnityObjectPool<T> where T : Component
    {
        private static readonly Dictionary<Component, ComponentPool<T>> PoolsPool = new Dictionary<Component, ComponentPool<T>>();

        public static ComponentPool<T> Get(Component instance, int fillAmount, HideFlags flags, bool dontDestroyOnLoad)
        {
            if (!PoolsPool.ContainsKey(instance))
            {
                ComponentPool<T> poolInstance = new ComponentPool<T>(instance, flags, dontDestroyOnLoad);
                PoolsPool.Add(instance, poolInstance);
                poolInstance.Fill(fillAmount);
            }
            
            return PoolsPool[instance];
        }

        public static ComponentPool<T> Get(Component instance, int fillAmount)
        {
            return Get(instance, fillAmount, HideFlags.None, false);
        }
        
        public static ComponentPool<T> Get(Component instance, HideFlags flags)
        {
            return Get(instance, 0, flags, false);
        }
        
        public static ComponentPool<T> Get(Component instance, bool dontDestroyOnLoad)
        {
            return Get(instance, 0, HideFlags.None, dontDestroyOnLoad);
        }
        
        public static ComponentPool<T> Get(Component instance, int fillAmount, bool dontDestroyOnLoad)
        {
            return Get(instance, fillAmount, HideFlags.None, dontDestroyOnLoad);
        }
        
        public static ComponentPool<T> Get(Component instance, HideFlags flags, bool dontDestroyOnLoad)
        {
            return Get(instance, 0, flags, dontDestroyOnLoad);
        }
        
        public static ComponentPool<T> Get(GameObject instance, int fillAmount)
        {
            return Get(instance.transform, fillAmount, HideFlags.None, false);
        }
        
        public static ComponentPool<T> Get(GameObject instance, HideFlags flags)
        {
            return Get(instance.transform, 0, flags, false);
        }
        
        public static ComponentPool<T> Get(GameObject instance, bool dontDestroyOnLoad)
        {
            return Get(instance.transform, 0, HideFlags.None, dontDestroyOnLoad);
        }
        
        public static ComponentPool<T> Get(GameObject instance, int fillAmount, bool dontDestroyOnLoad)
        {
            return Get(instance.transform, fillAmount, HideFlags.None, dontDestroyOnLoad);
        }
        
        public static ComponentPool<T> Get(GameObject instance, HideFlags flags, bool dontDestroyOnLoad)
        {
            return Get(instance.transform, 0, flags, dontDestroyOnLoad);
        }
        
        public static ComponentPool<T> Get(GameObject instance, int fillAmount, HideFlags flags)
        {
            return Get(instance.transform, fillAmount, flags, false);
        }
        
        public static ComponentPool<T> Get(GameObject instance)
        {
            return Get(instance.transform);
        }
        

        public static ComponentPool<T> Get(Component instance)
        {
            return Get(instance, 0, HideFlags.None, false);
        }

        private ComponentPool(Component instance, HideFlags flags, bool dontDestroy) : base(instance, flags, dontDestroy)
        {
            PoolObject = instance;
            HideFlags = flags;
            DontDestroyOnLoad = dontDestroy;
        }

        public override T Get()
        {
            Component instance = null;
            if (PoolQueue.Count <= 0)
            {
                instance = Object.Instantiate(PoolObject) as T;
                instance.gameObject.SetActive(true);
                instance.gameObject.hideFlags = HideFlags;

                if(DontDestroyOnLoad)
                    Object.DontDestroyOnLoad(instance.gameObject);
                
                if(instance is IPoolObject poolObjectInstance)
                    poolObjectInstance.Initialize((obj) => Put(obj as T));
            }
            else
            {
                instance = PoolQueue.Pop();
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

        public override void Put(T instance)
        {
            if(!instance.gameObject.activeInHierarchy)
                return;
            
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(null);
            PoolQueue.Push(instance);
            
            if(DontDestroyOnLoad)
                Object.DontDestroyOnLoad(instance.gameObject);
        }
    }
}
