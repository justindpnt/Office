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
            Debug.Log(collider.attachedRigidbody.velocity.magnitude);
            if (collider.attachedRigidbody.velocity.magnitude > minRequiredBreakMagnitude)
            {
                windowObject.gameObject.SetActive(false);
                brokenWindowObject.gameObject.SetActive(true);
            }
        }


    }
}
