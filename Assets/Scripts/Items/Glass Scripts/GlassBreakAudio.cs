using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreakAudio : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<AudioSource>().Play();
    }
}
