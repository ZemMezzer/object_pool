using System;
using System.Collections;
using UnityEngine;

namespace ObjectPool.Demo
{
    public class SomeBehaviour : MonoBehaviour
    {
        [SerializeField] private PooledObject pooledObjectPrefab;
        private ComponentObjectPool<PooledObject> pooledObjectsPool;
        
        private readonly WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(5);

        private void Awake()
        {
            pooledObjectsPool = ComponentObjectPool<PooledObject>.Get(pooledObjectPrefab, 100, HideFlags.HideInHierarchy | HideFlags.HideInInspector, true);
        }

        private IEnumerator Start()
        {
            yield return waitForSecondsRealtime;
            
            Vector3 offset = Vector3.zero;
            
            for (int i = 0; i < 100; i++)
            {
                if(i % 2==0 && i != 0)
                    offset += Vector3.right;
                
                pooledObjectsPool.Get(i%2 == 0 ? offset : -offset);

                yield return null;
            }
        }
    }
}
