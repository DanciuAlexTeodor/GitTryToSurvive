using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAtSpawningPoint : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Restarting player position!");
            
            PlayerState.Instance.DecreaseHealth(200);
        }
    }
}
