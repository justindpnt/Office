using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableSound : MonoBehaviour
{
    AudioSource continuousSound;
    Rigidbody objectRB;
    public float audioDampenerFactor = 10f;
    public float minimumMovementVelocity = .1f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        continuousSound = GetComponent<AudioSource>();
        objectRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Sqrt(Mathf.Pow(objectRB.velocity.x, 2) + Mathf.Pow(objectRB.velocity.z, 2)) > minimumMovementVelocity)
        {
            continuousSound.volume = Mathf.Clamp01(
                Mathf.Sqrt(Mathf.Pow(objectRB.velocity.x, 2) + Mathf.Pow(objectRB.velocity.z, 2)) / audioDampenerFactor );

            if(continuousSound.isPlaying != true)
            {
                continuousSound.Play();
            }
        }
        else
        {
            continuousSound.Stop();
        }
    }
}
