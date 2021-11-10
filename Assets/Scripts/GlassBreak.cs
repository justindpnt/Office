using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour
{
    public float minRequiredBreakMagnitude = 1f;
    public Transform brokenWindowObject;
    public Transform windowObject;

    private void OnTriggerEnter(Collider collider)
    {        
        if(collider.attachedRigidbody != null)
        {
            if(collider.attachedRigidbody.velocity.magnitude > 1f)
            {
                //Debug.Log(collider.attachedRigidbody.name);
               // Debug.Log(collider.attachedRigidbody.velocity.magnitude);
            }
            
            if (collider.attachedRigidbody.velocity.magnitude > minRequiredBreakMagnitude)
            {
                windowObject.gameObject.SetActive(false);
                brokenWindowObject.gameObject.SetActive(true);
            }
        }
    }
}
