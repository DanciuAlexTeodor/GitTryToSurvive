using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVoice : MonoBehaviour
{
    public static PlayerVoice Instance { get; set; }

    public AudioSource lowHealth;
    public AudioSource lowCalories;
    public AudioSource lowWater;
    public AudioSource lowTemperature;
    

    private AudioSource currentPlayingSound; 

    public void PlayVoice(AudioSource sound)
    {
        if (currentPlayingSound != null && currentPlayingSound.isPlaying)
        {
            currentPlayingSound.Stop();
        }
        sound.Play();
        currentPlayingSound = sound;
    }

    public void StopVoice(AudioSource sound)
    {
        if (sound.isPlaying)
        {
            sound.Stop();

            if (currentPlayingSound == sound)
            {
                currentPlayingSound = null;
            }
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
            DontDestroyOnLoad(gameObject);
        }
    }
}
