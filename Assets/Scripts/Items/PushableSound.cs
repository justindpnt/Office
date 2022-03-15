using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableSound : MonoBehaviour
{
    AudioSource continuousSound;
    Rigidbody objectRB;
    float audioDampenerFactor = 5f;
    float minimumMovementVelocity = .1f;

    //Sound type
    public enum pushingSound { none, chair, light, medium, heavy };

    //This item type
    public pushingSound soundType;


    // Start is called before the first frame update
    void Start()
    {
        AudioManager sceneAudioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        if (soundType == pushingSound.chair)
        {
            copySoundEffectsFromManager(sceneAudioManager.rollingChair);
            continuousSound = GetComponent<AudioSource>();
        }
        else if (soundType == pushingSound.light)
        {
            copySoundEffectsFromManager(sceneAudioManager.lightPushableObject);
            continuousSound = GetComponent<AudioSource>();
        }
        else if (soundType == pushingSound.medium)
        {
            copySoundEffectsFromManager(sceneAudioManager.mediumPushableObject);
            continuousSound = GetComponent<AudioSource>();
        }
        else if (soundType == pushingSound.heavy)
        {
            copySoundEffectsFromManager(sceneAudioManager.heavyPushableObject);
            continuousSound = GetComponent<AudioSource>();
        }
        objectRB = GetComponent<Rigidbody>();
    }
    private void copySoundEffectsFromManager(AudioSource soundFromManager)
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

    // Update is called once per frame
    void Update()
    {
        if(soundType != pushingSound.none)
        {
            if (Mathf.Sqrt(Mathf.Pow(objectRB.velocity.x, 2) + Mathf.Pow(objectRB.velocity.z, 2)) > minimumMovementVelocity)
            {
                continuousSound.volume = Mathf.Clamp01(
                    Mathf.Sqrt(Mathf.Pow(objectRB.velocity.x, 2) + Mathf.Pow(objectRB.velocity.z, 2)) / audioDampenerFactor);

                if (continuousSound.isPlaying != true)
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
}
