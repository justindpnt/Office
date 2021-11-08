using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePush : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    PlayerController controller;

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Apply a kick force on anything that interacts with the player
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

       // Debug.Log("Kick" + rigidbody.gameObject.name);

        if(rigidbody != null)
        {
            // Get objects player is on top of
            Collider[] itemsCurrentlyOn = controller.itemsStandingOn();

            Debug.Log(itemsCurrentlyOn.Length);

            // If on top of any items, don't add a force to it
            if (itemsCurrentlyOn.Length > 0)
            {
                foreach (Collider item in itemsCurrentlyOn)
                {
                    Debug.Log(item.gameObject.name);
                    //Don't add a force to anything you are currently on top of
                    if (rigidbody.gameObject.GetInstanceID() != item.gameObject.GetInstanceID())
                    {
                        //Debug.Log("You are on top of:" + item.gameObject.name);
                        Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                        forceDirection.y = 0;
                        forceDirection.Normalize();

                        rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
                    }
                }
            }

            // Else you are free to kick the object
            else
            {
                Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                forceDirection.y = 0;
                forceDirection.Normalize();

                Debug.Log(forceDirection);

                rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
            }
        }
    }
}
