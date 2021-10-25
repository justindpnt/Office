using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform cameraSpot;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraSpot.transform.position;

        //Debug.Log(Quaternion.Euler(new Vector3(cameraSpot.parent.transform.rotation.x, cameraSpot.parent.transform.rotation.y, cameraSpot.parent.transform.rotation.z)));

        Vector3 eulerRotationY = new Vector3(
    0,
    cameraSpot.parent.transform.eulerAngles.y,
    0);
        Vector3 eulerRotationX = new Vector3(
            cameraSpot.parent.transform.eulerAngles.x,
            0,
            0);
        transform.eulerAngles = eulerRotationX;
        transform.eulerAngles += eulerRotationY;

    }
}