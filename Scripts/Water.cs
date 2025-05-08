using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public bool playerInRange;
    public float detectionDistance;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            SelectionManager.Instance.IsNearWater = false;
           
            //Debug.Log("Player entered the water.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SelectionManager.Instance.IsNearWater = true;
            playerInRange = false;
            //Debug.Log("Player exited the water.");
        }
    }
}
