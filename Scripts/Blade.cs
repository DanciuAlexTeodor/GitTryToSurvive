using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public bool playerInRange;
    public float distance=2f;

    private void Update()
    {
        float distanceFromPlayerToAnvil = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);
        if (distanceFromPlayerToAnvil <= distance && distanceFromPlayerToAnvil>=1f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }

    
}
