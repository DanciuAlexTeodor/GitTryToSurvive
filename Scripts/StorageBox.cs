using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : MonoBehaviour
{
    public bool playerInRange;

    public List<string> items;

    public enum BoxType
    {
        smallBox,
        bigBox
    }

    public BoxType thisBoxType;

    private void Update()
    {
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if(distance <= 6f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }
}
