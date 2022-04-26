using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableItem : MonoBehaviour
{
    // Reference to the rigidbody
    private Rigidbody rb;
    public Rigidbody Rb => rb;

    public bool hasAGlassPaneBeenHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
}
