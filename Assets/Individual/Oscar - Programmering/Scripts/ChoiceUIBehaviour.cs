using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ChoiceUIBehaviour : MonoBehaviour
{
    
    protected ChoiceUIManager manager;
    
    
    [SerializeField] protected TextMeshProUGUI choiceItemTitleText;
    [SerializeField] protected TextMeshProUGUI choiceItemDescriptionText;
    [SerializeField] protected Image choiceItemImage;
    
    protected string choiceItemName;
    protected string choiceItemDescription;
    protected Sprite choiceItemSprite;
    // Start is called before the first frame update
    
    private void Awake()
    {
        manager = ChoiceUIManager.Instance;
       
    }
    protected virtual void PopulateDisplayValues(ChoiceObject newChoiceObject)
    {
        choiceItemTitleText.text = newChoiceObject.choiceObjectName;
        choiceItemDescriptionText.text = newChoiceObject.choiceObjectDescription;
        choiceItemSprite = newChoiceObject.choiceObjectSprite;
    }
    
    
    public virtual void UpdateSelectionDisplay(ChoiceObject newChoiceObject)
    {
        PopulateDisplayValues(newChoiceObject);

        choiceItemTitleText.text = choiceItemName;
        choiceItemDescriptionText.text = choiceItemDescription;
        if (choiceItemImage != null)
        {
            choiceItemImage.sprite = choiceItemSprite;
        }
        
    }
    
    public virtual void RegisterMouseClick()
    {
    }
}
