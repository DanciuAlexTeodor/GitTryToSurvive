using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeedShop : MonoBehaviour
{
    public static SeedShop Instance { get; set; }

    public Checkpoint checkPoint;
    public bool playerInRange;
    


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkPoint!=null)
        {
            playerInRange = true;
            checkPoint.isCompleted = true;
            QuestManager.Instance.RefreshTrackerList();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}
