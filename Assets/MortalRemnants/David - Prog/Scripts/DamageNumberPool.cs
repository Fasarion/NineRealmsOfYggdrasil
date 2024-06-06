using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageNumberPool : MonoBehaviour
{
    private ObjectPool<DamagePopup> pool;

    [SerializeField] private DamagePopup popupPrefab = null;
    
    [SerializeField] private int inactiveCount;
    [SerializeField] private int activeCount;
    
    [SerializeField] private int allocationCount;

    private List<DamagePopup> popups = new List<DamagePopup>();
    
    
    public ObjectPool<DamagePopup> Pool
    {
        get { return pool; }
    }
    
    private void Awake()
    {
        popups.Clear();
        pool = new ObjectPool<DamagePopup>(CreatePopup, OnTakePopupFromPool, OnReturnPopupToPool);
        StartCoroutine(PreAllocateSpawns());
    }
    
    private DamagePopup CreatePopup()
    {
        var popup = Instantiate(popupPrefab);
        popup.SetPool(pool);
        return popup;
    }
    private void OnTakePopupFromPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(true);
    }

    private void OnReturnPopupToPool(DamagePopup popup)
    {
        popup.gameObject.SetActive(false);
    }
    
    private IEnumerator PreAllocateSpawns()
    {
        popups.Clear();

        for (int i = 0; i < allocationCount; i++)
        {
            var popup = pool.Get();
            popups.Add(popup);
        }

        foreach (DamagePopup popup in popups)
        {
            popup.ReturnToPool();
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
