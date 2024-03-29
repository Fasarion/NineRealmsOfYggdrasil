using System;
using System.Collections.Generic;
using DS.ScriptableObjects;
using TMPro;
using UnityEngine;


public delegate void DialogueEventTrigger(DialogueContainer dialogueStart);

public enum TriggerType
{
    Script,
    Counter,
    Collider,
    Interaction
};

public enum CounterType
{
    Gold,
    Health,
    XP,
    Level,
    Timer
    
}

public enum DialogueSystemType
{
    Legacy,
    DialogueGraph
}
    public class DialogueTrigger : MonoBehaviour
    {
        [HideInInspector]public DialogueManager dialogueManager;
        private DialogueTriggerReceiver dialogueTriggerReceiver;
        
        [HideInInspector]public bool dialogueAdded = false;
        private bool canOpenDialogue = true;

        private InteractCubeRotation interactCubeRotation;
        
        public TriggerType triggerType;
        public CounterType counterType;
        
        public DialogueSystemType dialogueSystemType;
        public DialogueContainer startingDialogueBranch;
        public GameObject interactUIText;

        public bool destroyExclamationMarkOnTrigger = false;
        public GameObject exclamationMarkObject;
        public bool destroyParentOnTrigger = false;

        public bool destroySelfOnTrigger = false;
        
        
        
        public float counter;
        public float countGoal;
        public bool countingDown;

        
        private DialogueContainerGeneratorBehaviour dialogueContainerGeneratorBehaviour;
        public DSDialogueContainerSO dialogueContainerScriptableObject;

        

        private bool counterDone = false;
        private void Awake()
        {
            if (dialogueSystemType == DialogueSystemType.DialogueGraph)
            {
               dialogueContainerGeneratorBehaviour = FindObjectOfType<DialogueContainerGeneratorBehaviour>();
               dialogueContainerGeneratorBehaviour.Initialize();
                ContainerParent dialogueParent;
                dialogueContainerGeneratorBehaviour.dialogueContainerParents.TryGetValue(
                    dialogueContainerScriptableObject.name, out dialogueParent);
                startingDialogueBranch = dialogueParent.containers[0];

            }
            if (triggerType != TriggerType.Interaction)
            {
                interactUIText = null;
            }
            else
            {
                interactUIText = FindObjectOfType<InteractCheck>(true).gameObject;
            }
            
            dialogueManager = FindObjectOfType<DialogueManager>();
            dialogueManager.dialogueClosed += OnDialogueClosed;
            dialogueTriggerReceiver = FindObjectOfType<DialogueTriggerReceiver>();

            if (triggerType == TriggerType.Counter)
            {
                
            }
            
        }

        public void OnDisable()
        {
            dialogueManager.dialogueClosed -= OnDialogueClosed;
            if (triggerType == TriggerType.Counter)
            {
              
            }
        }

        //TODO: vi borde sätta det h�r i en coroutine ist�llet f�r att kolla update
        public void Update()
        {
            if (counterType == CounterType.Timer && counterDone == false)
            {
                if (countingDown)
                {
                    counter -= Time.deltaTime;
                }
                else
                {
                    counter += Time.deltaTime;
                }
                
                CheckCounterDone();
            }
        }

        private void SetGold(float amount)
        {
            if (counterType == CounterType.Gold)
            {
                counter = amount;
                CheckCounterDone();
            }
        }

        private void SetHealth(float amount)
        {
            if (counterType == CounterType.Health)
            {
                counter = amount;
                CheckCounterDone();
            }
        }

        private void SetXP(float amount)
        {
            if (counterType == CounterType.XP)
            {
                counter = amount;
                CheckCounterDone();
            }
        }

        private void SetLevel(int amount)
        {
            if (counterType == CounterType.Level)
            {
                counter = amount;
                CheckCounterDone();
            }
        }

        public void CheckCounterDone()
        {
            if (countingDown)
            {
                if (counter <= countGoal)
                {
                    if (counterDone != true)
                    {
                        OnCounter(); 
                        counterDone = true;
                    }
                
                }
                
                
            }
            else
            {
                if (counter >= countGoal)
                {
                    if (counterDone != true)
                    {
                        OnCounter(); 
                        counterDone = true;
                    }
                
                }
            }
            
        }

        public void OnCounter()
        {
            if (dialogueAdded == false)
            {
                dialogueManager.EnqueueDialogue(startingDialogueBranch);
                dialogueAdded = true;
            }
            if (dialogueManager.dialogueOpen) return;
            TriggerDialogue();
        }

        public void OnInteract()
        {
            if (canOpenDialogue)
            {
                canOpenDialogue = false;
                if (dialogueAdded == false)
                {
                    dialogueManager.EnqueueDialogue(startingDialogueBranch);
                    
                    dialogueAdded = true;
                }
                if (dialogueManager.dialogueOpen) return;
                TriggerDialogue();
               
            }
           
        }

        public void OnCollider()
        {
            if (dialogueAdded == false)
            {
                dialogueManager.EnqueueDialogue(startingDialogueBranch);
                dialogueAdded = true;
            }
            if (dialogueManager.dialogueOpen) return;
            TriggerDialogue();
        }

      

        public void OnEvent()
        {
            dialogueManager.useAutomaticDialogueSkip = true;
            //This is hardcoded which is very bad but there is not enough time to do much about that right now.
            dialogueManager.dialogueSkipTimer = 3;
            if (dialogueAdded == false)
            {
                dialogueManager.EnqueueDialogue(startingDialogueBranch);
                dialogueAdded = true;
            }
            if (dialogueManager.dialogueOpen) return;
            TriggerDialogue();
        }
        
        private void OnDialogueClosed()
        {
            Debug.Log("Dialogue was closed");
            dialogueAdded = false;
            canOpenDialogue = false;
        }
        
        

        public void TriggerDialogue()
        {
            dialogueManager.dialogueOpen = true;
            dialogueManager.firstTimeDialogueOpened = true;
            
            
            if (triggerType == TriggerType.Interaction)
            {
                interactUIText.SetActive(false);
            }
            else if (triggerType == TriggerType.Script)
            {
                if (dialogueAdded == false)
                {
                    dialogueManager.EnqueueDialogue(startingDialogueBranch);
                    dialogueAdded = true;
                }
                if (dialogueManager.dialogueOpen) return;
            }
            


            if (destroyExclamationMarkOnTrigger)
            {
                Destroy(exclamationMarkObject);
            }
            
            if (destroyParentOnTrigger)
            {
                Destroy(gameObject.transform.parent.gameObject);
            } 
            else if (destroySelfOnTrigger)
            {
                Destroy(gameObject);
            }
            
            
            
            
            
            
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (triggerType == TriggerType.Interaction)
            {
                canOpenDialogue = true;
                interactUIText.SetActive(true);
                dialogueTriggerReceiver.ReceiveCurrentDialogueTrigger(this);
            }
            else if (triggerType == TriggerType.Collider)
            {
                OnCollider();
            }
           
            
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (triggerType == TriggerType.Interaction)
            {
                canOpenDialogue = false;
                interactUIText.SetActive(false);
                dialogueTriggerReceiver.ReceiveCurrentDialogueTrigger(null);
            }

        }
    }
