using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Back up technique. If the object isn't moving fast enough to trigger the collider,
// then trigger this, which will at least still break the glass
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
