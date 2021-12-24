using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //Mouse variables
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 1f;
    public float mouseSensitivityMultiplier;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = .03f;
    [SerializeField] bool lockCursor = true;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;
    Vector2 targetMouseDelta;
    Vector2 targetDir;
    float cameraPitch = 0.0f;
    public bool canLook { get; set; }

    //Movement variables
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float crouchSpeed = 3f;
    float moveSpeed;
    public bool canMove { get; set; }

    //Crouch variables
    [SerializeField] float crouchTime = 1f;
    [SerializeField] float crouchHieght = 1f;
    private bool crouchPressed = false;
    private bool crouchReleased = false;
    float crouchTimeCounter;
    float storedCrouchedHeight;
    float storedStandingHeight;
    public bool isCrouched = false;

    //Jump variables
    public bool isGrounded;
    private bool spacePressed = false;
    Vector3 verticalVelocity;
    public float gravity = -9.81f;
    public float jumpMultiplier = 3f;

    //Character controller info
    Rigidbody rb;
    CapsuleCollider capCollider;
    public Transform groundCheck;
    public float groundDistance = .4f;
    LayerMask groundMask;
    LayerMask itemMask;
    GrabSystem grabSystem;

    //Game Settings cache
    PlayerSettings settings;

    //Always have a fresh pointer to the one active game settings
    private void Awake()
    {
        settings = FindObjectOfType<PlayerSettings>() as PlayerSettings;
        mouseSensitivityMultiplier = settings.mouseSensetivity;
    }

    // Start is called before the first frame update
    void Start()
    {
        grabSystem = GetComponent<GrabSystem>();
        rb = GetComponent<Rigidbody>();
        capCollider = GetComponent<CapsuleCollider>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        canLook = true;
        canMove = true;

        groundMask = 1 << LayerMask.NameToLayer("Ground");
        itemMask = 1 << LayerMask.NameToLayer("Item");

        //Ground is if the player is on an object or the ground
        groundMask |= itemMask;
        
    }

    // Update things that don't involve rigidbodies
    void Update()
    {
        UpdateGroundStatus();
        collectInput();
    }

    // Update things that do involve rigidodies
    private void FixedUpdate()
    {
        if (canMove)
        {
            UpdateMovement();
        }
        if (canLook)
        {
            UpdateMouseLook();
        }
    }

    // Only collect input from user during update, don't move any rigidbodies
    private void collectInput()
    {
        if (canLook)
        {
            targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        }

        targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        if (canMove)
        {
            if (Input.GetKeyDown("space") && (isGrounded))
            {
                spacePressed = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouchPressed = true;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                crouchReleased = true;
            }
        }
    }

    void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    
    void UpdateMouseLook()
    {
        cameraPitch -= currentMouseDelta.y * mouseSensitivity * mouseSensitivityMultiplier * 2;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity * mouseSensitivityMultiplier * 2);

        //pass rotation to grab system, neccesary to keep the item facing the player
        grabSystem.makeItemFacePlayer(Vector3.up * currentMouseDelta.x * mouseSensitivity * mouseSensitivityMultiplier * 2);
    }

    // Update the movement of the character
    void UpdateMovement()
    {
         HandleCrouch();
         HandleVerticalMovement();
         HandleHorizontalMovement();
    }

    //don't touch
    float accelerationTimer = 0;
    float deccelerationTimer = 0;
    float maxTimerValue = 50f;
    
    //increase for overall faster fill speed
    public float speedTimerFillSpeed = 1f;
    public float firstZoneTimeBoundary;
    public float secondZoneTimeBoundary;
    public float firstZoneDivisorFactor;
    public float secondZoneDivisorFactor;

    Vector2 lastKnownTargetDir = new Vector2(0,0);

    // Handle the left and right movement of the character
    private void HandleHorizontalMovement()
    {
        if (targetDir.magnitude > 0)
        {
            deccelerationTimer = 0f;
            
            if(accelerationTimer > maxTimerValue)
            {
                accelerationTimer = maxTimerValue;
            }
            else
            {
                accelerationTimer += Time.deltaTime * speedTimerFillSpeed;
            }

            if(accelerationTimer < firstZoneTimeBoundary)
            {
                rb.velocity = transform.rotation * 
                    new Vector3(targetDir.x * moveSpeed / firstZoneDivisorFactor, 
                        rb.velocity.y, 
                        targetDir.y * moveSpeed / firstZoneDivisorFactor);
                lastKnownTargetDir = targetDir;
            }
            else if (firstZoneTimeBoundary < accelerationTimer && accelerationTimer < secondZoneTimeBoundary)
            {
                rb.velocity = transform.rotation *
                    new Vector3(targetDir.x * moveSpeed / secondZoneDivisorFactor, 
                        rb.velocity.y, 
                        targetDir.y * moveSpeed / secondZoneDivisorFactor);
                lastKnownTargetDir = targetDir;
            }
            else if (accelerationTimer > secondZoneTimeBoundary)
            {
                rb.velocity = transform.rotation * new Vector3(targetDir.x * moveSpeed, rb.velocity.y, targetDir.y * moveSpeed);
                lastKnownTargetDir = targetDir;
            }  
        }
        else
        {
            accelerationTimer = 0f;

            if (deccelerationTimer > maxTimerValue)
            {
                deccelerationTimer = maxTimerValue;
            }
            else
            {
                deccelerationTimer += Time.deltaTime * speedTimerFillSpeed;
            }

            if (deccelerationTimer < firstZoneTimeBoundary)
            {
                rb.velocity = transform.rotation *
                    new Vector3(lastKnownTargetDir.x * moveSpeed / secondZoneDivisorFactor,
                        rb.velocity.y,
                        lastKnownTargetDir.y * moveSpeed / secondZoneDivisorFactor);
            }
            else if (firstZoneTimeBoundary < deccelerationTimer && deccelerationTimer < secondZoneTimeBoundary)
            {
                rb.velocity = transform.rotation *
                    new Vector3(lastKnownTargetDir.x * moveSpeed / firstZoneDivisorFactor,
                        rb.velocity.y,
                        lastKnownTargetDir.y * moveSpeed / firstZoneDivisorFactor);
            }
            else if (deccelerationTimer > secondZoneTimeBoundary)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
    }

    // Handle the up and down movement of the character
    private void HandleVerticalMovement()
    {
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        if (spacePressed)
        {
            spacePressed = false;
            verticalVelocity.y = Mathf.Sqrt(jumpMultiplier * -2f * gravity);
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        rb.velocity = (new Vector3(rb.velocity.x, verticalVelocity.y, rb.velocity.z));
    }

    
    // Handle crouched movement of the character
    private void HandleCrouch()
    {
        if (isCrouched)
        {
            moveSpeed = crouchSpeed;

            if (capCollider.height > crouchHieght)
            {
                if (crouchTimeCounter < crouchTime)
                {
                    capCollider.height = Mathf.Lerp(storedStandingHeight, crouchHieght, crouchTimeCounter / crouchTime);
                    playerCamera.localPosition = new Vector3(0f, Mathf.Lerp(storedStandingHeight, crouchHieght, crouchTimeCounter / crouchTime), 0f);
                    crouchTimeCounter += Time.deltaTime;
                    capCollider.center = Vector3.up * capCollider.height / 2f;
                }
            }
        }
        else
        {
            moveSpeed = walkSpeed;

            if (capCollider.height < 2f)
            {
                if (crouchTimeCounter < crouchTime)
                {
                    playerCamera.localPosition = new Vector3(0f, Mathf.Lerp(storedCrouchedHeight, 2f, crouchTimeCounter / crouchTime), 0f);
                    capCollider.height = Mathf.Lerp(storedCrouchedHeight, 2f, crouchTimeCounter / crouchTime);
                    crouchTimeCounter += Time.deltaTime;
                    capCollider.center = Vector3.up * capCollider.height / 2f;
                }
            }
        }

        if (crouchPressed)
        {
            crouchPressed = false;
            isCrouched = true;
            crouchTimeCounter = 0;
            storedStandingHeight = capCollider.height;
        }

        if (crouchReleased)
        {
            crouchReleased = false;
            isCrouched = false;
            crouchTimeCounter = 0;
            storedCrouchedHeight = capCollider.height;
        }
    }
    
    

    // Draw ground check sphere
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

    public bool isStandingOn()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, itemMask);
    }

    public Collider[] itemsStandingOn()
    {
        return Physics.OverlapSphere(groundCheck.position, groundDistance, itemMask);
    }

    public void updateMouseSensMultiplier(float newSense)
    {
        mouseSensitivityMultiplier = newSense;
    }
}
