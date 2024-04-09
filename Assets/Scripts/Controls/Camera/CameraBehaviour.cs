using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;


public class CameraBehaviour : MonoBehaviour
{
    private PlayerWeaponManagerBehaviour _playerWeaponManagerBehaviour;

    private float xPosOffset;
    private float yPosOffset;
    private float zPosOffset;

    [HideInInspector] public Vector3 shakeValue = new Vector3(0, 0, 0);

    [HideInInspector] public bool isShaking;

    [SerializeField] private float cursorOffsetSensitivityX = 8f;
    [SerializeField] private float cursorOffsetSensitivityZ = 4f; // factor to multiply the cursor distance from center with
    [SerializeField] private float zOffset = 0f; // offset for the camera's z world position
    [SerializeField] private float maxOffset = 0.5f;

    private Transform playerTransform;
    private void Awake()
    {
      
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("FindPlayer", 0.1f);
      //  FindPlayer();
    }

    private void FindPlayer()
    {
        _playerWeaponManagerBehaviour = PlayerWeaponManagerBehaviour.Instance;
        playerTransform = _playerWeaponManagerBehaviour.gameObject.transform;
        var camPos = transform.position;
        xPosOffset = camPos.x;
        yPosOffset = camPos.y;
        zPosOffset = camPos.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            var playerPos = playerTransform.position;
            Vector3 pos = new Vector3();
            pos.x = playerPos.x + xPosOffset;
            pos.y = playerPos.y + yPosOffset;
            pos.z = playerPos.z + zPosOffset + zOffset; // add z offset

            Vector3 cursorPos = Input.mousePosition;
            cursorPos.z = Camera.main.transform.position.y; // set cursor position in the same plane as the camera
            cursorPos = Camera.main.ScreenToWorldPoint(cursorPos); // convert cursor position to world space

            float cursorDistance = Vector2.Distance(new Vector2(cursorPos.x, cursorPos.z), new Vector2(transform.position.x, transform.position.z));
    
            pos.x += Mathf.Clamp(cursorOffsetSensitivityX * 0.0001f * cursorDistance * (cursorPos.x - transform.position.x), -maxOffset, maxOffset); // add offset and reduce the factor by 10000 to x position
            pos.z += Mathf.Clamp(cursorOffsetSensitivityZ * 0.0001f * cursorDistance * (cursorPos.z - transform.position.z), -maxOffset, maxOffset); // add offset and reduce the factor by 10000 to z position

            transform.position = pos + shakeValue;
        }
        else
        {
            //Debug.LogWarning("Player has been destroyed, camera no longer has anything to follow");
        }
    }
}
