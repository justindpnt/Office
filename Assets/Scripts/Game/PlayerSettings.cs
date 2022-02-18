using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings instanceOfThis;

    public float mouseSensetivity = .5f;
    public float masterVolume = 0f;

    //Create singleton for playersettings
    private void Awake()
    {
        if(instanceOfThis == null)
        {
            instanceOfThis = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    //Method called from UX menu from slider
    public void updateMouseSensitivity(float newMouseSense)
    {
        mouseSensetivity = newMouseSense;
    }

    //Method called from UX menu from slider
    public void updateGameVolume(float newGameVolume)
    {
        masterVolume = newGameVolume;
    }
}
