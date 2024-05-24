using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HammerZapEntityPool : MonoBehaviour
{
    private ObjectPool<HammerZapEntityBehaviour> pool;
    
        [SerializeField] private HammerZapEntityBehaviour hammerZapEntityPrefab = null;
        
        [SerializeField] private int inactiveCount;
        [SerializeField] private int activeCount;
        
        [SerializeField] private int allocationCount;
    
        private List<HammerZapEntityBehaviour> hammerZapEntityBehaviours = new List<HammerZapEntityBehaviour>();
        
        
        public ObjectPool<HammerZapEntityBehaviour> Pool
        {
            get { return pool; }
        }
        
        private void Awake()
        {
            pool = new ObjectPool<HammerZapEntityBehaviour>(CreatePopup, OnTakePopupFromPool, OnReturnPopupToPool);
            StartCoroutine(PreAllocateSpawns());
        }
        
        private HammerZapEntityBehaviour CreatePopup()
        {
            var popup = Instantiate(hammerZapEntityPrefab);
            popup.SetPool(pool);
            return popup;
        }
        private void OnTakePopupFromPool(HammerZapEntityBehaviour hammerZapEntityBehaviour)
        {
            hammerZapEntityBehaviour.gameObject.SetActive(true);
        }
    
        private void OnReturnPopupToPool(HammerZapEntityBehaviour hammerZapEntityBehaviour)
        {
            hammerZapEntityBehaviour.gameObject.SetActive(false);
        }
        
        private IEnumerator PreAllocateSpawns()
        {
            hammerZapEntityBehaviours.Clear();
    
            for (int i = 0; i < allocationCount; i++)
            {
                var popup = pool.Get();
                hammerZapEntityBehaviours.Add(popup);
            }
    
            foreach (HammerZapEntityBehaviour hammerZapEntityBehaviour in hammerZapEntityBehaviours)
            {
                hammerZapEntityBehaviour.ReturnToPool();
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
