using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundableItem : MonoBehaviour
{
    AudioSource soundEffect;
    //bool hasPlayed = false;

    private void Awake()
    {
        if (GetComponent<AudioSource>())
        {
            soundEffect = GetComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 5f && soundEffect)
        {
            //hasPlayed = true;
            //soundEffect.loop = true;
            soundEffect.Play();
            //AudioSource.PlayClipAtPoint(soundEffect.clip, collision.transform.position);
        }
    }

}