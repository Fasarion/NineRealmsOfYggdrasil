using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HammerThrowIndicatorPool : MonoBehaviour
{
      private ObjectPool<HammerThrowIndicatorBehaviour> pool;
    
        [SerializeField] private HammerThrowIndicatorBehaviour hammerThrowIndicatorPrefab = null;
        
        [SerializeField] private int inactiveCount;
        [SerializeField] private int activeCount;
        
        [SerializeField] private int allocationCount;
    
        private List<HammerThrowIndicatorBehaviour> hammerThrowIndicatorBehaviours = new List<HammerThrowIndicatorBehaviour>();
        
        
        public ObjectPool<HammerThrowIndicatorBehaviour> Pool
        {
            get { return pool; }
        }
        
        private void Awake()
        {
            pool = new ObjectPool<HammerThrowIndicatorBehaviour>(CreatePopup, OnTakePopupFromPool, OnReturnPopupToPool);
            StartCoroutine(PreAllocateSpawns());
        }
        
        private HammerThrowIndicatorBehaviour CreatePopup()
        {
            var popup = Instantiate(hammerThrowIndicatorPrefab);
            popup.SetPool(pool);
            return popup;
        }
        private void OnTakePopupFromPool(HammerThrowIndicatorBehaviour hammerThrowIndicatorBehaviour)
        {
            hammerThrowIndicatorBehaviour.gameObject.SetActive(true);
        }
    
        private void OnReturnPopupToPool(HammerThrowIndicatorBehaviour hammerThrowIndicatorBehaviour)
        {
            hammerThrowIndicatorBehaviour.gameObject.SetActive(false);
        }
        
        private IEnumerator PreAllocateSpawns()
        {
            hammerThrowIndicatorBehaviours.Clear();
    
            for (int i = 0; i < allocationCount; i++)
            {
                var popup = pool.Get();
                hammerThrowIndicatorBehaviours.Add(popup);
            }
    
            foreach (HammerThrowIndicatorBehaviour hammerThrowIndicatorBehaviour in hammerThrowIndicatorBehaviours)
            {
                hammerThrowIndicatorBehaviour.ReturnToPool();
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
