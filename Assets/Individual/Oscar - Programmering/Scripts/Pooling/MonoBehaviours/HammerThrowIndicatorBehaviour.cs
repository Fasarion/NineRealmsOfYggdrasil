using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HammerThrowIndicatorBehaviour : MonoBehaviour
{
    private ObjectPool<HammerThrowIndicatorBehaviour> pool = null;
    public void SetPool(ObjectPool<HammerThrowIndicatorBehaviour> pool)
    {
        this.pool = pool;
    }

    public void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
