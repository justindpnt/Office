using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePush : MonoBehaviour
{
    [SerializeField] private float forceMultiplier;
    Movement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<Movement>();
    }

    // Apply a kick force on anything that interacts with the player
    private void OnCollisionEnter(Collision hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        if(rigidbody != null)
        {
            // If on top of any items, don't add a force to it
            if (playerMovement.isStandingOn())
            {
                // Get objects player is on top of
                Collider[] itemsCurrentlyOn = playerMovement.itemsStandingOn();

                foreach (Collider item in itemsCurrentlyOn)
                {
                    //Don't add a force to anything you are currently on top of
                    if (rigidbody.gameObject.GetInstanceID() != item.transform.parent.GetInstanceID())
                    {
                        Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                        forceDirection.y = 0;
                        forceDirection.Normalize();

                        //Add a force relative to how fast you are moving compared to the object
                        rigidbody.AddForceAtPosition(forceDirection * forceMultiplier * hit.relativeVelocity.magnitude, transform.position, ForceMode.Impulse);
                    }
                }
            }

            // Else you are free to kick the object
            else
            {
                Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                forceDirection.y = 0;
                forceDirection.Normalize();

                rigidbody.AddForceAtPosition(forceDirection * forceMultiplier, transform.position, ForceMode.Impulse);
            }
        }
    }
}
