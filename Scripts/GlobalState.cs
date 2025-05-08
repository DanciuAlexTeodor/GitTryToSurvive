using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalState : MonoBehaviour
{
    //we create a singleton pattern,
    //which ensures that only one instance of GlobalState
    //exists throughout the game.
    public static GlobalState Instance { get; set; }

    public float resourceHealth, resourceMaxHealth;
    //due to the singleton pattern, these variables
    //can be accessed and modified from any scene.

    private void Awake()
    {
        if (Instance != null && Instance!=this)
        { 
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
          
        }
    }
}
