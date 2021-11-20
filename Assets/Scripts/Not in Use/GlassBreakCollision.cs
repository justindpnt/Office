using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This script is not used as of right now. When using a collision, the velocity of the object is always stopped and the collision don't look realistic.
public class GlassBreakCollision : MonoBehaviour
{
    public float minRequiredBreakMagnitude = 1f;
    public Transform brokenWindowObject;
    public Transform windowObject;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > minRequiredBreakMagnitude)
        {
            windowObject.gameObject.SetActive(false);
            brokenWindowObject.gameObject.SetActive(true);
            collision.rigidbody.AddForce(collision.rigidbody.velocity, ForceMode.Impulse);
        }
    }
}
