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

        if(rigidbody != null)
        {
            // If on top of any items, don't add a force to it
            if (controller.isStandingOn())
            {
                // Get objects player is on top of
                Collider[] itemsCurrentlyOn = controller.itemsStandingOn();

                foreach (Collider item in itemsCurrentlyOn)
                {
                    //Don't add a force to anything you are currently on top of
                    if (rigidbody.gameObject.GetInstanceID() != item.transform.parent.GetInstanceID())
                    {
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

                rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
            }
        }
    }
}
