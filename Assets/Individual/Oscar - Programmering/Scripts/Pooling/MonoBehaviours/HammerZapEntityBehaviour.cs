using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HammerZapEntityBehaviour : MonoBehaviour
{
    private ObjectPool<HammerZapEntityBehaviour> pool = null;
    public void SetPool(ObjectPool<HammerZapEntityBehaviour> pool)
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
