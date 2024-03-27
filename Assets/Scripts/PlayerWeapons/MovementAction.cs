using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Unity.VisualScripting;


public class MovementAction : ModifierAction
{
    public enum TargetType{MousePosition, WorldPosition, RelativePosition}

    public TargetType targetType;

    public float movementSpeed;

    private GameObject movementObject;    
    private Vector3 movementTarget = new Vector3(0, 0, 0);
    private PlayerMovement playerMovement;

    private Vector3 ogPosition;



    public override void ExecuteAction()
    {
        Timing.RunCoroutine(RunAction());
    }

    public override IEnumerator<float> RunAction()
    {
        if (targetType == TargetType.MousePosition)
        {
            movementTarget = playerMovement.MousePos;
            
            while (true)
            {
                yield return 0;
            }
        }
    }

    public override void SetUpAction(Weapon weapon)
    {
        movementObject = weapon.GameObject();
        playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
    }
    
}
