using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : MonoBehaviour
{
    public bool playerInRange;

    void Update()
    {
        float distanceFromPlayerToWell = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);
        if (distanceFromPlayerToWell <= 4f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }
}
