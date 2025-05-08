using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneSoundManager : MonoBehaviour
{
    public static MainSceneSoundManager Instance { get; set; }

    //every time i want to implement a new audio, i will
    //have to add a new public AudioSource variable

    //Sounds 
    public AudioSource hoverButton;
    public AudioSource clickButton;

    public void PlaySound(AudioSource sound)
    {
        if (!sound.isPlaying)
        {
            sound.Play();
        }
    }



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure this object isn't destroyed between scenes
        }
    }
}
