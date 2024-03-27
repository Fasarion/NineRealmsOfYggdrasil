using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FMOD.Studio;
//using FMODUnity;
using MEC;
//using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PlayerMovement : MonoBehaviour
{
    //public PlayerAudio playerAudio;
    [Header("Object References")]
    public Camera cam;
    private Rigidbody rBody;
    [SerializeField] private Animator playerAnimator;
    
    [Header("Move Parameters")]
    public float moveSpeed;
    private float speed;
    
    private Vector3 movement;
    
    //Mouse look
    private Vector3 lookDirection;
    private Quaternion lookRotation;
    private Vector3 mousePos;
    [HideInInspector] public Vector3 movementDirection;

    private bool canDash = true;

    
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    private float dashTimer;
    public bool isDashing;

    public GameObject dashVfx;

    //private PlayerMovementAnimationEventHandler animationEventHandler;

    public Vector3 MousePos
    {
        get
        {
            return mousePos;
        }
    }
    
    private IEnumerator<float> Dash()
    {

        canDash = false;
        isDashing = true;
        dashVfx.SetActive(true);
        speed = dashSpeed;
        //FMODUnity.RuntimeManager.PlayOneShot(playerAudio.dash);

        //yield return Timing.WaitForSeconds(dashTime);

        dashVfx.SetActive(false);
        speed = moveSpeed;
        isDashing = false;
        
        //speed = moveSpeed;

        yield return Timing.WaitForSeconds(dashCooldown);
        canDash = true;
        
        

    }

    private void OnEnable()
    {
        //animationEventHandler = FindObjectOfType<PlayerMovementAnimationEventHandler>();
        //animationEventHandler.onFootstep += OnFootstep;
        dashVfx.SetActive(false);
        speed = moveSpeed;
        canDash= true;
    }

    private void OnFootstep()
    {
        //FMODUnity.RuntimeManager.PlayOneShot(playerAudio.footsteps);
    }

    private void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        //playerAnimator = GetComponentInChildren<Animator>();
        
    }

private void FixedUpdate()
{
    if (isDashing)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, movement.normalized, out hit, movement.magnitude * speed * Time.fixedDeltaTime,Physics.AllLayers,QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.CompareTag("Enemy"))
            {
                // If there is a collision, stop the player's movement
                movement = Vector3.zero;
            }
            return;
        }
    }


    // If there is no collision, move the player
    rBody.MovePosition(rBody.position + movement.normalized * speed * Time.fixedDeltaTime);
    rBody.MoveRotation(lookRotation.normalized);
    
    UpdateSound();
}

    // Update is called once per frame
    void Update()
    {
        playerAnimator.SetFloat("speed", movementDirection.magnitude);
        
        PointPlayerTowardsMouse();

        if(Input.GetKeyDown(KeyCode.Space) && canDash) 
        {
            Timing.RunCoroutine(Dash());
        }
    }

    public void GetMovementInput(Vector3 movement)
    {
        this.movement = movement;
        movementDirection = movement.normalized;
        
        float forwardBackwardsMagnitude = 0;
        float rightLeftMagnitude = 0;
        if (this.movement.magnitude > 0) {
            //Vector3 normalizedLookingAt = direction - transform.position;
            //normalizedLookingAt.Normalize ();
            forwardBackwardsMagnitude = Mathf.Clamp (
                Vector3.Dot (this.movement, lookDirection), -1, 1
            );
 
            Vector3 perpendicularLookingAt = new Vector3 (
                lookDirection.z, 0, -lookDirection.x
            );
            rightLeftMagnitude = Mathf.Clamp (
                Vector3.Dot (this.movement, perpendicularLookingAt), -1, 1
            );
        }
 
        // update the animator parameters
        playerAnimator.SetFloat ("yDirection", forwardBackwardsMagnitude);
        playerAnimator.SetFloat ("xDirection", rightLeftMagnitude);
        
        
       
    }


    public void PointPlayerTowardsMouse()
    {
        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }
        lookDirection = (mousePos - transform.position).normalized;
        lookRotation = Quaternion.LookRotation(lookDirection).normalized;

    }
    //Audio
    //private EventInstance playerFootsteps;

    public void Start()
    {
        //Note: 2 olika tutorials clashar..
        //playerFootsteps = AudioManager.instance.CreateInstance(playerAudio.footsteps);
       
    }

    private void UpdateSound()
    //Start footsteps event if the player has an x velocity and is on ground
    {
        if (rBody.velocity.x !=0)
        {
            //get the playback state
            //PLAYBACK_STATE playbackState;
            //playerFootsteps.getPlaybackState(out playbackState);
            //if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
               // playerFootsteps.start();
                //playerFootsteps.release();
            }
        } 
        //otherwise, stop the footsteps event
        else
        {
            //playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
