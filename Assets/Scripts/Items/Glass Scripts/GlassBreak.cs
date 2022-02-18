using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    public float minRequiredBreakMagnitude = 1f;
    public Transform brokenWindowObject;
    public Transform windowObject;

    //This is placed on a barrier around the windows, and if it detects an object moving fast enough, breaks a window
    //More realistic collision if the object is moving fast enough
    private void OnTriggerEnter(Collider collider)
    {        
        if(collider.attachedRigidbody != null)
        {         
            if (collider.attachedRigidbody.velocity.magnitude > minRequiredBreakMagnitude)
            {
                windowObject.gameObject.SetActive(false);
                brokenWindowObject.gameObject.SetActive(true);
            }
        }
    }
}
