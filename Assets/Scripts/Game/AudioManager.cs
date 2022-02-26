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

    // Start is called before the first frame update
    void Awake()
    {
        if (instanceOfThis == null)
        {
            instanceOfThis = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AudioSource[] lightCollisionSounds = { lightCollisionSoundOne, lightCollisionSoundTwo, lightCollisionSoundThree };
        AudioSource[] mediumCollisionSounds = { mediumCollisionSoundOne, mediumCollisionSoundTwo, mediumCollisionSoundThree };
        AudioSource[] heavyCollisionSounds = { heavyCollisionSoundOne, heavyCollisionSoundTwo, heavyCollisionSoundThree };
    }
}
