using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace KKD
{
    public enum QuestType
    {
        TalkToQuest,
        FetchQuest,
        KillQuest,
        CollectQuest,
        
    }

    [CreateAssetMenu(fileName = "QuestHandler", menuName = "EventHandlers/QuestHandler" + "")]
    public class QuestHandler : ScriptableObject
    {

        [SerializeField] private int currentQuestState;
        public QuestType questType;
        
        
        
        public bool questActive;
        public bool questTasksComplete;
        public bool questComplete;
        public bool questAccepted;
        public bool firstTimeAccepting;

        //TalkToQuest
        [SerializeField]private string characterToTalkTo;
        
        //FetchQuest
        [ReadOnly, SerializeField] private bool fetchItemRetrieved;
        public string itemToFetchName;
        [SerializeField] private string characterToFetchFrom;
        [SerializeField]private string characterToFetchFor;
        
        //CollectQuest
        public string collectableItemName;
        public List<MonoBehaviour> itemTypesToCollect;
        [SerializeField] private int numberOfItemsToCollect;
        [SerializeField]private int currentCollectedItems;
        [SerializeField]private int startingCollectedItems;
        
        //KillQuest
        public string enemyTypeName;
        public Component enemyTypeToKill;
        [SerializeField] private int startingKills = 0;
        [SerializeField] private int killGoal = 100;
        [SerializeField]private int currentKills;
        
        


        public void OnValidate()
        {
            
        }

        public QuestType GetQuest(out  QuestHandler questHandler)
        {
            questHandler = this;
            return questType;
        }

        public void Awake()
        {
            firstTimeAccepting = true;
        }

        public void OnDisable()
        {
            
        }

        public void AddKill(Component component)
        {
            if (questType == QuestType.KillQuest)
            {
                if (component.GetType() == enemyTypeToKill.GetType())
                {
                    if (currentKills < killGoal)
                    {
                        currentKills += 1;
                        if (currentKills >= killGoal)
                        {
                            MaxKillsAchieved();
                        }
                        else
                        {
                            EnemyKilled();
                        }
                    }
                }
            }
            
           
            
        }


        public void AddItem(MonoBehaviour monoBehaviour)
        {
            if (questType == QuestType.CollectQuest)
            {
                bool sameType;

                for (int i = 0; i < itemTypesToCollect.Count; i++)
                {
                    if (monoBehaviour.GetType() == itemTypesToCollect[i].GetType())
                    {
                        sameType = true;
                    }
                    else
                    {
                        sameType = false;
                    }

                    if (sameType && currentCollectedItems < numberOfItemsToCollect)
                    {
                        Debug.Log("Collected " + itemTypesToCollect[i].name);
                        currentCollectedItems += 1;
                        if (currentCollectedItems >= numberOfItemsToCollect)
                        {
                            MaxItemsCollected();
                        }
                        else
                        {
                            ItemCollected();
                        }
                    }
                }
               
                
            }
        }


        public int GetKillGoal()
        {
            return killGoal;
        }
        
        public int GetCurrentQuestState()
        {
            return currentQuestState;
        }
        
        public void SetCurrentQuestState(int state)
        {
            currentQuestState = state;
           
        }

        public int GetCollectGoal()
        {
            return numberOfItemsToCollect;
        }

        public int GetCurrentKills()
        {
            return currentKills;
        }

        public int GetCurrentCollectedItems()
        {
            return currentCollectedItems;
        }

        public bool GetCurrentFetchStatus()
        {
            return fetchItemRetrieved;
        }
        
        
        public event Action<QuestHandler> onCollect;
        //public event Action<QuestHandler> onAddItem;

        
        public static void OnCollect(Component component)
        {
            
        }
        public event Action<QuestHandler> onQuestActivated;
        
        public static event Action<QuestHandler> onQuestCompleted;
        public event Action onQuestTasksCompleted;
        public static event Action onQuestTasksCompletedGlobal;
        public event Action onMaxKillsAchieved;
        public event Action onKill;
        
        public event Action onMaxItemsCollected;
        //public event Action onCollect;

        public event Action onItemFetched;

        public string GetTalkToName()
        {
            return characterToTalkTo;
        }
        
        public string GetFetchNames(out string characterToFetchFrom, out string characterToFetchFor)
        {
            characterToFetchFrom = this.characterToFetchFrom;
            characterToFetchFor = this.characterToFetchFor;
            return itemToFetchName;
        }

        public void QuestTasksCompleted()
        {
            questTasksComplete = true;
            onQuestTasksCompleted?.Invoke();
            onQuestTasksCompletedGlobal?.Invoke();
        }
        
        //Fetch quest does not allow multiple pickups.
        public void ItemFetched()
        { 
            fetchItemRetrieved = true;
            QuestTasksCompleted();
           
            onItemFetched?.Invoke();
        }
        private void MaxItemsCollected()
        {
            //questTasksComplete = true;
            onCollect?.Invoke(this);
            QuestTasksCompleted();
            //onQuestTasksCompleted?.Invoke();
            //onQuestTasksCompletedGlobal?.Invoke();
            onMaxItemsCollected?.Invoke();
        }
        private void ItemCollected()
        {
            onCollect?.Invoke(this);
        }

        private void MaxKillsAchieved()
        {
            
            onKill?.Invoke();
            QuestTasksCompleted();
            onMaxKillsAchieved?.Invoke();
        }

        private void EnemyKilled()
        {
            onKill?.Invoke();
        }

        public void QuestCompleted(QuestHandler handler)
        {
            questComplete = true;
            onQuestCompleted?.Invoke(handler);
        }

        public void QuestActivated()
        {
            questActive = true;
            onQuestActivated?.Invoke(this);
        }

        public void ResetItemCollect()
        {
            currentCollectedItems = startingCollectedItems;
        }
        public void ResetKills()
        {
            currentKills = startingKills;
        }
        
        public void ResetQuestState()
        {
            currentQuestState = 0;
            questTasksComplete = false;
        }

        public void ResetFetch()
        {
            fetchItemRetrieved = false;
        }

        private void OnEnable()
        {
            switch (questType)
            {
                case QuestType.KillQuest:
                    currentKills = startingKills;
                    break;
                case QuestType.CollectQuest:
                    currentCollectedItems = startingCollectedItems;
                    break;
                case QuestType.FetchQuest:
                    fetchItemRetrieved = false;
                    break;
                case QuestType.TalkToQuest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}
