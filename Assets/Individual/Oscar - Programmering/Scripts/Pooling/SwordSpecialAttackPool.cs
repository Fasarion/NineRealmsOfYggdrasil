using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SwordSpecialAttackPool : MonoBehaviour
{
    
    private ObjectPool<SwordSpecialAttackBehaviour> pool;

    [SerializeField] private SwordSpecialAttackBehaviour specialAttackPrefab = null;
    
    [SerializeField] private int inactiveCount;
    [SerializeField] private int activeCount;
    
    [SerializeField] private int allocationCount;

    private List<SwordSpecialAttackBehaviour> swordSpecialAttackBehaviours = new List<SwordSpecialAttackBehaviour>();
    
    
    public ObjectPool<SwordSpecialAttackBehaviour> Pool
    {
        get { return pool; }
    }
    
    private void Awake()
    {
        pool = new ObjectPool<SwordSpecialAttackBehaviour>(CreatePopup, OnTakePopupFromPool, OnReturnPopupToPool);
        StartCoroutine(PreAllocateSpawns());
    }
    
    private SwordSpecialAttackBehaviour CreatePopup()
    {
        var popup = Instantiate(specialAttackPrefab);
        popup.SetPool(pool);
        return popup;
    }
    private void OnTakePopupFromPool(SwordSpecialAttackBehaviour specialAttackBehaviour)
    {
        specialAttackBehaviour.gameObject.SetActive(true);
    }

    private void OnReturnPopupToPool(SwordSpecialAttackBehaviour specialAttackBehaviour)
    {
        specialAttackBehaviour.gameObject.SetActive(false);
    }
    
    private IEnumerator PreAllocateSpawns()
    {
        swordSpecialAttackBehaviours.Clear();

        for (int i = 0; i < allocationCount; i++)
        {
            var popup = pool.Get();
            swordSpecialAttackBehaviours.Add(popup);
        }

        foreach (SwordSpecialAttackBehaviour swordSpecialAttackBehaviour in swordSpecialAttackBehaviours)
        {
            swordSpecialAttackBehaviour.ReturnToPool();
        }
        
        UpdateDebugCount();

        yield return null;
    }
    
    public void UpdateDebugCount()
    {
        activeCount = pool.CountActive;
        inactiveCount = pool.CountInactive;
    }
}
