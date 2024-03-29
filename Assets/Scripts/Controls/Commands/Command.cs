using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    protected GameObject receiver;

    protected Command(GameObject receiver)
    {
        this.receiver = receiver;
    }
    public virtual void Execute()
    {
        
    }
    
    public virtual void ExecuteRawAxis(Vector3 inputAxisVector)
    {
        
    }
}
