using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public float offset = .2f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, offset, 0);
        GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
