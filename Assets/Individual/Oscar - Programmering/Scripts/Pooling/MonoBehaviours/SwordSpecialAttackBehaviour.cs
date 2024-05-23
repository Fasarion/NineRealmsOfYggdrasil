using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SwordSpecialAttackBehaviour : MonoBehaviour
{
    private ObjectPool<SwordSpecialAttackBehaviour> pool = null;
    public void SetPool(ObjectPool<SwordSpecialAttackBehaviour> pool)
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
