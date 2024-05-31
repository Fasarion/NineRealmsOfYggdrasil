using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UpgradeCardUIManager : MonoBehaviour
{
    [SerializeField] private List<UpgradeCardUIBehaviour> upgradeCards;
    
    private static UpgradeCardUIManager _instance;

    private bool _isUIDisplayed;

    private UpgradeObject[] _upgradeObjects;

    public Action<int> OnUpgradeChosen;

    public float upgradeUIClickDelay = 0.5f;

    private float _upgradeUIClickDelayTimer = 0;

    private float _cachedTimeStamp;
    
    
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
        
        HideUI();
    }
    
    public Camera camera; // Reference to the camera
    public GameObject[] objectsToPlace; // Array of objects to place in front of the camera
    public float distanceInFront = 5.0f; // Distance in front of the camera
    public float duration = 2.0f; // Duration of the lowering animation
    public float spacing = 2.0f; // Horizontal spacing between objects
    public float delay = 1.0f; // Delay before the effect starts

    void Start()
    {
        // Start the coroutine to lower the objects one by one
        //StartCoroutine(LowerObjectsSequentially());
    }

    IEnumerator LowerObjectsSequentially()
    {
        yield return new WaitForSeconds(delay);
        
        // Calculate the central target position in front of the camera
        Vector3 cameraPosition = camera.transform.position;
        Vector3 cameraForward = camera.transform.forward;
        Vector3 centerPosition = cameraPosition + cameraForward * distanceInFront;

        // Get the world position of the top of the screen
        Vector3 topOfScreen = camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, distanceInFront));

        // Calculate the starting positions and target positions for each object
        for (int i = 0; i < objectsToPlace.Length; i++)
        {
            float offset = (i - (objectsToPlace.Length - 1) / 2.0f) * spacing;
            Vector3 targetPosition = centerPosition + camera.transform.right * offset;
            Vector3 startPosition = topOfScreen + camera.transform.right * offset;

            // Set the initial position of the object
            objectsToPlace[i].transform.position = startPosition;

            // Optional: Align the object to face the same direction as the camera
            //objectsToPlace[i].transform.rotation = camera.transform.rotation;

            // Start the coroutine to lower the current object
            yield return StartCoroutine(LowerObject(objectsToPlace[i], startPosition, targetPosition));
        }
    }

    IEnumerator LowerObject(GameObject obj, Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;

        // Gradually move the object from startPosition to targetPosition
        while (elapsedTime < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.fixedUnscaledDeltaTime * 10;
            yield return null;
        }

        // Ensure the object is exactly at the target position at the end
        obj.transform.position = targetPosition;
    }

    private void Update()
    {
        _upgradeUIClickDelayTimer = Time.unscaledTime;
    }

    private void DisplayUpgradeCards(UpgradeObject[] upgradeObjects)
    {
        _cachedTimeStamp = Time.unscaledTime;
        
        ShowUI(upgradeObjects.Length);
        
        for (int i = 0; i < upgradeObjects.Length; i++)
        {
            UpgradeObject upg = upgradeObjects[i];
            upgradeCards[i].UpdateCardDisplay(upg);
        }
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

        _isUIDisplayed = true;
        //Time.timeScale = 0f;
        EventManager.OnPause?.Invoke(PauseType.FreezeGame);
    }

    private void HideUI()
    {
        foreach (var card in upgradeCards)
        {
            card.gameObject.SetActive(false);
        }

        _isUIDisplayed = false;
        //Time.timeScale = 1f;
        EventManager.OnUnpause?.Invoke(PauseType.FreezeGame);
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
        if (_upgradeUIClickDelayTimer < _cachedTimeStamp + upgradeUIClickDelay) return;
        
        HideUI();
        OnUpgradeChosen?.Invoke(index);
    }
}
