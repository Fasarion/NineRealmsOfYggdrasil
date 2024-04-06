using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUIBehaviour : MonoBehaviour
{
    private string _cardTitle;
    private string _cardDescription;
    private Sprite _cardSprite;
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardTitleText;
    [SerializeField] private TextMeshProUGUI cardDescriptionText;
    private int _upgradeObjectIndex;
    private RectTransform _transform;
    private UpgradeCardUIManager _manager;

    private void Awake()
    {
        _transform = this.GetComponent<RectTransform>();
        _manager = UpgradeCardUIManager.Instance;
    }

    private void PopulateDisplayValues(UpgradeObject upgradeObject)
    {
        _cardTitle = upgradeObject.upgradeTitle;
        _cardDescription = upgradeObject.upgradeDescription;
        _cardSprite = upgradeObject.upgradeSprite;
        _upgradeObjectIndex = upgradeObject.upgradeIndex;
    }

    public void UpdateCardDisplay(UpgradeObject upgradeObject)
    {
        PopulateDisplayValues(upgradeObject);

        cardTitleText.text = _cardTitle;
        cardDescriptionText.text = _cardDescription;
        cardImage.sprite = _cardSprite;
    }

    public void OnMouseHoverEnter()
    {
        _transform.localScale = new Vector3(.75f, .75f, .75f);
    }

    public void OnMouseHoverExit()
    {
        _transform.localScale = new Vector3(.5f, .5f, .5f);
    }

    public void RegisterMouseClick()
    {
        Debug.Log("Register mouse click");
        OnMouseHoverExit();
        _manager.RegisterUpgradeCardClick(_upgradeObjectIndex);

    }
}
