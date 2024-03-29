using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCommand : Command
{
    private readonly PlayerMovement playerMovement;
    public MovementCommand(GameObject receiver): base(receiver)
    {
        playerMovement = this.receiver.GetComponent<PlayerMovement>();
    }

    public override void ExecuteRawAxis(Vector3 inputAxisVector)
    {
        playerMovement.GetMovementInput(inputAxisVector);
    }
}
