using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectPool
{
    public class UnityObjectPool<T> where T : Object
    {
        private static readonly Dictionary<Object, UnityObjectPool<T>> PoolsPool = new Dictionary<Object, UnityObjectPool<T>>();

        public static UnityObjectPool<T> Get(Object instance, int fillAmount, HideFlags flags, bool dontDestroyOnLoad)
        {
            if (!PoolsPool.ContainsKey(instance))
            {
                UnityObjectPool<T> poolInstance = new UnityObjectPool<T>(instance, flags, dontDestroyOnLoad);
                PoolsPool.Add(instance, poolInstance);
                poolInstance.Fill(fillAmount);
            }
            
            return PoolsPool[instance];
        }

        public static UnityObjectPool<T> Get(Object instance, int fillAmount)
        {
            return Get(instance, fillAmount, HideFlags.None, false);
        }
        
        public static UnityObjectPool<T> Get(Object instance, HideFlags flags)
        {
            return Get(instance, 0, flags, false);
        }
        
        public static UnityObjectPool<T> Get(Object instance, bool dontDestroyOnLoad)
        {
            return Get(instance, 0, HideFlags.None, dontDestroyOnLoad);
        }
        
        public static UnityObjectPool<T> Get(Object instance, int fillAmount, bool dontDestroyOnLoad)
        {
            return Get(instance, fillAmount, HideFlags.None, dontDestroyOnLoad);
        }
        
        public static UnityObjectPool<T> Get(Object instance, HideFlags flags, bool dontDestroyOnLoad)
        {
            return Get(instance, 0, flags, dontDestroyOnLoad);
        }

        public static UnityObjectPool<T> Get(Object instance)
        {
            return Get(instance, 0, HideFlags.None, false);
        }

        protected readonly Stack<T> PoolQueue = new Stack<T>();
        protected Object PoolObject;
        protected HideFlags HideFlags;
        protected bool DontDestroyOnLoad;
        
        private readonly Stack<T> fillStack = new Stack<T>();

        protected UnityObjectPool(Object instance, HideFlags flags, bool dontDestroy)
        {
            PoolObject = instance;
            HideFlags = flags;
            DontDestroyOnLoad = dontDestroy;
        }

        public virtual T Get()
        {
            Object instance = null;
            if (PoolQueue.Count <= 0)
            {
                instance = Object.Instantiate(PoolObject) as T;
                if (instance != null)
                    instance.hideFlags = HideFlags;
                

                if(DontDestroyOnLoad)
                    Object.DontDestroyOnLoad(instance);
                
                if(instance is PoolObject poolObjectInstance)
                    poolObjectInstance.Initialize((obj) => Put(obj as T));
            }
            else
            {
                instance = PoolQueue.Pop();
            }
            
            return (T)instance;
        }

        public virtual void Put(T instance)
        {
            PoolQueue.Push(instance);
        }
        
        public void Fill(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                fillStack.Push(Get());
            }

            int count = fillStack.Count;
            for (int i = 0; i < count; i++)
            {
                Put(fillStack.Pop());
            }
        }
    }
}
