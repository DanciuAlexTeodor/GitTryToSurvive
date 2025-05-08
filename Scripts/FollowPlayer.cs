using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerl;
    public Vector3 offset;//optional

    private void LateUpdate()
    {
        if(playerl != null)
        {
            transform.position = playerl.position + offset;
        }
    }
}
