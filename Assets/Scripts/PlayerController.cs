using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 1f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float crouchSpeed = 3f;
    float moveSpeed;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = .3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = .03f;
    [SerializeField] bool lockCursor = true;
    float crouchTimeCounter;
    [SerializeField] float crouchTime = 1f;
    [SerializeField] float crouchHieght = 1f;

    float storedCrouchedScale;
    float storedStandingScale;

    float cameraPitch = 0.0f;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public Transform groundCheck;
    public float groundDistance = .4f;
    public float itemStandDistance = .1f;
    public LayerMask groundMask;
    public LayerMask itemMask;

    public bool isCrouched = false;
    public bool isGrounded;
    Vector3 verticalVelocity;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    private bool canLook = true;

    GrabSystem grabSystem;

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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGroundStatus();
        if (canLook)
        {
            UpdateMouseLook();
        }
        UpdateMovement();
    }

    void UpdateGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    
    
    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
        //playerCamera.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);

        
    }

    void UpdateMovement()
    {
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        if (Input.GetKeyDown("space") && (isGrounded||isOnItem()))
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        HandleCrouch();

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * moveSpeed;

        controller.Move(velocity * Time.deltaTime);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (isCrouched)
        {
            moveSpeed = crouchSpeed;

            if (transform.localScale.y > crouchHieght)
            {
                if (crouchTimeCounter < crouchTime)
                {
                    grabSystem.getHeldItem().transform.SetParent(null);
                    transform.localScale = new Vector3(1f, Mathf.Lerp(storedStandingScale, crouchHieght, crouchTimeCounter / crouchTime), 1f);
                    crouchTimeCounter += Time.deltaTime;
                    grabSystem.getHeldItem().transform.SetParent(transform);

                    //float FixeScale = grabSystem.getHeldItemScale();

                    //grabSystem.getHeldItem().transform.localScale = new Vector3(grabSystem.getHeldItemScale(), grabSystem.getHeldItemScale(), grabSystem.getHeldItemScale());

                   //grabSystem.getHeldItem().transform.localScale =
                         //new Vector3(FixeScale,
                            //FixeScale / transform.localScale.y,
                            //FixeScale);
                }
            }
        }
        else
        {
            moveSpeed = walkSpeed;

            if (transform.localScale.y < Vector3.one.y)
            {
                if (crouchTimeCounter < crouchTime)
                {
                    grabSystem.getHeldItem().transform.SetParent(null);
                    transform.localScale = new Vector3(1f, Mathf.Lerp(storedCrouchedScale, 1f, crouchTimeCounter / crouchTime), 1f);
                    crouchTimeCounter += Time.deltaTime;
                    controller.center = Vector3.up * controller.height / 2f;

                    grabSystem.getHeldItem().transform.SetParent(transform);

                    //float FixeScale = grabSystem.getHeldItemScale();

                    //grabSystem.getHeldItem().transform.localScale =
                    //new Vector3(FixeScale / transform.localScale.x,
                    //FixeScale / transform.localScale.y,
                    //FixeScale / transform.localScale.z);
                }
            }
            else
            {
                controller.center = new Vector3(0f, 1f, 0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouched = true;
            crouchTimeCounter = 0;
            storedStandingScale = transform.localScale.y;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouched = false;
            crouchTimeCounter = 0;
            storedCrouchedScale = transform.localScale.y;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, itemStandDistance);
    }

    public bool isOnItem()
    {
        return Physics.CheckSphere(groundCheck.position, itemStandDistance, itemMask);
    }

    public void setLookBool(bool state){canLook = state;}
    public bool getLookBool(){return canLook;}
}
