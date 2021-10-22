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

    private PickableItem pickedItem;
    private PlayerController controller;

    public Image defaultCursor;
    public Image enabledCursor;

    private void Start()
    {
        controller = GetComponent<PlayerController>();
    }


    // Update is called once per frame
    private void Update()
    {
        if(pickedItem)
        {
            updateItemPosition();
            processRotation();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // If there is already an item picked up, do not pick up another item
            if (pickedItem)
            {
                DropItem(pickedItem);
            }
            else
            {
                var ray = characterCamera.ViewportPointToRay(Vector3.one * .5f);
                RaycastHit hit;

                //if hit
                if(Physics.Raycast(ray, out hit, pickupDistance))
                {
                    var canPickUp = hit.transform.GetComponent<PickableItem>();

                    //if the item can be picked up
                    if (canPickUp)
                    {
                        pickUpItem(canPickUp);
                    }
                }
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

    private void DropItem(PickableItem item)
    {
        pickedItem = null;
        item.transform.SetParent(null);
        item.Rb.useGravity = true;

        item.Rb.AddForce(characterCamera.transform.forward * throwSpeed, ForceMode.VelocityChange);
    }
}
