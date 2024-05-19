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
    [SerializeField] private float standardScale = 0.5f; 
    [SerializeField] private float hoverScale = 0.75f; 
    private int _upgradeObjectIndex;
    private RectTransform _transform;
    private UpgradeCardUIManager _manager;
    private AudioManager _audioManager;
    

    private void Awake()
    {
        _transform = this.GetComponent<RectTransform>();
        _manager = UpgradeCardUIManager.Instance;
        
    }

    private void Start()
    {
        if (!_audioManager)
        {
            _audioManager = AudioManager.Instance;
        }
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
        OnMouseHoverExit();

        cardTitleText.text = _cardTitle;
        cardDescriptionText.text = _cardDescription;
        cardImage.sprite = _cardSprite;
    }

    public void OnMouseHoverEnter()
    {
        _transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);
    }

    public void OnMouseHoverExit()
    {
        _transform.localScale = new Vector3(standardScale, standardScale, standardScale);
    }

    public void RegisterMouseClick()
    {

        _manager.RegisterUpgradeCardClick(_upgradeObjectIndex);
        _audioManager.uiAudio.MenuClickAudio();

    }
}
