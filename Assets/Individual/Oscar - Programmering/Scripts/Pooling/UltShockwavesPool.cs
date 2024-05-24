using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UltShockwavesPool : MonoBehaviour
{
     private ObjectPool<UltShockwavesBehaviour> pool;
    
        [SerializeField] private UltShockwavesBehaviour ultShockwavesPrefab = null;
        
        [SerializeField] private int inactiveCount;
        [SerializeField] private int activeCount;
        
        [SerializeField] private int allocationCount;
    
        private List<UltShockwavesBehaviour> ultShockwavesBehaviours = new List<UltShockwavesBehaviour>();
        
        
        public ObjectPool<UltShockwavesBehaviour> Pool
        {
            get { return pool; }
        }
        
        private void Awake()
        {
            pool = new ObjectPool<UltShockwavesBehaviour>(CreatePopup, OnTakePopupFromPool, OnReturnPopupToPool);
            StartCoroutine(PreAllocateSpawns());
        }
        
        private UltShockwavesBehaviour CreatePopup()
        {
            var popup = Instantiate(ultShockwavesPrefab);
            popup.SetPool(pool);
            return popup;
        }
        private void OnTakePopupFromPool(UltShockwavesBehaviour ultShockwavesBehaviour)
        {
            ultShockwavesBehaviour.gameObject.SetActive(true);
        }
    
        private void OnReturnPopupToPool(UltShockwavesBehaviour ultShockwavesBehaviour)
        {
            ultShockwavesBehaviour.gameObject.SetActive(false);
        }
        
        private IEnumerator PreAllocateSpawns()
        {
            ultShockwavesBehaviours.Clear();
    
            for (int i = 0; i < allocationCount; i++)
            {
                var popup = pool.Get();
                ultShockwavesBehaviours.Add(popup);
            }
    
            foreach (UltShockwavesBehaviour ultShockwavesBehaviour in ultShockwavesBehaviours)
            {
                ultShockwavesBehaviour.ReturnToPool();
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
