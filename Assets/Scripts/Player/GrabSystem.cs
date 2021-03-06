using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabSystem : MonoBehaviour
{

    //Character controller cache
    [SerializeField] private Camera characterCamera;
    public LayerMask playerMask;
    private Movement playerMovement;

    //Item variables
    [SerializeField] public float pickupDistance = 1.5f;
    [SerializeField] public float itemOffsetFromPlayer = 1f;
    [SerializeField] public float itemMoveSpeed = 1f;
    [SerializeField] public float itemRotationSpeed = 1f;
    [SerializeField] public float itemControllerRotationSpeed = 1f;
    [SerializeField] public float powerBarFillSpeed = 1f;
    [SerializeField] public float powerMultiplier = 1f;
    [SerializeField] public float itemThrowSpinSpeed = 10f;
    private PickableItem pickedItem;
    public bool canThrow = false;
    private bool rotateObjectView = false;
    public float power, maxPower = 100f;
    public float stall, stallTime = 5f;


    //UI cache
    Image defaultCursor;
    Image enabledCursor;
    Image powerBar;

    //Aduio cache
    AudioSource pickUpSound;

    public PickableItem getHeldItem(){ return pickedItem; }

    private void Awake()
    {
        defaultCursor = GameObject.Find("UI/CenterDot").GetComponent<Image>();
        enabledCursor = GameObject.Find("UI/CenterDotConfirmed").GetComponent<Image>();
        powerBar = GameObject.Find("UI/ThrowPower").GetComponent<Image>();
    }

    private void Start()
    {
        playerMovement = GetComponent<Movement>();
        power = 0f;
        powerBar.fillAmount = 0f;
        stall = 0f;

        pickUpSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessControllerRT();

        if (pickedItem)
        {
            processRotation();
            defaultCursor.enabled = false;
            enabledCursor.enabled = false;
            processThrow();
        }

        //This means no items are picked up, check if we can pick up a new one
        else
        {
            processPlayerInteraction();
        }
    }
    enum rightBumperPressedState { notPresseed, firstFramePressed, pressed, lastFramePressed }
    rightBumperPressedState rightBumperState = rightBumperPressedState.notPresseed;
    bool wasRightBumperPressedLastFrame = false;

    private void ProcessControllerRT()
    {
        if (Input.GetAxis("ControllerThrow") > 0)
        {
            if (wasRightBumperPressedLastFrame)
            {
                rightBumperState = rightBumperPressedState.pressed;
            }
            else
            {
                rightBumperState = rightBumperPressedState.firstFramePressed;
                wasRightBumperPressedLastFrame = true;
            }
        }
        else if (Input.GetAxis("ControllerThrow") < .1)
        {
            if (wasRightBumperPressedLastFrame)
            {
                rightBumperState = rightBumperPressedState.lastFramePressed;
                wasRightBumperPressedLastFrame = false;
            }
            else
            {
                rightBumperState = rightBumperPressedState.notPresseed;
            }
        }
    }

    private void processPlayerInteraction()
    {
        var rayPickup = characterCamera.ViewportPointToRay(Vector3.one * .5f);
        RaycastHit hitPickup;

        //if hit
        if (Physics.Raycast(rayPickup, out hitPickup, pickupDistance, ~playerMask))
        {
            var canPickUp = hitPickup.transform.GetComponent<PickableItem>();

            //if the item can be picked up
            if (canPickUp)
            {
                defaultCursor.enabled = false;
                enabledCursor.enabled = true;
                
                if (Input.GetButtonDown("Interact") || (rightBumperState == rightBumperPressedState.firstFramePressed)) 
                {
                    defaultCursor.enabled = false;
                    enabledCursor.enabled = false;
                    pickUpItem(canPickUp);
                }
            }
            else
            {
                defaultCursor.enabled = true;
                enabledCursor.enabled = false;
            }
        }
        else
        {
            defaultCursor.enabled = true;
            enabledCursor.enabled = false;
        }
    }

    private void processThrow()
    {
        if ((Input.GetButton("Interact") || (rightBumperState == rightBumperPressedState.pressed))  && canThrow)
        {
            powerBar.enabled = true;
            if ((power < maxPower) && (stall > stallTime))
            {
                power += Time.deltaTime * powerBarFillSpeed;
            }
            else if((stall > stallTime))
            {
                power = maxPower;
            }
            else
            {
                stall += Time.deltaTime * powerBarFillSpeed;
            }
            powerBar.fillAmount = power / maxPower;
        }

        if ((Input.GetButtonUp("Interact") || (rightBumperState == rightBumperPressedState.lastFramePressed)) && canThrow)
        {
            DropItem(pickedItem, power);
            powerBar.enabled = false;
            canThrow = false;
            power = 0f;
            stall = 0f;
            defaultCursor.enabled = true;
            enabledCursor.enabled = false;
        }

        else if (Input.GetButtonUp("Interact") || (rightBumperState == rightBumperPressedState.lastFramePressed))
        {
            canThrow = true;
        }
    }

    private void FixedUpdate()
    {
        if (pickedItem)
        {
            //If right click is being held
            if (rotateObjectView)
            {
                rotateHeldItem();
            }
            updateItemPosition();
        }   
    }

    bool isUsingControllerToInspect = false;

    private void processRotation()
    {
        // Right click inspect
        if (Input.GetMouseButtonDown(1))
        {
            playerMovement.canLook = false;
            rotateObjectView = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            rotateObjectView = false;
            playerMovement.canLook = true;
        }

        if (Input.GetButtonDown("Inspect"))
        {
            isUsingControllerToInspect = true;
            playerMovement.canLook = false;
            rotateObjectView = true;
        }

        if (Input.GetButtonUp("Inspect"))
        {
            isUsingControllerToInspect = false;
            rotateObjectView = false;
            playerMovement.canLook = true;
        }
    }

    public void makeItemFacePlayer(Vector3 angle)
    {
        if (!rotateObjectView) 
        {
            if (pickedItem)
            {
                // Alternate, but not as good, way to rotate the object
                //pickedItem.transform.Rotate(transform.up, angle.y, Space.World);

                // Rotate the object around the player
                pickedItem.transform.RotateAround(transform.position, transform.up, angle.y);
            }
        }
    }

    //Move position of picked up item to stay around player
    private void updateItemPosition()
    {
        pickedItem.Rb.freezeRotation = true;

        Vector3 moveDirection = ((characterCamera.transform.forward * itemOffsetFromPlayer) + characterCamera.transform.position) - pickedItem.transform.position;
        pickedItem.Rb.velocity = moveDirection * itemMoveSpeed;

        pickedItem.Rb.freezeRotation = false;
    }

    //Right click is held down
    private void rotateHeldItem()
    {
        if (isUsingControllerToInspect)
        {
            pickedItem.transform.Rotate(transform.up, -Input.GetAxis("Mouse X") * itemControllerRotationSpeed * playerMovement.mouseSensitivityMultiplier * 2, Space.World);
            pickedItem.transform.Rotate(characterCamera.transform.right, Input.GetAxis("Mouse Y") * itemControllerRotationSpeed * playerMovement.mouseSensitivityMultiplier * 2, Space.World);
        }
        else
        {
            pickedItem.transform.Rotate(transform.up, -Input.GetAxis("Mouse X") * itemRotationSpeed * playerMovement.mouseSensitivityMultiplier * 2, Space.World);
            pickedItem.transform.Rotate(characterCamera.transform.right, Input.GetAxis("Mouse Y") * itemRotationSpeed * playerMovement.mouseSensitivityMultiplier * 2, Space.World);
        }  
    }

    private void pickUpItem(PickableItem item)
    {
        pickUpSound.Play();

        // Tie item to player
        pickedItem = item;

        // Turn off physics on rigidbody
        item.Rb.useGravity = false;
        item.Rb.velocity = Vector3.zero;
        item.Rb.angularVelocity = Vector3.zero;

        // Reset glass counter
        item.hasAGlassPaneBeenHit = false;

        // Set object and all children parts to being the status of picked up
        item.gameObject.layer = LayerMask.NameToLayer("PickedUp");
        foreach(Transform child in item.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("PickedUp");
        }
    }

    //Drop item
    private void DropItem(PickableItem item, float power)
    {
        // Break tie between player and object
        pickedItem = null;
        
        // Turn on physics on rigidbody
        item.Rb.useGravity = true;

        // Set object to no longer being picked up
        item.gameObject.layer = LayerMask.NameToLayer("Item");
        foreach (Transform child in item.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Item");
        }

        // Add a throw force to the object
        item.Rb.AddForce(characterCamera.transform.forward * power * powerMultiplier, ForceMode.VelocityChange);

        // Add torque if there is a throw force
        if (power != 0)
        {     
            item.Rb.AddTorque(itemThrowSpinSpeed, 0f, 0f, ForceMode.Force);
        }
    }
}
