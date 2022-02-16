using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundableItem : MonoBehaviour
{
    //AudioSource soundEffect;
    //bool hasPlayed = false;
    AudioSource[] collisionSounds; 

    private void Awake()
    {
        if (GetComponent<AudioSource>())
        {
            collisionSounds = GetComponents<AudioSource>();
            //soundEffect = GetComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2f)
        {
            int indexOfSoundToPlay = Random.Range(0, collisionSounds.Length);

            //The sound of a collision is depedent on the speed of the collision
            collisionSounds[indexOfSoundToPlay].volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 40); 
            collisionSounds[indexOfSoundToPlay].Play();
        }
    }

}