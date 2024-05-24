using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IndicatorBehaviour : MonoBehaviour
{
    private ObjectPool<IndicatorBehaviour> pool = null;
    public void SetPool(ObjectPool<IndicatorBehaviour> pool)
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
