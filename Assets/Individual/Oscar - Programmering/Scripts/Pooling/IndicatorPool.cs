using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IndicatorPool : MonoBehaviour
{
     private ObjectPool<IndicatorBehaviour> pool;
    
        [SerializeField] private IndicatorBehaviour indicatorPrefab = null;
        
        [SerializeField] private int inactiveCount;
        [SerializeField] private int activeCount;
        
        [SerializeField] private int allocationCount;
    
        private List<IndicatorBehaviour> indicatorBehaviours = new List<IndicatorBehaviour>();
        
        
        public ObjectPool<IndicatorBehaviour> Pool
        {
            get { return pool; }
        }
        
        private void Awake()
        {
            pool = new ObjectPool<IndicatorBehaviour>(CreatePopup, OnTakePopupFromPool, OnReturnPopupToPool);
            StartCoroutine(PreAllocateSpawns());
        }
        
        private IndicatorBehaviour CreatePopup()
        {
            var popup = Instantiate(indicatorPrefab);
            popup.SetPool(pool);
            return popup;
        }
        private void OnTakePopupFromPool(IndicatorBehaviour indicatorBehaviour)
        {
            indicatorBehaviour.gameObject.SetActive(true);
        }
    
        private void OnReturnPopupToPool(IndicatorBehaviour indicatorBehaviour)
        {
            indicatorBehaviour.gameObject.SetActive(false);
        }
        
        private IEnumerator PreAllocateSpawns()
        {
            indicatorBehaviours.Clear();
    
            for (int i = 0; i < allocationCount; i++)
            {
                var popup = pool.Get();
                indicatorBehaviours.Add(popup);
            }
    
            foreach (IndicatorBehaviour indicatorBehaviour in indicatorBehaviours)
            {
                indicatorBehaviour.ReturnToPool();
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
