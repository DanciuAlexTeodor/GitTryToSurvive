using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Bed : MonoBehaviour
{
    //public GameObject sleepingPhotos;
    public bool playerInRange;

    private void Update()
    {
        float distanceFromPlayertoBed = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);
        if (distanceFromPlayertoBed <= 6f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

    }

    public void Sleep()
    {
        PlayerState.Instance.currentHealth += 20;
        PlayerState.Instance.currentCalories -= 500;
        PlayerState.Instance.currentHydration -= 25;

        TimeManager.Instance.TriggerNextDay();
        DayNightSystem.Instance.currentTimeOfDay = 0.35f;
        
    }
}
