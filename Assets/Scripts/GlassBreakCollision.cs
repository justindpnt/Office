using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreakCollision : MonoBehaviour
{
    public float minRequiredBreakMagnitude = 1f;
    public Transform brokenWindowObject;
    public Transform windowObject;

    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log(collision.relativeVelocity.magnitude);

        if (collision.relativeVelocity.magnitude > minRequiredBreakMagnitude)
        {
            windowObject.gameObject.SetActive(false);
            brokenWindowObject.gameObject.SetActive(true);
            collision.rigidbody.AddForce(collision.rigidbody.velocity, ForceMode.Impulse);
        }
    }
}
