using System.Collections;
using System.Collections.Generic;
using DevLocker.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomChoiceUIBehaviour : MonoBehaviour
{
    private RoomChoiceUIManager manager;
    
    [SerializeField] private Image roomImage;
    [SerializeField] private TextMeshProUGUI roomTitleText;
    [SerializeField] private TextMeshProUGUI roomDescriptionText;
    
    private string roomName;
    private string roomDescription;
    private Sprite roomSprite;
    private SceneReference roomSceneReference;

    [SerializeField] public RoomNode roomNode;
    
    
    // Start is called before the first frame update
    private void Awake()
    {
        manager = RoomChoiceUIManager.Instance;
    }
    
    private void PopulateDisplayValues(RoomChoiceObject newRoomChoiceObject)
    {
        roomName = newRoomChoiceObject.roomName;
        roomDescription = newRoomChoiceObject.roomDescription;
        roomSprite = newRoomChoiceObject.roomSprite;
        roomSceneReference = newRoomChoiceObject.roomSceneReference;
    }
    
    public void UpdateRoomSelectionDisplay(RoomChoiceObject newRoomChoiceObject)
    {
        PopulateDisplayValues(newRoomChoiceObject);

        roomTitleText.text = roomName;
        roomDescriptionText.text = roomDescription;
        roomImage.sprite = roomSprite;
    }




    public void RegisterMouseClick()
    {
        manager.RegisterRoomSelectionClick(roomSceneReference);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
