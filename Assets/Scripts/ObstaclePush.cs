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
        
        // If the player is standing on something, don't apply a kick force to that object
        if(rigidbody != null)
        {
            foreach (Collider item in controller.isOnItem())
            {
                //Don't add a force to anything you are currently on top of
                if (rigidbody.gameObject.GetInstanceID() != item.gameObject.GetInstanceID())
                {
                    Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                    forceDirection.y = 0;
                    forceDirection.Normalize();

                    rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
                }
            }
        }
    }
}
