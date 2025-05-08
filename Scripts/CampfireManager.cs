using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireManager : MonoBehaviour
{
    public static CampfireManager Instance { get; set; }

    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }




}
