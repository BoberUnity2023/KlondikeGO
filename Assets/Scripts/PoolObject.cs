using System;
using UnityEngine;

namespace BloomLines.ObjectPool
{
    public class PoolObject : MonoBehaviour
    {
        public Action<PoolObject> onMoveToPool;

        protected virtual void OnDisable()
        {
            onMoveToPool?.Invoke(this);
        }
    }
}