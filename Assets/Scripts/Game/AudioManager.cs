using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioManager instanceOfThis;

    public AudioSource lightCollisionSoundOne;
    public AudioSource lightCollisionSoundTwo;
    public AudioSource lightCollisionSoundThree;

    public AudioSource mediumCollisionSoundOne;
    public AudioSource mediumCollisionSoundTwo;
    public AudioSource mediumCollisionSoundThree;

    public AudioSource heavyCollisionSoundOne;
    public AudioSource heavyCollisionSoundTwo;
    public AudioSource heavyCollisionSoundThree;

    public AudioSource rollingChair;
    public AudioSource lightPushableObject;
    public AudioSource mediumPushableObject;
    public AudioSource heavyPushableObject;

    [Header("Runtime Arrys (Only populated during runtime)")]
    public AudioSource[] lightCollisionSounds;
    public AudioSource[] mediumCollisionSounds;
    public AudioSource[] heavyCollisionSounds;

    // Start is called before the first frame update
    void Awake()
    {

        if (instanceOfThis != null && instanceOfThis != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instanceOfThis = this;
        }
        //DontDestroyOnLoad(gameObject);


        lightCollisionSounds = new AudioSource[] { lightCollisionSoundOne, lightCollisionSoundTwo, lightCollisionSoundThree };
        mediumCollisionSounds = new AudioSource[] { mediumCollisionSoundOne, mediumCollisionSoundTwo, mediumCollisionSoundThree };
        heavyCollisionSounds = new AudioSource[] { heavyCollisionSoundOne, heavyCollisionSoundTwo, heavyCollisionSoundThree };
    }
}
