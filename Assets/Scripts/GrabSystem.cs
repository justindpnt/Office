using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrabSystem : MonoBehaviour
{
    [SerializeField] private Camera characterCamera;
    [SerializeField] public float pickupDistance = 1.5f;
    [SerializeField] public float itemOffset = 1f;
    [SerializeField] public float smooth = 1f;
    [SerializeField] public float throwSpeed = 1f;
    [SerializeField] public float itemSpeed = 1f;
    [SerializeField] public float rotationSpeed = 1f;
    [SerializeField] public float powerFillSpeed = 1f;
    [SerializeField] public float powerDivisor = 1f;
    [SerializeField] public float spinSpeed = 10f;

    private PickableItem pickedItem;
    private PlayerController controller;

    public Image defaultCursor;
    public Image enabledCursor;
    public Image powerBar;
    public bool canThrow = false;

    public float power, maxPower = 100f;
    

    private void Start()
    {
        controller = GetComponent<PlayerController>();
        power = 0f;
        powerBar.fillAmount = 0f;
    }


    // Update is called once per frame
    private void Update()
    {
        if(pickedItem)
        {
            updateItemPosition();
            processRotation();
            defaultCursor.enabled = false;
            enabledCursor.enabled = false;

            if (Input.GetKey(KeyCode.E) && canThrow)
            {
                powerBar.enabled = true;
                if(power < maxPower)
                {
                    power += Time.deltaTime * powerFillSpeed;
                }
                else
                {
                    power = maxPower;
                }
                powerBar.fillAmount = power / maxPower;
            }

            if (Input.GetKeyUp(KeyCode.E) && canThrow)
            {
                DropItem(pickedItem, power);
                powerBar.enabled = false;
                canThrow = false;
                power = 0f;
                defaultCursor.enabled = true;
                enabledCursor.enabled = false;
            }

            else if (Input.GetKeyUp(KeyCode.E))
            {
                canThrow = true;
            }

        }
        else
        {
            var rayPickup = characterCamera.ViewportPointToRay(Vector3.one * .5f);
            RaycastHit hitPickup;

            //if hit
            if (Physics.Raycast(rayPickup, out hitPickup, pickupDistance))
            {
                var canPickUp = hitPickup.transform.GetComponent<PickableItem>();

                //if the item can be picked up
                if (canPickUp)
                {
                    defaultCursor.enabled = false;
                    enabledCursor.enabled = true;
                    if (Input.GetKeyDown(KeyCode.E))
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
    }

    private void processRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            controller.setLookBool(false);
        }
        if (Input.GetMouseButtonUp(1))
        {
            controller.setLookBool(true);
        }
        if (Input.GetMouseButton(1))
        {
            updateItemRotation();
        }
    }

    private void updateItemPosition()
    {
       pickedItem.Rb.freezeRotation = true;

       Vector3 moveDirection = (characterCamera.transform.forward * itemOffset) + characterCamera.transform.position - pickedItem.transform.position;
       pickedItem.Rb.velocity = moveDirection * itemSpeed;

       pickedItem.Rb.freezeRotation = false;
    }

    private void updateItemRotation()
    {
        pickedItem.transform.Rotate(transform.up, -Input.GetAxis("Mouse X") * rotationSpeed, Space.World);
        pickedItem.transform.Rotate(characterCamera.transform.right, Input.GetAxis("Mouse Y") * rotationSpeed, Space.World);
    }

    //Pick up item
    private void pickUpItem(PickableItem item)
    {
        // Tie item to player
        pickedItem = item;

        // Turn off physics on rigidbody
        item.Rb.useGravity = false;
        item.Rb.velocity = Vector3.zero;
        item.Rb.angularVelocity = Vector3.zero;

        //Set parent to character
        item.transform.SetParent(transform);
    }

    private void DropItem(PickableItem item, float power)
    {
        pickedItem = null;
        item.transform.SetParent(null);
        item.Rb.useGravity = true;

        item.Rb.AddForce(characterCamera.transform.forward * power / powerDivisor, ForceMode.Impulse);
        item.Rb.AddTorque(spinSpeed, 0f, 0f, ForceMode.Force);
    }
}
