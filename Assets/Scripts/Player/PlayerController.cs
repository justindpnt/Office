using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = .3f;
    float moveSpeed;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;
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
    public float jumpHeight = 3f;

    //Character controller info
    CharacterController controller = null;
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
        controller = GetComponent<CharacterController>();
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

    // Handle the left and right movement of the character
    private void HandleHorizontalMovement()
    {
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * moveSpeed;
        controller.Move(velocity * Time.deltaTime);
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
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    // Handle crouched movement of the character
    private void HandleCrouch()
    {
        if (isCrouched)
        {
            moveSpeed = crouchSpeed;

            if (controller.height > crouchHieght)
            {
                if (crouchTimeCounter < crouchTime)
                {
                    controller.height = Mathf.Lerp(storedStandingHeight, crouchHieght, crouchTimeCounter / crouchTime);
                    playerCamera.localPosition = new Vector3(0f, Mathf.Lerp(storedStandingHeight, crouchHieght, crouchTimeCounter / crouchTime), 0f);
                    crouchTimeCounter += Time.deltaTime;
                    controller.center = Vector3.up * controller.height / 2f;
                }
            }
        }
        else
        {
            moveSpeed = walkSpeed;

            if (controller.height < 2f)
            {
                if (crouchTimeCounter < crouchTime)
                {
                    playerCamera.localPosition = new Vector3(0f, Mathf.Lerp(storedCrouchedHeight, 2f, crouchTimeCounter / crouchTime), 0f);
                    controller.height = Mathf.Lerp(storedCrouchedHeight, 2f, crouchTimeCounter / crouchTime);
                    crouchTimeCounter += Time.deltaTime;
                    controller.center = Vector3.up * controller.height / 2f;
                }
            }
        }

        if (crouchPressed)
        {
            crouchPressed = false;
            isCrouched = true;
            crouchTimeCounter = 0;
            storedStandingHeight = controller.height;
        }

        if (crouchReleased)
        {
            crouchReleased = false;
            isCrouched = false;
            crouchTimeCounter = 0;
            storedCrouchedHeight = controller.height;
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
