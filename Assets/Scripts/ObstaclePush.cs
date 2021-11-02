using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePush : MonoBehaviour
{
    [SerializeField] private float forceMagnitude;
    PlayerController controller;
    GrabSystem grabSystem;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        grabSystem = GetComponent<GrabSystem>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rigidbody = hit.collider.attachedRigidbody;
        
        if(rigidbody != null)
        {
            foreach (Collider item in controller.isOnItem())
            {
                if (rigidbody.gameObject.GetInstanceID() != item.gameObject.GetInstanceID())
                {
                    Vector3 forceDirection = hit.gameObject.transform.position - transform.position;
                    forceDirection.y = 0;
                    forceDirection.Normalize();

                    rigidbody.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
                }
            }
        }

        /*
        if (rigidbody != null && !controller.isOnItem())
        {

        }
        */
    }
}
