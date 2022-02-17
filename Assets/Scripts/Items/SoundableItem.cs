using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundableItem : MonoBehaviour
{
    //AudioSource soundEffect;
    //bool hasPlayed = false;
    AudioSource[] collisionSounds;
    Rigidbody objectRB;

    private void Awake()
    {
        objectRB = GetComponent<Rigidbody>();

        //If this object has an audio source, then we put that 
        if (GetComponent<AudioSource>())
        {
            collisionSounds = GetComponents<AudioSource>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool playSound = false;

        if (collision.relativeVelocity.magnitude > 2f)
        {
            //if the object has a parent (which means it is a collider object)
            if (collision.collider.transform.parent != null)
            {
                //if that object has a soundable item
                if (collision.collider.transform.parent.gameObject.GetComponent<SoundableItem>())
                {
                    //Only the faster moving object in a collsiion should make a sound
                    if (objectRB.velocity.magnitude > collision.collider.transform.parent.gameObject.GetComponent<Rigidbody>().velocity.magnitude)
                    {
                        playSound = true;
                    }
                }
                else
                {
                    playSound = true;
                }
            }
            else
            {
                playSound = true;
            }

            if (playSound)
            {
                int indexOfSoundToPlay = Random.Range(0, collisionSounds.Length);

                //The sound of a collision is depedent on the speed of the collision
                collisionSounds[indexOfSoundToPlay].volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 40);
                collisionSounds[indexOfSoundToPlay].Play();
            }
        }
    }
}