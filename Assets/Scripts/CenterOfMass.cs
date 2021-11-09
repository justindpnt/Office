using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public float centerOfMassY = .2f;

    // Set the chair center of mass so it reacts to physics properly
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = new Vector3(0, centerOfMassY, 0);
        GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
    }
}
