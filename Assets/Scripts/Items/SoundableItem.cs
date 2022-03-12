using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class SoundableItem : MonoBehaviour
{
    //AudioSource soundEffect;
    //bool hasPlayed = false;
    public AudioSource[] collisionSounds;
    Rigidbody objectRB;
    Movement player;
    public float effectRadius = 20f;
    public bool shouldCreateCollisionSound = true;

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
        //If this object has an audio source, then we put that 
        if (shouldCreateCollisionSound)
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
                        playSoundandVibrate = true;
                    }
                }
                else
                {
                    playSoundandVibrate = true;
                }
            }
            else
            {
                playSoundandVibrate = true;
            }

            if (playSoundandVibrate)
            {
                int indexOfSoundToPlay = Random.Range(0, collisionSounds.Length);

                //The sound of a collision is depedent on the speed of the collision
                collisionSounds[indexOfSoundToPlay].volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 160);
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
                        HapticPatterns.PlayEmphasis(Mathf.Clamp01(collision.relativeVelocity.magnitude / 20), 0.0f);
                    }
                }
            }
        }
    }
}