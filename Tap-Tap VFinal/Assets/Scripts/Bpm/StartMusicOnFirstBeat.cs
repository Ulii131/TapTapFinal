using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMusicOnFirstBeat : MonoBehaviour
{
    public Image bpmIndicatorImage;      // El Image del Canvas que cambia de color
    public AudioSource musicSource;

    private bool musicStarted = false;

    void Update()
    {
        if (!musicStarted && bpmIndicatorImage.color == Color.green)
        {
            musicSource.Play();
            musicStarted = true;
        }
    }
}
