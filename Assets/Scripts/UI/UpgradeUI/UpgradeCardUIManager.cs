using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UpgradeCardUIManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeCardUIBehaviour> upgradeCards;
    [SerializeField] private List<GameObject> upgradeStones;
    [SerializeField] private GameObject birdStonePrefab;
    [SerializeField] private GameObject hammerStonePrefab;
    [SerializeField] private GameObject swordStonePrefab;
    [SerializeField] private GameObject playerStonePrefab;
    private Vector3[] _uICardPositions;

    private UpgradeObject[] upgradeObjects;
    
    private static UpgradeCardUIManager _instance;

    private bool _isUIDisplayed;

    private UpgradeObject[] _upgradeObjects;

    public Action<int> OnUpgradeChosen;

    public float upgradeUIClickDelay = 0.5f;

    private float _upgradeUIClickDelayTimer = 0;

    private float _cachedTimeStamp;

    public float uiOffset;

    private bool _isStartup;
    
    
    public static UpgradeCardUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UpgradeCardUIManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }

        _uICardPositions = new Vector3[3];
        HideUI();
    }
    
    public Camera camera; // Reference to the camera
    public float distanceInFront = 5.0f; // Distance in front of the camera
    public float duration = 2.0f; // Duration of the lowering animation
    public float spacing = 2.0f; // Horizontal spacing between objects
    public float delay = 1.0f; // Delay before the effect starts

    void Start()
    {
        // Start the coroutine to lower the objects one by one
        //StartCoroutine(LowerObjectsSequentially());
    }

    IEnumerator LowerObjectsSequentially(List<GameObject> objectsToPlace)
    {
        //yield return new WaitForSeconds(delay);
        
        // Calculate the central target position in front of the camera
        Vector3 cameraPosition = camera.transform.position;
        Vector3 cameraForward = camera.transform.forward;
        Vector3 centerPosition = cameraPosition + cameraForward * distanceInFront;

        // Get the world position of the top of the screen
        Vector3 topOfScreen = camera.ViewportToWorldPoint(new Vector3(0.5f, 2f, distanceInFront));

        // Calculate the starting positions and target positions for each object
        for (int i = 0; i < objectsToPlace.Count; i++)
        {
            float offset = (i - (objectsToPlace.Count - 1) / 2.0f) * spacing;
            Vector3 targetPosition = centerPosition + camera.transform.right * offset;
            Vector3 startPosition = topOfScreen + camera.transform.right * offset;

            // Set the initial position of the object
            objectsToPlace[i].transform.position = startPosition;

            // Optional: Align the object to face the same direction as the camera
            //objectsToPlace[i].transform.rotation = camera.transform.rotation;
            float currentDelay = delay * i;
            // Start the coroutine to lower the current object
            StartCoroutine(LowerObject(objectsToPlace[i], startPosition, targetPosition, i, currentDelay));
        }

        yield return null;
    }

    IEnumerator LowerObject(GameObject obj, Vector3 startPosition, Vector3 targetPosition, int i, float delay)
    {
        float elapsedTime = 0f;
        if (i == 0) elapsedTime = delay;

        while (elapsedTime < delay)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Gradually move the object from startPosition to targetPosition
        while (elapsedTime < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime * 10;
            yield return null;
        }

        // Ensure the object is exactly at the target position at the end
        obj.transform.position = targetPosition;
        _uICardPositions[i] = targetPosition;
        if(i == upgradeObjects.Length - 1) DisplayUpgradeText(upgradeObjects);
    }

    private void Update()
    {
        if (!_isUIDisplayed) return;
        
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            upgradeCards[i].transform.position = camera.WorldToScreenPoint(_uICardPositions[i] + new Vector3(uiOffset, 0, 0));
        }
    }

    private void DisplayUpgradeCards(UpgradeObject[] upgradeObjects)
    {
        this.upgradeObjects = upgradeObjects;

        if (this.upgradeObjects.Length <= 0) return;
        
        //ShowUI(upgradeObjects.Length);
        
        EventManager.OnPause?.Invoke(PauseType.FreezeGame);
        
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            UpgradeObject upg = upgradeObjects[i];
            var prefab = GetUpgradeStonePrefab(upg);
            var stone = GameObject.Instantiate(prefab);
            upgradeStones.Add(stone);
        }
        
        StartCoroutine(LowerObjectsSequentially(upgradeStones));
    }

    private void DisplayUpgradeText(UpgradeObject[] upgradeObjects)
    {
        
        ShowUI(upgradeObjects.Length);
        
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            UpgradeObject upg = upgradeObjects[i];
            upgradeCards[i].UpdateCardDisplay(upg);
            upgradeCards[i].transform.position = camera.WorldToScreenPoint(_uICardPositions[i]);
        }
    }

    private GameObject GetUpgradeStonePrefab(UpgradeObject upg)
    {
        GameObject gameObject;
        switch (upg.upgrades[0].thingToUpgrade)
        {
            case UpgradeBaseType.Birds:
                gameObject = birdStonePrefab;
                return gameObject;
            case UpgradeBaseType.Hammer:
                gameObject = hammerStonePrefab;
                return gameObject;
            case UpgradeBaseType.Sword:
                gameObject = swordStonePrefab;
                return gameObject;
            case UpgradeBaseType.Player:
                gameObject = playerStonePrefab;
                return gameObject;
        }

        gameObject = playerStonePrefab;
        return gameObject;
    }

    private void OnEnable()
    {
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        upgradeUISystem.OnUpgradeUIDisplayCall += DisplayUpgradeCards;
    }

    private void OnDisable()
    {
        if (World.DefaultGameObjectInjectionWorld == null) return;
        var upgradeUISystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<UpgradeUISystem>();
        upgradeUISystem.OnUpgradeUIDisplayCall -= DisplayUpgradeCards;
    }

    private void ShowUI(int cardCount)
    {
        for (int i = 0; i < cardCount; i++)
        {
            upgradeCards[i].gameObject.SetActive(true);
        }

        EventManager.OnDisableUI();

        _isUIDisplayed = true;
        //Time.timeScale = 0f;

    }

    private void HideUI()
    {
        foreach (var card in upgradeCards)
        {
            card.gameObject.SetActive(false);
        }

        foreach (var stone in upgradeStones)
        {
            GameObject.Destroy(stone);
        }
        upgradeStones = new List<GameObject>();

        _isUIDisplayed = false;
        //Time.timeScale = 1f;
        EventManager.OnUnpause?.Invoke(PauseType.FreezeGame);

        if (!_isStartup)
        {
            _isStartup = true;
        }
        else EventManager.OnEnableUI();
        
    }

    public RectTransform[] GetUpgradeCardDimensions(UpgradeObject[] upgradeObjects)
    {
        int cardsToDisplay = upgradeObjects.Length;
        RectTransform[] transforms = new RectTransform[cardsToDisplay];
        for (int i = 0; i < cardsToDisplay; i++)
        {
            transforms[i] = upgradeCards[i].GetComponent<RectTransform>();
        }

        return transforms;
    }
    
    public void RegisterUpgradeCardClick(int index)
    {
        //if (_upgradeUIClickDelayTimer < _cachedTimeStamp + upgradeUIClickDelay) return;
        
        HideUI();
        OnUpgradeChosen?.Invoke(index);
    }
}
