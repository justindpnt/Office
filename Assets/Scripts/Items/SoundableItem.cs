using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class SoundableItem : MonoBehaviour
{
    //Sound Variables
    public AudioSource[] collisionSounds;
    Rigidbody objectRB;
    Movement player;

    float effectRadius = 20f; //Change this to increase/decrease vibration radius
    float soundDampFactor = 60f; //Decrease this number to increase sound volume

    float minimumCollisionVelocityToMakeSound = 3f;

    //Sound type
    public enum CollisionSound {lightObject, mediumObject, heavyObject };

    //This item type
    public CollisionSound soundType;


    private void Awake()
    {
        objectRB = GetComponent<Rigidbody>();
        player = FindObjectOfType<Movement>();
    }

    //This has to happen in start so the arrays in audioManager get initialized
    private void Start()
    {
        AudioManager sceneAudioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        if(soundType == CollisionSound.lightObject)
        {
            copySoundEffectsFromManager(sceneAudioManager.lightCollisionSounds);
        }
        else if(soundType == CollisionSound.mediumObject)
        {
            copySoundEffectsFromManager(sceneAudioManager.mediumCollisionSounds);
        }
        else if(soundType == CollisionSound.heavyObject)
        {
            copySoundEffectsFromManager(sceneAudioManager.heavyCollisionSounds);
        }

        collisionSounds = GetComponents<AudioSource>();
    }

    private void copySoundEffectsFromManager(AudioSource[] managerSounds)
    {
        foreach (var soundFromManager in managerSounds)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = soundFromManager.clip;
            audioSource.outputAudioMixerGroup = soundFromManager.outputAudioMixerGroup;
            audioSource.volume = soundFromManager.volume;
            audioSource.playOnAwake = soundFromManager.playOnAwake;
            audioSource.loop = soundFromManager.loop;
            audioSource.pitch = soundFromManager.pitch;
            audioSource.spatialBlend = soundFromManager.spatialBlend;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool playSoundandVibrate = false;

        if (collision.relativeVelocity.magnitude > minimumCollisionVelocityToMakeSound)
        {
            /* 
            //Commented out code here is using the old model structure where the colliders were not on the base level
            //of object and we had to acess the parent to get the sound
            //if the object has a parent (which means it is a collider object)
            //
            //if (collision.collider.transform.parent != null)
            //{
                //if the object we are hitting also has a soundable object as well
                if (collision.collider.transform.parent.gameObject.GetComponent<SoundableItem>())
                {
                    //Only the faster moving object in a collsiion should make a sound
                    if (objectRB.velocity.magnitude > collision.collider.transform.parent.gameObject.GetComponent<Rigidbody>().velocity.magnitude)
                    {
                        playSoundandVibrate = true;
                    }
                }
                else
                {
                    playSoundandVibrate = true;
                }
            //}
            //else
            //{
            //    playSoundandVibrate = true;
            //}
            */

            //tags where the object should NOT make a sound during collision
            if (!(collision.collider.tag.Contains("BreakableGlass")
                || collision.collider.tag.Contains("ShatteredGlass")
                || collision.collider.tag.Contains("GlassShard")))
            {

                if (!(objectRB.useGravity == false))
                {
                    //if the object we are hitting also has a soundable object as well
                    if (collision.collider.gameObject.GetComponent<SoundableItem>())
                    {
                        //Only the faster moving object in a collsiion should make a sound
                        if (objectRB.velocity.magnitude > collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude)
                        {
                            playSoundandVibrate = true;
                        }
                    }
                    else
                    {
                        playSoundandVibrate = true;
                    }
                }
            }

            if (playSoundandVibrate)
            {
                int indexOfSoundToPlay = Random.Range(0, collisionSounds.Length);

                //The sound of a collision is depedent on the speed of the collision
                collisionSounds[indexOfSoundToPlay].volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / soundDampFactor);
                collisionSounds[indexOfSoundToPlay].Play();

                //Only play rumble id there is a controller connected
                if((GamepadRumbler.IsConnected() != false) 
                    && !collision.transform.tag.Contains("Glass") 
                    && collision.relativeVelocity.magnitude > 4f)
                {
                    if ((player.transform.position - transform.position).magnitude < effectRadius)
                    {
                        //HapticPatterns.PresetType type = HapticPatterns.PresetType.Selection;
                        //HapticPatterns.PlayPreset(type);
                        //HapticPatterns.PlayConstant(Mathf.Clamp01(collision.relativeVelocity.magnitude / 40), 0.0f, .1f);

                        //This one is the one I was using most recently
                        //HapticPatterns.PlayEmphasis(Mathf.Clamp01(collision.relativeVelocity.magnitude / 20), 0.0f);
                    }
                }
            }
        }
    }
}