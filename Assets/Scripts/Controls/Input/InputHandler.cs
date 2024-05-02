using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class InputHandler : MonoBehaviour
{
    [HideInInspector]public GameObject playerInputObject;
    [HideInInspector]public GameObject dialogueInputObject;
    [HideInInspector]public GameObject dialogueTriggerTransmitterInputObject;

    public bool invertControls;
    public bool useDialogue;
    public Command buttonF;
    public Command buttonE;
    public Command buttonSpacebar;
    public Command buttonEnter;
    public Command buttonMouse0Down;
    public Command pleaseCompileForMe;
    public Command buttonMouse1Down;
    public Command buttonMouse0Hold;
    public Command buttonMouse0Up;
    public Command inputAxis; 
    
    // Start is called before the first frame update
    void Awake()
    {
        playerInputObject = FindObjectOfType<PlayerBehaviour>().gameObject;
        /*if (FindObjectOfType<DialogueManager>() != null)
        {
            dialogueInputObject = FindObjectOfType<DialogueManager>().gameObject;
        }
        if (dialogueInputObject != null)
        {
            BindCommand(new TriggerDialogueCommand(dialogueInputObject), out buttonE);
            BindCommand(new DialogueCommand(dialogueInputObject), out buttonF);
            
            
            
        }*/

        BindCommand(new MovementCommand(playerInputObject), out inputAxis);

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }
    
    public void GetInput()
    {
        
        GetAxisInput();
        
        if(Input.GetMouseButtonDown(0))
        {
            //buttonMouse0Down.Execute();
        }
        
        if (Input.GetMouseButton(0))
        {
            //buttonMouse0Hold.Execute();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            //buttonMouse0Up.Execute();
        }
        

        if (Input.GetKeyDown(KeyCode.F)&& dialogueInputObject != null)
        {
            buttonE.Execute();
            buttonF.Execute();
            
        }

        /*if (Input.GetKeyDown(KeyCode.E) && dialogueInputObject != null)
        {
            
        }*/

      

        

        if (Input.GetKey(KeyCode.Space))
        {
           
            //buttonSpacebar.Execute();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            buttonEnter.Execute();
        }
    }

    public void GetAxisInput()
    {
        Vector3 movement = new Vector3();
        if (invertControls)
        {
            movement.x = -Input.GetAxisRaw("Horizontal");
            movement.z = -Input.GetAxisRaw("Vertical");
        }
        else
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");
        }
        
        inputAxis.ExecuteRawAxis(movement);
    }
    
    public void BindCommand(Command command,out Command keyBinding)
    {
        keyBinding = command;
    }
}
