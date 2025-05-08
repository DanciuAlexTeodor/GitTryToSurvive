using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    //every time i want to implement a new audio, i will
    //have to add a new public AudioSource variable
    //NPC voices
    public AudioSource mariaVoice;

    //Sounds 
    public AudioSource dropItemOnSlot;
    public AudioSource wood_hit;
    public AudioSource rock_hit;
    public AudioSource log_hit_ground;
    public AudioSource swing;
    public AudioSource grassWalkSound;
    public AudioSource grassRunSound;
    public AudioSource pickUpItem;
    public AudioSource creating_planks;
    public AudioSource appleEating;
    public AudioSource mushroomEating;
    public AudioSource construction;
    public AudioSource woodDoor;
    public AudioSource hoverButton;
    public AudioSource clickButton;
    public AudioSource takeMoney;
    public AudioSource meatCutting;
    public AudioSource dig;
    public AudioSource watering;
    public AudioSource openBox;
    public AudioSource closeBox;
    public AudioSource bandage;
    public AudioSource collectWater;
    public AudioSource drink;
    public AudioSource weaponDestroy;
    public AudioSource hammerHitIron;
    public AudioSource Heartbeat;
    public AudioSource HeartBeatAndDie;
    public AudioSource armorEquip;
    public AudioSource armorHit;
    public AudioSource lowEnergy;
    public AudioSource bowLoad;
    public AudioSource bowShoot;
    public AudioSource playerGetHit;
    public AudioSource fire;
    public AudioSource creatingIronItems;


    public void PlaySound(AudioSource sound)
    {
        if(!sound.isPlaying)
        {
            sound.Play();
        }
    }

    public void StopSound(AudioSource sound)
    {
        if(sound.isPlaying)
        {
            sound.Stop();
        }
    }

    //Music
    public AudioSource startingZoneMusic;



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

    public void PlayMariaVoice(AudioClip clip)
    {
        mariaVoice.clip = clip;

        if(!mariaVoice.isPlaying)
        {
            mariaVoice.Play();
        }
        else
        {
            mariaVoice.Stop();
            mariaVoice.Play();
        }
    }
}
