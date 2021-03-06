using System;
using UnityEngine;

namespace ObjectPool
{
    public abstract class PoolObject : MonoBehaviour
    {
        private Action<PoolObject> onRelease;
        private bool isInitialized;
        
        
        internal void Initialize(Action<PoolObject> onReleaseCallback)
        {
            if (isInitialized)
            {
                Debug.LogError("Attempt to Reinitialize Initialized Object");
                return;
            }
            
            onRelease = onReleaseCallback;
            isInitialized = true;
        }

        public void Put()
        {
            onRelease?.Invoke(this);
        }

        private void OnDisable()
        {
            onRelease?.Invoke(this);
            OnObjectDisable();
        }
        
        protected virtual void OnObjectDisable(){}
    }
}
