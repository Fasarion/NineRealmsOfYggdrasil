using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerWeaponManagerBehaviour _playerWeaponManagerBehaviour;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _playerWeaponManagerBehaviour = PlayerWeaponManagerBehaviour.Instance;
        _camera.gameObject.transform.position = _playerWeaponManagerBehaviour.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
