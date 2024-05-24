using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UltShockwavesBehaviour : MonoBehaviour
{
    private ObjectPool<UltShockwavesBehaviour> pool = null;
    public void SetPool(ObjectPool<UltShockwavesBehaviour> pool)
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
